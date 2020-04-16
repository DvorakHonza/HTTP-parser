﻿using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using static Pidgin.Parser;
using HTTP_Parser.HTTP;

namespace HTTP_Parser.Parsers
{
    public static class StatusLineParser
    {
        private static readonly Parser<char, double> Version = 
            SimpleParsers.Http.Then(SimpleParsers.Slash).Then(Real).Labelled("Version");

        //TODO add obs-text
        private static readonly Parser<char, string> ReasonPhrase = 
            LetterOrDigit.Or(SimpleParsers.Space).Or(SimpleParsers.HTab).ManyString().Labelled("ReasonPhrase");

        public static readonly Parser<char, StartLine> StatusLine =
            Map((version, statusCode, reasonPhrase) => new StatusLine(version, statusCode, reasonPhrase) as StartLine,
                Version.Before(SimpleParsers.Space),
                DecimalNum.Before(SimpleParsers.Space),
                ReasonPhrase.Before(SimpleParsers.CRLF)
                ).Labelled("StatusLineParser");
    }
}