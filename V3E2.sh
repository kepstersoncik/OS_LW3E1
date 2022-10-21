#!/bin/bash
printf "%s\n" "Список содержащий имена пользователей и их домашние каталоги:" 
cat /etc/passwd | awk -F ":" '{printf "%s%25s\n", $1, $6}' | column -t
printf "%s %d\n" "Количество пользователей в директории home:" $(cat /etc/passwd | grep "/home" | wc -l)