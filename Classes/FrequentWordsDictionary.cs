using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.DataModel;
using TextProcessor.Classes;

namespace TextProcessor
{
    class FrequentWordsDictionary
    {
        readonly TextProcessorContext database;

        public FrequentWordsDictionary()
        {
            database = new TextProcessorContext();
        }

        public FrequentWordsDictionary(string[] command)
        {
            database = new TextProcessorContext();
            GetInputCommand(command);
        }

        ~FrequentWordsDictionary()
        {
            database.Dispose();
        }


        /// <summary>
        /// Обработчик входных команд из командной строки <br/> 
        /// CREATE - создать словарь с данными из текстового файла  ex: CREATE C:/01/txt.txt<br/>
        /// UPDATE - обновить словарь с данными из текстового файла  ex: UPDATE C:/01/txt.txt<br/>
        /// CLEANUP - удалить данные из словаря  ex: DELETE
        /// </summary>
        /// <param name="commands">Команда</param>
        void GetInputCommand(string[] commands)
        {
            try
            {
                switch (commands[0].ToUpper())
                {
                    case "CREATE":
                        CreateDictionaryWithDataFromTextFile(commands[1]);
                        break;
                    case "UPDATE":
                        UpdateDictionaryWithDataFromTextFile(commands[1]);
                        break;
                    case "CLEANUP":
                        CleanupDictionary();
                        break;
                    default:
                        Console.WriteLine("Комманда не поддерживается: " + commands[0].ToString());
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Не достаточно аргументов для отработки команды");
            }
        }


        /// <summary>
        /// Создать словарь - заполнить его часто повторяющимися из текстового файла
        /// </summary>
        /// <param name="textFilePath">Путь к текстовуму файлу</param>
        void CreateDictionaryWithDataFromTextFile(string textFilePath)
        {
            if (IOExtentions.IsFileExists(textFilePath))
            {
                try
                {
                    Console.WriteLine($"Создание словаря и заполнение его данными из файла {textFilePath}");

                    var wordsFromFile = IOExtentions.GetWordsFromUtf8File(textFilePath);

                    var frequentWords = GetFrequentWords(wordsFromFile);

                    foreach (var data in frequentWords)
                    {
                        if (IsWordContainsInFrequentWordsTable(data.Word))
                        {
                            Console.WriteLine($"Слово {data.Word} уже существует в словаре, добавление не было произведено");
                        }
                        else
                        {
                            UpdateDictionaryWithWord(data);
                        }
                    }
                }
                catch(Exception e)
                {
                    throw new DbUpdateException($"При попытке создать словарь произошла ошибка: {e.Message}");
                }              
            }
        }


        /// <summary>
        /// Обновить словарь - дополнение его часто повторяющимися словами из файа
        /// </summary>
        /// <param name="textFilePath">Путь к текстовуму файлу</param>
        void UpdateDictionaryWithDataFromTextFile(string textFilePath)
        {
            if (IOExtentions.IsFileExists(textFilePath))
            {
                try
                {
                    Console.WriteLine($"Обновление словаря данными из файла {textFilePath}");

                    var wordsFromFile = IOExtentions.GetWordsFromUtf8File(textFilePath);

                    var frequentWords = GetFrequentWords(wordsFromFile);

                    foreach (var data in frequentWords)
                    {
                        if (IsWordContainsInFrequentWordsTable(data.Word))
                        {
                            Console.WriteLine($"Слово {data.Word} уже существует в словаре, добавление не было произведено");
                        }
                        else
                        {
                            UpdateDictionaryWithWord(data);
                        }
                    }
                }
                catch(Exception e)
                {
                    throw new DbUpdateException($"При попытке обновить словарь произошла ошибка: {e.Message}");
                }
            }
        }


        /// <summary>
        /// Очистка таблицы базы данных со словарём частых слов
        /// </summary>
        void CleanupDictionary()
        {
            try
            {
                database.Database.ExecuteSqlRaw("TRUNCATE TABLE [FrequentWords]");
                Console.WriteLine("Таблица словаря успешно очищена");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Не удалось очистить таблицу словаря {e.Message}");
            }          
        }


        /// <summary>
        /// Обновить словарь - дополнить словарь входящим элементом FrequentWord
        /// </summary>
        /// <param name="inputFrequentWord">FrequentWord объекта, для добавления в словарь</param>
        void UpdateDictionaryWithWord(FrequentWord inputFrequentWord)
        {
            try
            {            
                database.FrequentWords.Add(inputFrequentWord);
                database.SaveChanges();
                Console.WriteLine($"Слово {inputFrequentWord.Word} было добавлено в базу данных");               
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine($"Слово {inputFrequentWord.Word} не удалось добавить в базу данных");
                throw new DbUpdateException(e.Message);
            }
        }


        /// <summary>
        /// Выбор часто повторяющихся слов из входного массива строк
        /// </summary>
        /// <param name="wordsArr">Массив слов для анализа</param>
        /// <returns>Возвращает список объектов FrequenWord</returns>
        List<FrequentWord> GetFrequentWords(string[] wordsArr)
        {
            try
            {
                // Возвращаем слова по фильтру [ Повторяющиеся больше 2 раз, имеющие длину от 3 до 15 символов ]
                return wordsArr.
                    GroupBy(x => x).
                    Where(x => x.Count() > 2).
                    Where(x => x.Key.Length > 2).
                    Where(x => x.Key.Length < 16).
                    Select(x => new FrequentWord { Word = x.Key, Frequent = x.Count() }).
                    ToList();
            }
            catch(Exception e)
            {
                throw new Exception($"Не удалось выбрать часто повторяющиеся слова из входного массива: {e.Message}");
            }
        }


        /// <summary>
        /// Проверка содержится ли слово в таблице FrequentWords
        /// </summary>
        /// <param name="database">База данных содержащая таблицу FrequentWords</param>
        /// <param name="searchWord">Слово для поиска</param>
        /// <returns>True - слово найдено, False - слово не найдено</returns>
        bool IsWordContainsInFrequentWordsTable(string searchWord)
        {
            try
            {
                if (database.FrequentWords.Any(x => x.Word == searchWord))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw new Exception($"Не удалось установить содержится ли слово в таблице FrequentWords. Ошибка: {e.Message}");
            }         
        }
    }
}
