using HTTP_Parser.HTTP.RequestTargets;
using Pidgin;
using static Pidgin.Parser;

namespace HTTP_Parser.Parsers.RequestTargetParsers
{
    public static class RequestTargetParser
    {
        public static readonly Parser<char, RequestTarget> AuthorityForm =
            Map(AuthorityFormParser.CreateAuthorityForm,
                Try(AuthorityFormParser.UserInfo.Before(SimpleParsers.AtSign)).Optional(),
                AuthorityFormParser.Host,
                SimpleParsers.Colon.Then(DecimalNum).Optional()
                );

        public static readonly Parser<char, RequestTarget> AbsoluteForm =
           Map(AbsoluteFormParser.FormatAbsoluteUriOutput,
               UriComponents.Scheme.Before(SimpleParsers.Colon),
               AbsoluteFormParser.HierPart,
               SimpleParsers.QuestionMark.Then(UriComponents.Queries).Optional(),
               UriComponents.Fragment.Optional()
               );

        public static readonly Parser<char, RequestTarget> AsteriskForm =
           SimpleParsers.Asterisk
               .Select(res => new AsteriskForm() as RequestTarget)
           .Labelled("Asterisk form");

        public static readonly Parser<char, RequestTarget> OriginForm =
            from absolutePath in OriginFormParser.AbsolutePath.Labelled("OriginForm absolute path")
            from queries in SimpleParsers.QuestionMark.Then(UriComponents.Queries).Optional().Labelled("Optional query")
            select queries.HasValue ?
                new OriginForm(absolutePath, queries.Value) as RequestTarget :
                new OriginForm(absolutePath) as RequestTarget;

    }
}
