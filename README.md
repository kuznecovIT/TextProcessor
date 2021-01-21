# TextProcessor

## Варианты использования
1. Запуск TextProcessor.exe без параметров
В таком случае программа запускается в режиме автодополнения слов, в дальнейшем необходимо будет вводить слова которые после программа автоматически дополнит
2. Запуск TextProcessor.exe с параметрами

Варианты доступных параметров: 

CREATE - создать словарь с данными из текстового файла  
ex: TextProcessor.exe CREATE C:/01/txt.txt<br/>

UPDATE - обновить словарь с данными из текстового файла  
ex: TextProcessor.exe UPDATE C:/01/txt.txt<br/>

CLEANUP - удалить данные из словаря  
ex: TextProcessor.exe CLEANUP


Для тестов можно использовать файл text.txt
