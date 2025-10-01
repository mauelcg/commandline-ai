const express = require('express');
const fs = require('fs').promises;
const path = require('path');
const OpenAI = require('openai');

const app = express();
app.use(express.json());

// Initialize OpenAI
const openai = new OpenAI({
  apiKey: "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" 
//   apiKey: process.env.OPENAI_API_KEY
});

// Configuration
const WORKSPACE_DIR = process.env.WORKSPACE_DIR || './workspace';
const PORT = process.env.PORT || 3000;

// Ensure workspace directory exists
async function initWorkspace() {
  try {
    await fs.mkdir(WORKSPACE_DIR, { recursive: true });
    console.log(`Workspace initialized at: ${WORKSPACE_DIR}`);
  } catch (error) {
    console.error('Failed to initialize workspace:', error);
  }
}

// Helper: Get file metadata
async function getFileMetadata(filePath) {
  try {
    const stats = await fs.stat(filePath);
    const ext = path.extname(filePath);
    return {
      path: filePath,
      name: path.basename(filePath),
      size: stats.size,
      created: stats.birthtime,
      modified: stats.mtime,
      isDirectory: stats.isDirectory(),
      extension: ext
    };
  } catch (error) {
    throw new Error(`Failed to get metadata: ${error.message}`);
  }
}

// Helper: Resolve safe path within workspace
function resolveSafePath(relativePath) {
  const resolved = path.resolve(WORKSPACE_DIR, relativePath);
  if (!resolved.startsWith(path.resolve(WORKSPACE_DIR))) {
    throw new Error('Path traversal detected');
  }
  return resolved;
}

// File Operations
const fileOps = {
  async create(filePath, content = '') {
    const safePath = resolveSafePath(filePath);
    const dir = path.dirname(safePath);
    await fs.mkdir(dir, { recursive: true });
    await fs.writeFile(safePath, content);
    return { success: true, path: filePath, message: 'File created successfully' };
  },

  async read(filePath) {
    const safePath = resolveSafePath(filePath);
    const content = await fs.readFile(safePath, 'utf-8');
    const metadata = await getFileMetadata(safePath);
    return { success: true, content, metadata };
  },

  async update(filePath, content) {
    const safePath = resolveSafePath(filePath);
    await fs.writeFile(safePath, content);
    return { success: true, path: filePath, message: 'File updated successfully' };
  },

  async delete(filePath) {
    const safePath = resolveSafePath(filePath);
    const stats = await fs.stat(safePath);
    if (stats.isDirectory()) {
      await fs.rm(safePath, { recursive: true });
    } else {
      await fs.unlink(safePath);
    }
    return { success: true, path: filePath, message: 'File deleted successfully' };
  },

  async move(source, destination) {
    const safeSrc = resolveSafePath(source);
    const safeDest = resolveSafePath(destination);
    const destDir = path.dirname(safeDest);
    await fs.mkdir(destDir, { recursive: true });
    await fs.rename(safeSrc, safeDest);
    return { success: true, from: source, to: destination, message: 'File moved successfully' };
  },

  async copy(source, destination) {
    const safeSrc = resolveSafePath(source);
    const safeDest = resolveSafePath(destination);
    const destDir = path.dirname(safeDest);
    await fs.mkdir(destDir, { recursive: true });
    await fs.copyFile(safeSrc, safeDest);
    return { success: true, from: source, to: destination, message: 'File copied successfully' };
  },

  async search(query, options = {}) {
    const { extension, minSize, maxSize, modifiedAfter } = options;
    const results = [];

    async function scanDirectory(dir) {
      const entries = await fs.readdir(dir, { withFileTypes: true });
      
      for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);
        
        if (entry.isDirectory()) {
          await scanDirectory(fullPath);
        } else {
          const metadata = await getFileMetadata(fullPath);
          
          // Apply filters
          if (extension && metadata.extension !== extension) continue;
          if (minSize && metadata.size < minSize) continue;
          if (maxSize && metadata.size > maxSize) continue;
          if (modifiedAfter && metadata.modified < new Date(modifiedAfter)) continue;
          
          // Check if filename matches query
          if (metadata.name.toLowerCase().includes(query.toLowerCase())) {
            results.push(metadata);
          }
        }
      }
    }

    await scanDirectory(WORKSPACE_DIR);
    return { success: true, results, count: results.length };
  },

  async list(dirPath = '.') {
    const safePath = resolveSafePath(dirPath);
    const entries = await fs.readdir(safePath, { withFileTypes: true });
    const items = await Promise.all(
      entries.map(async (entry) => {
        const fullPath = path.join(safePath, entry.name);
        return await getFileMetadata(fullPath);
      })
    );
    return { success: true, items, count: items.length };
  }
};

// AI Command Parser
async function parseCommand(userInput, conversationHistory = []) {
  const messages = [
    {
      role: 'system',
      content: `You are a file management assistant. Parse user commands into structured actions.

Available operations:
- create: Create a file (params: path, content)
- read: Read a file (params: path)
- update: Update a file (params: path, content)
- delete: Delete a file (params: path)
- move: Move a file (params: source, destination)
- copy: Copy a file (params: source, destination)
- search: Search for files (params: query, options: {extension, minSize, maxSize, modifiedAfter})
- list: List directory contents (params: path)

Respond ONLY with valid JSON in this format:
{
  "operation": "operation_name",
  "params": { /* operation-specific parameters */ },
  "response": "A friendly response to the user"
}

Examples:
User: "Create a file called notes.txt with the content 'Hello World'"
{"operation": "create", "params": {"path": "notes.txt", "content": "Hello World"}, "response": "I'll create notes.txt with that content for you."}

User: "Find all PDF files"
{"operation": "search", "params": {"query": "", "options": {"extension": ".pdf"}}, "response": "Searching for all PDF files in your workspace."}

User: "Move report.txt to archive folder"
{"operation": "move", "params": {"source": "report.txt", "destination": "archive/report.txt"}, "response": "Moving report.txt to the archive folder."}`
    },
    ...conversationHistory,
    { role: 'user', content: userInput }
  ];

  const completion = await openai.chat.completions.create({
    model: 'gpt-4',
    messages,
    temperature: 0.3
  });

  const response = completion.choices[0].message.content;
  return JSON.parse(response);
}

// API Routes
app.post('/command', async (req, res) => {
  try {
    const { message, history } = req.body;
    
    if (!message) {
      return res.status(400).json({ error: 'Message is required' });
    }

    // Parse command using AI
    const parsed = await parseCommand(message, history);
    
    // Execute operation
    let result;
    if (fileOps[parsed.operation]) {
      result = await fileOps[parsed.operation](...Object.values(parsed.params));
    } else {
      throw new Error(`Unknown operation: ${parsed.operation}`);
    }

    res.json({
      success: true,
      parsed,
      result,
      aiResponse: parsed.response
    });
  } catch (error) {
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// Direct operation endpoints
app.post('/ops/create', async (req, res) => {
  try {
    const { path: filePath, content } = req.body;
    const result = await fileOps.create(filePath, content);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.get('/ops/read', async (req, res) => {
  try {
    const { path: filePath } = req.query;
    const result = await fileOps.read(filePath);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.put('/ops/update', async (req, res) => {
  try {
    const { path: filePath, content } = req.body;
    const result = await fileOps.update(filePath, content);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.delete('/ops/delete', async (req, res) => {
  try {
    const { path: filePath } = req.query;
    const result = await fileOps.delete(filePath);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.post('/ops/move', async (req, res) => {
  try {
    const { source, destination } = req.body;
    const result = await fileOps.move(source, destination);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.post('/ops/copy', async (req, res) => {
  try {
    const { source, destination } = req.body;
    const result = await fileOps.copy(source, destination);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.get('/ops/search', async (req, res) => {
  try {
    const { query, extension, minSize, maxSize, modifiedAfter } = req.query;
    const options = {};
    if (extension) options.extension = extension;
    if (minSize) options.minSize = parseInt(minSize);
    if (maxSize) options.maxSize = parseInt(maxSize);
    if (modifiedAfter) options.modifiedAfter = modifiedAfter;
    
    const result = await fileOps.search(query || '', options);
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.get('/ops/list', async (req, res) => {
  try {
    const { path: dirPath } = req.query;
    const result = await fileOps.list(dirPath || '.');
    res.json(result);
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// Start server
async function start() {
  await initWorkspace();
  app.listen(PORT, () => {
    console.log(`🚀 AI File Management Server running on port ${PORT}`);
    console.log(`📁 Workspace: ${path.resolve(WORKSPACE_DIR)}`);
    console.log(`\nEndpoints:`);
    console.log(`  POST /command - Natural language commands`);
    console.log(`  POST /ops/create - Create file`);
    console.log(`  GET  /ops/read - Read file`);
    console.log(`  PUT  /ops/update - Update file`);
    console.log(`  DELETE /ops/delete - Delete file`);
    console.log(`  POST /ops/move - Move file`);
    console.log(`  POST /ops/copy - Copy file`);
    console.log(`  GET  /ops/search - Search files`);
    console.log(`  GET  /ops/list - List directory`);
  });
}

start();