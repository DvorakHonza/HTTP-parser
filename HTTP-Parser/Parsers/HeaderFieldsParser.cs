using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Pidgin;
using static Pidgin.Parser;

namespace HTTP_Parser.Parsers
{
    public static class HeaderFieldsParser
    {
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

        public static readonly Parser<char, ImmutableDictionary<string, string>> HeaderFields =
            HeaderFieldParser.Many().Select(kvps => kvps.ToImmutableDictionary());
    }
}
