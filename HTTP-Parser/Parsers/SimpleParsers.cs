using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using static Pidgin.Parser;

namespace HTTP_Parser.Parsers
{
    public static class SimpleParsers
    {
        public static readonly char[] Delimiters = { '"', '(', ')', ',', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '{', '}' };
        public static readonly char[] TCharSpecialSymbols = { '!', '#', '$', '%', '&', '\'', '*', '+', '-', '.', '^', '_', '`', '|', '~' };
        public static readonly char[] UriSubcomponentDelimiters = { '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=' };
        public static readonly char[] VCharComplement = { '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07', '\x08', '\x09', '\x0a', '\x0b', '\x0c',
                                                           '\x0d', '\x0e', '\x0f', '\x10', '\x11' ,'\x12', '\x13', '\x14', '\x15' ,'\x16', '\x17', '\x18', '\x19',
                                                           '\x1a', '\x1b', '\x1c', '\x1d', '\x1e', '\x1f', '\x20', '\x7f'};
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
        public static readonly Parser<char, char> Delimiter = OneOf(Delimiters).Labelled("Delimiter");

        public static readonly Parser<char, char> TChar =
            OneOf(new List<Parser<char, char>>() {OneOf(TCharSpecialSymbols), LetterOrDigit});

        public static readonly Parser<char, char> UriSubDelims = OneOf(UriSubcomponentDelimiters);
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
            OneOf(new List<Parser<char, char>>() {LetterOrDigit, Dash, Dot, Underscore, Tilde});
        public static readonly Parser<char, char> Percent = Char('%');
        public static readonly Parser<char, char> HexDigit = OneOf(HexDigits);
        public static readonly Parser<char, char> PercentEncoding =
            from percent in Percent
            from number in HexDigit.Repeat(2).Select(string.Concat)
            select Convert.ToChar(Convert.ToUInt32(number, 16));
        
    }
}
