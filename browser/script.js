import user from './assets/user.svg'
import bullet from './assets/bullet.png'

const form = document.querySelector('form')
const limit = 500;

let loadInterval;

function loader(element) {
	element.textContent = '';
	loadInterval = setInterval(() => {
		element.textContent += '.';
		
		if(element.textContent === '....'){
			element.textContent = '';
		}
	}, 300);
}

function typeText(element, text){
	let index = 0;
	var event = new Event('input', {
		'bubbles': true,
		'cancelable': true
	});
	let interval = setInterval(() => {
		if(index < text.length){
		element.value += text.charAt(index);
		element.dispatchEvent(event);
		index++;
		}
		else {
  			 clearInterval(interval);
  		}
	}, 50);
}

function generateUniqueId() {
	const timestamp = Date.now();
	const randomNumber = Math.random();
	const hexadecimalString = randomNumber.toString(16);
	return `id-${timestamp}-${hexadecimalString}`;
}

function chatContainer(uniqueId, isAi){
	return(`<div class="chat_container">
				<div class="title">Ask GPT anything. Type QUIT to quit.</div>
				<div class="history">
					<div class="prompt_appearance">
						<img class="prompt_img" src="assets/bullet.png" alt="" />
					</div>
				<textarea id=${uniqueId} class="textarea ${isAi ? 'response' : 'prompt'}" rows="1" cols="1" placeholder="()" disabled></textarea>
				</div>
			</div>`);
}

const handleSubmit = async (event, textarea) => {
	event.preventDefault();
	// user's chatStripe
	// chatContainer.innerHTML += chatStripe(false, data.get('prompt'));
	// bot's chatStripe
	const data = new FormData(form)
	const uniqueId = generateUniqueId();
	var mainContainer = document.getElementById("main_container");
	console.log(data)
	var parent = mainContainer.parentNode;
	var element = document.createRange()
			.createContextualFragment(chatContainer(uniqueId, false));
	var sibling = element.querySelector('textarea');
	sibling.value = String(textarea.value);
	sibling.style.height = Math.min(textarea.scrollHeight, limit) + "px";
	form.reset();
	textarea.style.height = "";
	parent.insertBefore(element, mainContainer);
	// chatContainer.innerHTML += chatStripe(true, " ", uniqueId);
	// chatContainer.scrollTop = chatContainer.scrollHeight;

	// const messageDiv = document.getElementById(uniqueId);
	// typeText(messageDiv, value);
	// loader(messageDiv);
	//
	console.log(data.get('prompt'));
	const response = await fetch('http://localhost:5000', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({
			prompt: data.get('prompt')
		})
	})
	//
	// clearInterval(loadInterval);
	// messageDiv.innerHTML = '';
	//
	if(response.ok){
		const data = await response.json();
		const parsedData = data.bot.trim();
		var element = document.createRange()
			.createContextualFragment(chatContainer(uniqueId, true));
		var resarea = element.querySelector('textarea');
		resarea.addEventListener('input', () => {
			resarea.style.height = "";
			resarea.style.height = Math.min(resarea.scrollHeight, limit) + "px";
		});
		typeText(resarea, parsedData);
		parent.insertBefore(element, mainContainer);
	}
	else {
		const err = await response.text();
		// messageDiv.innerHTML = "Something went wrong";
		console.log(err);
		alert(err);
	}
}

// form.addEventListener('submit', handleSubmit);
// form.addEventListener('keyup', (e) => {
// 	if(e.keyCode === 13){
// 		handleSubmit(e);
// 	}
// })

function formatTextArea(textArea){
	textArea.value = textArea.value.replace(/(^|\r\n|\n)([^*]|$)/g, "$1*$2");
}

let textarea = document.getElementById('main_textarea');
textarea.addEventListener('input', () => {
	textarea.style.height = "";
	textarea.style.height = Math.min(textarea.scrollHeight, limit) + "px";
});
form.addEventListener('keyup', (event) => {
	if(event.keyCode === 13){
		handleSubmit(event, textarea);
	}
});
form.addEventListener('submit', handleSubmit);


