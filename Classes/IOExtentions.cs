using System;
using System.IO;
using System.Text;

namespace TextProcessor.Classes
{
    /// <summary>
    /// Содержит расширенные методы для работы вводом-выводом
    /// </summary>
    static class IOExtentions
    {
        // Разделители слов, используются для String.Split
        static readonly char[] wordsSplitters = new char[] { ' ', '\r', '\n', '.', ',', '"' };


        /// <summary>
        /// Считывает слова из UTF8 файла в массив строк
        /// </summary>
        /// <param name="textFilePath">Путь к текстовуму файлу для чтения</param>
        /// <returns>Возвращает массив слов в lowercase, по порядку встречи в файле</returns>
        internal static string[] GetWordsFromUtf8File(string textFilePath)
        {
            if (IsFileExists(textFilePath))
            {               
                using (FileStream fs = File.OpenRead(textFilePath))
                {
                    byte[] readBytesArr = new byte[fs.Length];

                    fs.Read(readBytesArr, 0, readBytesArr.Length);

                    return Encoding.UTF8.GetString(readBytesArr)
                        .ToLowerInvariant()
                        .Split(wordsSplitters, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            else
            {
                throw new ArgumentException("Не удалось прочитать файл");
            }
        }


        /// <summary>
        /// Проверяет существует ли файл по указанному пути
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns>True - существует, False - не существует</returns>
        internal static bool IsFileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "Путь до файла не может быть пустым или null.");
            }
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                throw new ArgumentException($"Файла с расположением {path} не существует.");
            }
        }


        /// <summary>
        /// Метод позволяющий отслеживать нажатие ESC во время ввода ReadLine()
        /// </summary>
        /// <returns>NULL - если была нажата клавиша ESC, <br/> Строку как результат чтения ввода - если была нажата клавиша Enter</returns>
        internal static string ReadLineOrEsc()
        {
            string retString = String.Empty;

            int curIndex = 0;
            do
            {
                ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                // Обработка нажатия ESC
                if (readKeyResult.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return null;
                }

                // Обработка нажатия Enter
                if (readKeyResult.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return retString;
                }

                // Обработка нажатия Backspace
                if (readKeyResult.Key == ConsoleKey.Backspace)
                {
                    if (curIndex > 0)
                    {
                        retString = retString.Remove(retString.Length - 1);
                        Console.Write(readKeyResult.KeyChar);
                        Console.Write(' ');
                        Console.Write(readKeyResult.KeyChar);
                        curIndex--;
                    }
                }
                else
                // Обработка нажатия остальных клавиш
                {
                    retString += readKeyResult.KeyChar;
                    Console.Write(readKeyResult.KeyChar);
                    curIndex++;
                }
            }
            while (true);
        }
    }
}
