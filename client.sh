#!/bin/bash
SESSION=br;

# while getopts "s" flag;
# do
# 	case "${flag}" in
# 		s) session=${OPTARG};;
# 		*) 
# 			printf "Use [-s] for session. Available options: \n<br> (browser)\n<cl> (command-line)\n"
# 	esac
# done

while test $# -gt 0; do
	[ "$1" == "br" ] && SESSION="br"
	[ "$1" == "cl" ] && SESSION="cl"
	shift
done

if [ $SESSION == "cl" ]; then
	cd ./cli && node .
elif [ $SESSION == "br" ]; then
	cd ./browser && npm run dev
fi
