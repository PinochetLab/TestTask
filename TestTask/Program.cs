using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public static class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        private static void Main(string[] args)
        {
            var inputStream1 = GetInputStream(args[0]);
            var inputStream2 = GetInputStream(args[1]);

            var singleLetterStats = FillSingleLetterStats(inputStream1);
            var doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            inputStream1.Close();
            inputStream2.Close();
            
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            return FillLetterStats(stream, 1, true);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            return FillLetterStats(stream, 2, false);
        }
        
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения повторяющихся букв.
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <param name="letterCount">Количество повторяющихся букв</param>
        /// <param name="useRegister">Учитывается ли регистр</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillLetterStats(IReadOnlyStream stream, int letterCount, bool useRegister)
        {
            stream.ResetPositionToStart();
            
            var letterStats = new Dictionary<string, LetterStats>();

            var letters = string.Empty;
            
            while (!stream.IsEof)
            {
                
                var c = stream.ReadNextChar();
                if (!useRegister)
                {
                    c = char.ToLower(c);
                }
                
                if (!char.IsLetter(c))
                {
                    letters = string.Empty;
                    continue;
                }

                if (letters.Length > 0 && letters[letters.Length - 1] != c)
                {
                    letters = string.Empty;
                }

                letters += c;
                if (letters.Length > letterCount)
                {
                    letters = letters.Substring(letters.Length - letterCount, letterCount);
                }
                
                if (letters.Length != letterCount)
                {
                    continue;
                }

                if (letterStats.TryGetValue(letters, out var stats))
                {
                    IncStatistic(stats);
                }
                else
                {
                    letterStats.Add(letters, new LetterStats(letters));
                }
            }

            return letterStats.Values.ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            for (var i = letters.Count - 1; i >= 0; i--)
            {
                if (letters[i].AllLettersHasType(charType))
                {
                    letters.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            Console.Write("[");
            
            var letterStatsSorted = letters.ToList();
            letterStatsSorted.Sort();
            
            for (var i = 0; i < letterStatsSorted.Count; i++)
            {
                var stats = letterStatsSorted[i];
                Console.Write(stats);
                if (i < letterStatsSorted.Count - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.Write("]");
            Console.WriteLine();
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Inc();
        }


    }
}
