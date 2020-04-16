using Pidgin;
using static Pidgin.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using HTTP_Parser.HTTP.RequestTargets;
using HTTP_Parser.HTTP;
using System.Collections.Immutable;

namespace HTTP_Parser.Parsers
{
    class RequestLineParser
    {
        private static readonly Parser<char, string> Scheme = String("http");

        private static readonly Parser<char, double> Version =
            SimpleParsers.Http.Then(SimpleParsers.Slash).Then(Real).Labelled("Version");

        private static readonly Parser<char, string> UserInfo =
            OneOf(new List<Parser<char, char>>() {
                SimpleParsers.Unreserved,
                SimpleParsers.PercentEncoding,
                SimpleParsers.UriSubDelims,
                SimpleParsers.Colon }).ManyString().Labelled("User info");

        private static readonly Parser<char, string> RegistredName =
            SimpleParsers.Unreserved
            .Or(SimpleParsers.PercentEncoding)
            .Or(SimpleParsers.UriSubDelims)
            .ManyString()
            .Labelled("RegistredName");

        private static readonly Parser<char, string> IpLiteral =
            IPvFutureParser.IpVFuture.Or(IPv6Parser.Address)
            .Between(SimpleParsers.LeftBracket, SimpleParsers.RightBracket)
            .Labelled("IpLiteral");

        private static readonly Parser<char, string> Host =
            IpLiteral
            .Or(IPv4Parser.Address)
            .Or(RegistredName)
            .Labelled("Host");

        private static readonly Parser<char, RequestTarget> Authority =
            Map((userInfo, host, port) => new AuthorityForm(userInfo + "@" + host, port) as RequestTarget,
                UserInfo.Before(SimpleParsers.AtSign),
                Host.Before(SimpleParsers.Colon),
                DecimalNum
                );

        private static readonly Parser<char, char> PChar =
            OneOf(new List<Parser<char, char>>() {
                SimpleParsers.Unreserved,
                SimpleParsers.PercentEncoding,
                SimpleParsers.UriSubDelims,
                SimpleParsers.Colon,
                SimpleParsers.AtSign }).Labelled("pchar");

        private static readonly Parser<char, string> PathAbempty =
            SimpleParsers.Slash.Then(PChar.ManyString()).ManyString().Select(res => "/" + res.ToString()).Labelled("Path AbEmpty");

        private static readonly Parser<char, string> SegmentNz =
            PChar.AtLeastOnce().Select(res => SimpleParsers.ConvertIEnumerableToString(res)).Labelled("SegmentNz");

        private static readonly Parser<char, string> PathAbsoluteOptional =
            Map((segment, pathAbempty) => segment + pathAbempty, SegmentNz, PathAbempty);

        private static readonly Parser<char, string> PathAbsolute =
            from slash in SimpleParsers.Slash
            from optional in PathAbsoluteOptional.Optional()
            select optional.HasValue ? optional.Value : "/";

        private static readonly Parser<char, string> PathRootless =
            Map((segmentNz, pathAbempty) => segmentNz + pathAbempty, SegmentNz, PathAbempty);

        private static readonly Parser<char, string> PathEmpty = PChar.RepeatString(0).Labelled("Path empty");

        private static readonly Parser<char, string> Path = PathAbempty.Or(PathAbsolute).Or(PathRootless).Or(PathEmpty).Labelled("Path");

        private static readonly Parser<char, string> HierPart =
            Map((authority, path) => authority + path, SimpleParsers.DoubleSlash.Then(Authority), Path).Labelled("HierPart");

        private static readonly Parser<char, string> Fragment =
            SimpleParsers.NumberSign
            .Then(PChar
                .Or(SimpleParsers.Slash)
                .Or(SimpleParsers.QuestionMark)
                .ManyString())
            .Select(res => "#" + res).Labelled("Fragment");

        private static readonly Parser<char, string> Method = 
            Letter.AtLeastOnce().Select(res => SimpleParsers.ConvertIEnumerableToString(res)).Labelled("Method");

        private static readonly Parser<char, string> Segment = PChar.ManyString();

        private static readonly Parser<char, string> AbsolutePath = 
            SimpleParsers.Slash.Then(Segment).AtLeastOnce().Select(res => PrependChar(res, '/'));

        private static readonly Parser<char, KeyValuePair<string, string>> Query = 
            PChar
            .Or(SimpleParsers.Slash)
            .Or(SimpleParsers.QuestionMark)
            .ManyString()
            .Select(res => QueryToKVPair(res));

        private static readonly Parser<char, ImmutableDictionary<string, string>> Queries =
            Query
            .Separated(Char('&'))
            .Select(kvps => kvps.ToImmutableDictionary())
            .Labelled("Queries");

        private static readonly Parser<char, RequestTarget> OriginForm =
            from absolutePath in AbsolutePath.Labelled("OriginForm absolute path")
            from queries in SimpleParsers.QuestionMark.Then(Queries).Optional().Labelled("Optional query")
            from space in SimpleParsers.Space
            select queries.HasValue ?
                new OriginForm(absolutePath, queries.Value) as RequestTarget :
                new OriginForm(absolutePath) as RequestTarget;

        private static readonly Parser<char, RequestTarget> AbsoluteFormParser =
           Map((scheme, hierPart, queries, fragment) => FormatAbsoluteUriOutput(scheme, hierPart, queries, fragment) as RequestTarget,
               Scheme.Before(SimpleParsers.Colon),
               HierPart,
               SimpleParsers.QuestionMark.Then(Queries).Optional(),
               Fragment.Optional()
               );

        private static readonly Parser<char, RequestTarget> AsteriskFormParser =
            SimpleParsers.Asterisk.Between(SimpleParsers.Space).Select(res => new AsteriskForm() as RequestTarget).Labelled("Asterisk form");

        private static readonly Parser<char, RequestTarget> RequestTargetParser =
            OriginForm.Or(AsteriskFormParser).Or(AbsoluteFormParser).Or(Authority).Labelled("Request target");

        public static readonly Parser<char, StartLine> RequestLine =
            Map((method, requestTarget, version) => new RequestLine(method, requestTarget, version) as StartLine,
                Method.Before(SimpleParsers.Space),
                RequestTargetParser,
                Version.Before(SimpleParsers.CRLF)
                );

        private static KeyValuePair<string, string> QueryToKVPair(string query)
        {
            var parts = query.Split(new[] { '=' }, 2);
            return new KeyValuePair<string, string>(parts[0], parts[1]);
        }

        private static AbsoluteForm FormatAbsoluteUriOutput(string scheme, string hierPart, Maybe<ImmutableDictionary<string, string>> queries, Maybe<string> fragment)
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

        private static string PrependChar(IEnumerable<string> segments, char delimiter)
        {
            var stringBuilder = new StringBuilder();
            foreach (var segment in segments)
            {
                stringBuilder.Append(delimiter);
                stringBuilder.Append(segment);
            }
            return stringBuilder.ToString();
        }
    }
}
