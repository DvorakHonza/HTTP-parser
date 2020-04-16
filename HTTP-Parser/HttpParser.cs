using HTTP_Parser.HTTP;
using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq;
using System;
using System.Text;
using System.Collections.Immutable;
using HTTP_Parser.HTTP.RequestTargets;
using HTTP_Parser.Parsers;

namespace HTTP_Parser
{
    public static class HttpParser
    {

        private static readonly Parser<char, StartLine> StartLineParser = 
            (StatusLineParser.StatusLine).Or(RequestLineParser.RequestLine).Labelled("StartLine");

        //Header fields
        private static readonly Parser<char, string> FieldName = 
            SimpleParsers.TChar
            .AtLeastOnce()
            .Select(res => SimpleParsers.ConvertIEnumerableToString(res))
            .Labelled("FieldName");

        //TODO add obs-text
        private static readonly Parser<char, char> FieldVChar = 
            AnyCharExcept(SimpleParsers.VCharComplement)
            .Labelled("FieldVChar");

        private static readonly Parser<char, string> FieldContentOptional =
            Map((spaces, vchar) => spaces + vchar,
                SimpleParsers.Space
                .Or(SimpleParsers.HTab)
                .AtLeastOnce()
                .Select(res => SimpleParsers.ConvertIEnumerableToString(res)),
                FieldVChar
                );
            
        private static readonly Parser<char, string> FieldContent =
            from begin in FieldVChar.Labelled("FieldContent begin")
            from rest in FieldContentOptional.Optional().Labelled("FieldContent rest")
            select rest.HasValue ? begin + rest.Value : begin.ToString();

        private static readonly Parser<char, Unit> ObsFold =
            SimpleParsers.CRLF.Then(SimpleParsers.Space.Or(SimpleParsers.HTab).AtLeastOnce()).SkipMany();

        private static readonly Parser<char, string> FieldValue = FieldContent.ManyString();//.Or(ObsFold).ManyString();
        private static readonly Parser<char, string> WhitespacesExceptCrLf = Char('\x09').Or(Char('\x0b')).Or(Char('\x0c')).ManyString();

        private static readonly Parser<char, KeyValuePair<string, string>> HeaderFieldParser =
            FieldName.Before(SimpleParsers.ColonWhitespace)
            .Then(FieldValue, (key, value) => new KeyValuePair<string, string>(key, value)).Before(SimpleParsers.CRLF);

        private static readonly Parser<char, ImmutableDictionary<string, string>> HeaderFields =
            HeaderFieldParser.Many().Select(kvps => kvps.ToImmutableDictionary());

        private static readonly Parser<char, HttpHeader> HttpHeaderParser =
            Map((startLine, headerFields) => new HttpHeader(startLine, headerFields), StartLineParser, HeaderFields);


        private static readonly Parser<char, HttpMessage> HttpMessageParser =
            from header in HttpHeaderParser.Before(SimpleParsers.CRLF)
            from body in Any.Select(res => (byte)res).Repeat(header.GetMessageBodyLength()).Select(res => res.ToArray()).Optional()
            select body.HasValue ? new HttpMessage(header, body.Value) : new HttpMessage(header);

        private static readonly Parser<char, char> test = SimpleParsers.PercentEncoding;

        //public static Result<char, char> Parse(string input) => test.Parse(input);
        public static Result<char, HttpMessage> Parse(string input) => HttpMessageParser.Parse(input);

    }
}
