using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.DataModel;

namespace TextProcessor.Classes
{
    /// <summary>
    /// Содержит методы для автодополнения слов используя данные таблицы базы данных
    /// </summary>
    static class WordsAutoCompletion
    {
        /// <summary>
        /// Запуск процесса автодополнения слов
        /// </summary>
        /// <param name="maxPredictions">Количество выводимых автодополнений к слову</param>
        internal static void RunAutoCompletion(int maxPredictions)
        {
            try
            {
                Console.WriteLine("Программа запущена в режиме автодополнения слов");

                // Создаем контекст для работы с базой данных
                using (TextProcessorContext database = new TextProcessorContext())
                {
                    while (true)
                    {
                        var inputRead = IOExtentions.ReadLineOrEsc();

                        // Прерываем ввод слов если нажата клавиша ESC или введена пустая строка
                        if (inputRead == null || inputRead == string.Empty) break;

                        var predictedWords = AutoCompleteWordFromFrequentWordsTable(inputRead, database, maxPredictions);

                        PrintAutoCompletedWords(predictedWords);

                        Console.WriteLine("\nДля выхода нажмите ESC или введите пустую строку");
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception($"При попытке автодополнения слова произошла ошибка: {e.Message}");
            }          
        }

        /// <summary>
        /// Выполняет автодополнение слова используя данные из таблицы FrequentWords базы данных
        /// </summary>
        /// <param name="inputWordToCompletion">Слово для автодополнения</param>
        /// <param name="sourceDb">Контекст базы данных</param>
        /// <param name="completionLimit">Максимум слов для вывода</param>
        /// <returns>Список подходящих для автодополнения слов</returns>
        internal static List<FrequentWord> AutoCompleteWordFromFrequentWordsTable(string inputWordToCompletion, TextProcessorContext database, int completionLimit)
        {
            try
            {
                return database.FrequentWords.
                    AsNoTracking().
                    Where(x => x.Word.StartsWith(inputWordToCompletion)).
                    OrderByDescending(x => x.Frequent).
                    ThenBy(x => x.Word).
                    Take(completionLimit).
                    ToList();
            }
            catch(Exception e)
            {
                throw new Exception($"Не удалось получить данные для автодополнения из базы данных. Ошибка: {e.Message}");
            }
            
        }

        /// <summary>
        /// Вывести автодополненные слова
        /// </summary>
        /// <param name="completionWords">Список автодополненных слов</param>
        internal static void PrintAutoCompletedWords(List<FrequentWord> completionWords)
        {
            foreach(var data in completionWords)
            {
                Console.WriteLine(data.Word);
            }
        }
    }
}
