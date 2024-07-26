using System;
using System.Linq;

namespace Hotel5Stars.Models.Dto
{

    public static class FiscalCodeGenerator
    {
        private static readonly string[] VOWELS = { "A", "E", "I", "O", "U" };
        private static readonly string[] CONSONANTS = { "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" };
        private static readonly string[] MONTH_CODES = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" }; // Gennaio - Dicembre
        private static readonly string[] GENDER_CODES = { "M", "F" };

        // Metodo principale per generare il codice fiscale
        public static string Generate(string lastName, string firstName, DateTime birthDate, string gender, string cityCode)
        {
            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(cityCode))
            {
                throw new ArgumentException("Nome, cognome e codice del comune sono obbligatori.");
            }

            // Calcola il codice fiscale
            string fiscalCode = GetConsonants(lastName) + GetVowels(lastName) + GetConsonants(firstName) + GetVowels(firstName)
                                + GetBirthDateCode(birthDate) + GetGenderCode(gender) + cityCode;

            // Limita la lunghezza a 16 caratteri e aggiunge 'X' se necessario
            fiscalCode = new string(fiscalCode.Where(char.IsLetterOrDigit).ToArray()).ToUpper();
            return fiscalCode.Length > 16 ? fiscalCode.Substring(0, 16) : fiscalCode.PadRight(16, 'X');
        }

        // Estrae le consonanti da una stringa
        private static string GetConsonants(string input)
        {
            var consonants = new string(input.Where(c => CONSONANTS.Contains(c.ToString().ToUpper())).ToArray());
            return consonants.Length >= 3 ? consonants.Substring(0, 3) : consonants.PadRight(3, 'X');
        }

        // Estrae le vocali da una stringa
        private static string GetVowels(string input)
        {
            var vowels = new string(input.Where(c => VOWELS.Contains(c.ToString().ToUpper())).ToArray());
            return vowels.Length >= 3 ? vowels.Substring(0, 3) : vowels.PadRight(3, 'X');
        }

        // Calcola la parte della data di nascita del codice fiscale
        private static string GetBirthDateCode(DateTime birthDate)
        {
            // Anno (ultimi due numeri)
            string year = birthDate.Year.ToString().Substring(2, 2);
            // Mese (A = Gennaio, B = Febbraio, etc.)
            string month = MONTH_CODES[birthDate.Month - 1];
            // Giorno (aggiungi 40 se il sesso è femminile)
            string day = (birthDate.Day + (birthDate.Day > 31 ? 40 : 0)).ToString("00");

            return $"{year}{month}{day}";
        }

        // Determina il codice di genere
        private static string GetGenderCode(string gender)
        {
            return gender.ToUpper() == "F" ? "F" : "M";
        }
    }
}