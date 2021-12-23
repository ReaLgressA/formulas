using System.Collections.Generic;

namespace Formulas {
    public static class CharCodes {
        public const int ZERO = '0';
        public const int NINE = '9';
        public const int DOT = '.';
        public const int COMMA = ',';
        public const int SPACE = ' ';
        public const int MINUS = '-';
        public const int PARENTHESIS_OPEN = '(';
        public const int PARENTHESIS_CLOSE = ')';

        public static readonly HashSet<char> mapAllowedSymbols = 
            new HashSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_");
    }
}