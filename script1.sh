file_name=temp.txt

function printTable() {
    case $1 in
    -a)
        cat -n $file_name
        printf "\n"
    ;;
    -n)
        sed $2'!d' $file_name
    ;;
    -r)
        sed $2,$3'!d' $file_name
    ;;
    -s)
        case $2 in
        surname)
            sort -k 1 $file_name
        ;;
        name)
            sort -k 2 $file_name
        ;;
        birth)
            sort -k 3 $file_name
        ;;
        birthpalace)
            sort -k 4 $file_name
        ;;
        sex)
            sort -k 5 $file_name
        esac
    esac
}

function deleteString(){
    case $1 in
    -a)
        echo -n > $file_name
    ;;
    -n)
        sed -i $2'd' $file_name
        printf "%s\n" "Удалена строка $2"
    ;;
    -nd)
        for i in $(echo $(printf "%d\n" $@ 2> /dev/null | sort -nr ))
        do
            if [[ $i == 0 ]]; then
                :
            else
                printf "%s\n" "Удалена строка $i"
                sed -i $i'd' $file_name
            fi
        done
    ;;
    -r)
        printf "%s\n" "Удалены строки с $2 по $3"
        sed -i $2,$3'd' $file_name
    esac
}

function printHelp(){
    local help_list=("help - для вывода подсказки"
    "print - для вывода таблицы"
    "add - добавить строку"
    "delete - для удаления строки"
    "exit - для выхода из скрипта" "add - добавить запись")
    for i in ${!help_list[*]}
    do
    printf "%s\n" "${help_list[$i]}"
    done 
}

function doExit(){
    exit 0
}

function isTrueString(){
    if [ -z $5 ]
    then
        printf "%s\n" "Недостаточное количество полей!"
        return
    elif ! [ -n $6 ]
    then
        printf "%s\n" "Слишком много полей передано"
        return
    fi

    local DIGIT_RE='^[0-9]+$'

    if [[ $1 =~ $DIGIT_RE ]]; then
        printf "%s\n" "Фамилия не может быть числом!"
    fi

    if [[ $2 =~ $DIGIT_RE ]]; then
        printf "%s\n" "Имя не может быть числом!"
    fi

    if ! [[ $3 =~ $DIGIT_RE ]]; then
        printf "%s\n" "Год должен быть числом!"
    fi

    if [[ $4 =~ $DIGIT_RE ]]; then
        printf "%s\n" "Место рождения не может быть числом!"
    fi

    if ! [[ $5 = "М" || $5 = "Ж" ]]; then
        printf "%s\n" "Неверное значение пола!"
    fi
}

function addNewString(){
    printf "%s\n%s\n%s\n" "Введите данные в формате" "<ФАМИЛИЯ> <ИМЯ> <ГОД РОЖД> <МЕСТО РОЖД> <ПОЛ>" "Без угловых скобок!"
    read string
    printf "%s\n" "Проверка правильности..."
    local checked=$(isTrueString $string)
    if [[ ! -z $checked ]]
    then
        printf "%s\n" "Строка '$string' не прошла проверку!"
        return
    fi
    printf "%s\n" "Првоерка наличия строки в файле..."
    if grep "$string" ./$file_name; then
        printf "%s\n" "Эта запись уже имеется в файле!"
        return
    else
        printf "%s\n" "Запись добавлена!"
        echo $string >> $file_name
    fi
}

flag=1
while [ $flag=1 ]
do
printf "%s" "> "
read choose arg
case $choose in
help)
    printHelp
;;
print)
    printTable $arg $REPLY
;;
exit)
    doExit
;;
add)
    addNewString
;;
delete)
    deleteString $arg $REPLY
esac
done