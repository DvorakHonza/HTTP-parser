using HTTP_Parser.HTTP;
using Pidgin;
using System;
using System.Collections.Generic;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace HTTP_Parser
{
    public static class HttpParser
    {
        private static readonly char[] Delimiters = { '"', '(', ')', ',', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '{', '}' };
        private static readonly char[] tCharSpecialSymbols = { '!', '#', '$', '%', '&', '\'', '*', '+', '-', '.', '^', '_', '`', '|', '~' };

        private static readonly Parser<char, char> Slash = Char('/');
        private static readonly Parser<char, char> Dot = Char('.');
        private static readonly Parser<char, char> Space = Char(' ');
        private static readonly Parser<char, char> HTab = Char('\t');
        private static readonly Parser<char, char> QMark = Char('?');
        private static readonly Parser<char, char> EqualsSign = Char('=');
        private static readonly Parser<char, string> HTTP = String("HTTP");
        private static readonly Parser<char, string> Asterisk = String("*");
        private static readonly Parser<char, string> CRLF = String("\r\n");
        private static readonly Parser<char, char> Delimiter = OneOf(Delimiters);
        private static readonly Parser<char, char> tChar = OneOf(new List<Parser<char, char>>() { OneOf(tCharSpecialSymbols), LetterOrDigit });


        private static readonly Parser<char, double> Version = HTTP.Then(Slash).Then(Real);
        private static readonly Parser<char, int> StatusCode = Int(10);
        private static readonly Parser<char, string> ReasonPhrase =
            Token(char.IsLetterOrDigit).Or(Space).Or(HTab).ManyString();
        private static readonly Parser<char, StatusLine> StatusLineParser =
           from version in Version
           from statusCode in StatusCode.Between(SkipWhitespaces)
           from reasonPhrase in ReasonPhrase.Between(SkipWhitespaces)
           from crlf in CRLF
           select new StatusLine(version, statusCode, reasonPhrase);


        private static readonly Parser<char, string> Method =
            from begin in Letter
            from rest in Letter.ManyString()
            select begin + rest;

        private static readonly Parser<char, string> AbsolutePath =
            from leadingSlash in Slash
            from absolutePath in LetterOrDigit.ManyString()
            select leadingSlash.ToString() + absolutePath;

        private static readonly Parser<char, string> Query =
            from qMark in QMark
            from nameBegin in LetterOrDigit
            from nameRest in LetterOrDigit.ManyString()
            from equalsSign in EqualsSign
            from valueBegin in LetterOrDigit
            from valueRest in LetterOrDigit.ManyString()
            select qMark.ToString() + nameBegin + nameRest + equalsSign + valueBegin + valueRest;

        private static readonly Parser<char, string> OriginForm =
            from absolutePath in AbsolutePath
            from query in Query.Optional()//TODO determine whether this is desired behavior
            select absolutePath + query;

        private static readonly Parser<char, string> AsteriskForm = Asterisk.Between(SkipWhitespaces);
        private static readonly Parser<char, string> RequestTarget = OriginForm; //TODO add rest of the forms

        private static readonly Parser<char, RequestLine> RequestLineParser =
            from method in Method
            from requestTarget in RequestTarget.Between(SkipWhitespaces)
            from version in Version
            from crlf in CRLF
            select new RequestLine(method, requestTarget, version);

        public static Result<char, RequestLine> Parse(string input) => RequestLineParser.Parse(input);
        //public static Result<char, HttpMessage> Parse(string input) => Message.Parse(input);
        
    }
}
