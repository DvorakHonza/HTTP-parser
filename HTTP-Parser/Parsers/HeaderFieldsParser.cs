using System.Collections.Generic;
using System.Collections.Immutable;
using Pidgin;
using static Pidgin.Parser;

namespace HTTP_Parser.Parsers
{
    public static class HeaderFieldsParser
    {
        private static readonly Parser<char, string> FieldName =
           SimpleParsers.TChar
           .AtLeastOnce()
           .Select(string.Concat)
           .Labelled("FieldName");

        private static readonly Parser<char, char> FieldVChar =
            SimpleParsers.VChar.Or(SimpleParsers.ObsText);

        private static readonly Parser<char, string> FieldContentOptional =
            Map((spaces, vchar) => spaces + vchar,
                SimpleParsers.Space
                .Or(SimpleParsers.HTab)
                .AtLeastOnce()
                .Select(string.Concat),
                FieldVChar
                );

        private static readonly Parser<char, string> FieldContent =
            from begin in FieldVChar.Labelled("FieldContent begin")
            from rest in FieldContentOptional.Optional().Labelled("FieldContent rest")
            select rest.HasValue ? begin + rest.Value : begin.ToString();

        private static readonly Parser<char, string> ObsFold =
            SimpleParsers.Crlf
            .Then(SimpleParsers.Space
                .Or(SimpleParsers.HTab)
                .AtLeastOnce()
                .Select(string.Concat))
            .ManyString();

        private static readonly Parser<char, string> FieldValue = FieldContent.Or(Try(ObsFold)).ManyString().Before(SimpleParsers.Crlf);

        private static readonly Parser<char, KeyValuePair<string, string>> HeaderFieldParser =
            FieldName.Before(SimpleParsers.ColonWhitespace)
                .Then(FieldValue, (key, value) => new KeyValuePair<string, string>(key, value));

        public static readonly Parser<char, ImmutableDictionary<string, string>> HeaderFields =
            HeaderFieldParser.Many().Select(kvps => kvps.ToImmutableDictionary());
    }
}
