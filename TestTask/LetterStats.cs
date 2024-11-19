using System;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats : IComparable<LetterStats>
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        private readonly string _letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        private int _count;

        public LetterStats(string letter)
        {
            _letter = letter;
            _count = 1;
        }

        public void Inc()
        {
            _count++;
        }

        public bool AllLettersHasType(CharType charType)
        {
            return _letter.ToCharArray().All(c => GetLetterType(c) == charType);
            
            CharType GetLetterType(char letter)
            {
                const string vowels = "aeiou";
                return vowels.Contains(char.ToLower(letter)) ? CharType.Vowel : CharType.Consonants;
            }
        }

        public int CompareTo(LetterStats other)
        {
            return string.Compare(_letter, other._letter, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"{_letter}: {_count}";
        }
    }
}
