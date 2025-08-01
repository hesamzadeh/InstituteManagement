using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstituteManagement.Application.Common
{
    public static class NationalIdValidator
    {
        public static bool IsValidIranianCodeMeli(string codeMeli)
        {
            if (string.IsNullOrWhiteSpace(codeMeli))
                return false;

            // Must be 10 digits, all numeric
            if (!System.Text.RegularExpressions.Regex.IsMatch(codeMeli, @"^\d{10}$"))
                return false;

            // Reject all digits being the same (e.g., 1111111111, 0000000000)
            var allSame = codeMeli.All(c => c == codeMeli[0]);
            if (allSame)
                return false;

            // Checksum calculation
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (codeMeli[i] - '0') * (10 - i);
            }

            int remainder = sum % 11;
            int checkDigit = codeMeli[9] - '0';

            return (remainder < 2 && checkDigit == remainder) || (remainder >= 2 && checkDigit == (11 - remainder));
        }
    }
}
