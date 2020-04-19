using System;
using System.Collections.Generic;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace HTTP_Parser.Parsers
{
    public static class SimpleParsers
    {
        public static readonly char[] TCharSpecialSymbols = { '!', '#', '$', '%', '&', '\'', '*', '+', '-', '.', '^', '_', '`', '|', '~' };

        public static readonly char[] UriComponentDelimiters = { '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=' };

        public static readonly char[] HexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static readonly Parser<char, char> Slash = Char('/');

        public static readonly Parser<char, char> Dot = Char('.');

        public static readonly Parser<char, char> Space = Char(' ');

        public static readonly Parser<char, char> HTab = Char('\t');

        public static readonly Parser<char, char> QuestionMark = Char('?');

        public static readonly Parser<char, string> Http = String("HTTP");

        public static readonly Parser<char, string> Asterisk = String("*");

        public static readonly Parser<char, string> Crlf = String("\r\n");

        public static readonly Parser<char, char> AtSign = Char('@');

        public static readonly Parser<char, char> NumberSign = Char('#');

        public static readonly Parser<char, char> TChar =
            OneOf(new List<Parser<char, char>> {OneOf(TCharSpecialSymbols), LetterOrDigit});

        public static readonly Parser<char, char> UriSubDelimiters = OneOf(UriComponentDelimiters);

        public static readonly Parser<char, char> Dash = Char('-');

        public static readonly Parser<char, char> Underscore = Char('_');

        public static readonly Parser<char, char> Tilde = Char('~');

        public static readonly Parser<char, char> Colon = Char(':');

        public static readonly Parser<char, char> ColonWhitespace = Colon.Between(SkipWhitespaces);

        public static readonly Parser<char, string> DoubleColon = String("::");

        public static readonly Parser<char, string> DoubleSlash = String("//");

        public static readonly Parser<char, char> LeftBracket = Char('[');

        public static readonly Parser<char, char> RightBracket = Char(']');

        public static readonly Parser<char, char> Unreserved =
            OneOf(new List<Parser<char, char>> {LetterOrDigit, Dash, Dot, Underscore, Tilde});

        public static readonly Parser<char, char> Percent = Char('%');

        public static readonly Parser<char, char> HexDigit = OneOf(HexDigits);

        public static readonly Parser<char, char> ObsText = Token(IsExtendedAscii);

        public static readonly Parser<char, char> PercentEncoding =
            Map(number => Convert.ToChar(Convert.ToInt32(number, 16)),
                Percent.Then(HexDigit.Repeat(2).Select(string.Concat))
                );

        public static readonly Parser<char, char> VChar = Token(IsVChar);

        private static bool IsExtendedAscii(char c)
        {
            return c >= 0x80 && c <= 0xFF;
        }

        private static bool IsVChar(char c)
        {
            return c > 0x1F && c < 0x7f;
        }
    }
}
