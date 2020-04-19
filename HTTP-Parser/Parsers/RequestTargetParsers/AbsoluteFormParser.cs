using HTTP_Parser.HTTP.RequestTargets;
using Pidgin;
using static Pidgin.Parser;
using System.Collections.Immutable;
using HTTP_Parser.HTTP.Utilities;

namespace HTTP_Parser.Parsers.RequestTargetParsers
{
    public static class AbsoluteFormParser
    {

        private static readonly Parser<char, string> PathAbempty =
            SimpleParsers.Slash
            .Then(UriComponents.PChar.ManyString())
            .ManyString()
            .Select(res => "/" + res.ToString())
            .Labelled("Path AbEmpty");

        private static readonly Parser<char, string> SegmentNz =
            UriComponents.PChar.AtLeastOnce().Select(string.Concat).Labelled("SegmentNz");

        private static readonly Parser<char, string> SegmentNzNc =
            OneOf(
                SimpleParsers.Unreserved,
                SimpleParsers.PercentEncoding,
                SimpleParsers.UriSubDelims,
                SimpleParsers.AtSign
            ).AtLeastOnce()
            .Select(string.Concat);

        private static readonly Parser<char, string> PathAbsoluteOptional =
            Map((segment, pathAbempty) => segment + pathAbempty,
                SegmentNz,
                PathAbempty);

        private static readonly Parser<char, string> PathAbsolute =
            from slash in SimpleParsers.Slash
            from optional in PathAbsoluteOptional.Optional()
            select optional.HasValue ? "/" + optional.Value : "/";

        private static readonly Parser<char, string> PathNoScheme =
            Map((segmentNzNc, segments) => segmentNzNc + segments,
                SegmentNzNc,
                UriComponents.Segment
                    .Many()
                .Select(res => StringFormatting.PrependChar(res, '/')));


        private static readonly Parser<char, string> PathRootless =
            Map((segmentNz, pathAbempty) => segmentNz + pathAbempty, SegmentNz, PathAbempty);

        private static readonly Parser<char, string> PathEmpty = UriComponents.PChar.RepeatString(0).Labelled("Path empty");

        private static readonly Parser<char, string> Path =
            OneOf(
                PathAbempty,
                PathAbsolute,
                PathNoScheme,
                PathRootless,
                PathEmpty
                );

        public static readonly Parser<char, string> HierPart =
            Map((authority, path) => authority + path,
                SimpleParsers.DoubleSlash.Then(RequestTargetParser.AuthorityForm),
                Path);

        public static RequestTarget FormatAbsoluteUriOutput(string scheme, string hierPart, 
            Maybe<ImmutableDictionary<string, string>> queries, Maybe<string> fragment)
        {
            if (queries.HasValue)
            {
                if (fragment.HasValue)
                {
                    return new AbsoluteForm(scheme, hierPart, queries.Value, fragment.Value);
                }
                else
                {
                    return new AbsoluteForm(scheme, hierPart, queries.Value);
                }
            }
            else
            {
                return new AbsoluteForm(scheme, hierPart);
            }
        }
    }
}
