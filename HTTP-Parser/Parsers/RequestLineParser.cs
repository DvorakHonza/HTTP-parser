using Pidgin;
using static Pidgin.Parser;
using HTTP_Parser.HTTP.RequestTargets;
using HTTP_Parser.HTTP;
using HTTP_Parser.Parsers.RequestTargetParsers;

namespace HTTP_Parser.Parsers
{
    public static class RequestLineParser
    {
        private static readonly Parser<char, double> Version =
            SimpleParsers.Http.Then(SimpleParsers.Slash).Then(Real).Labelled("Version");

        private static readonly Parser<char, string> Method = 
            Letter.AtLeastOnce().Select(string.Concat).Labelled("Method");

        private static readonly Parser<char, RequestTarget> RequestTarget =
            RequestTargetParser.OriginForm
            .Or(RequestTargetParser.AsteriskForm)
            .Or(RequestTargetParser.AbsoluteForm)
            .Or(RequestTargetParser.AuthorityForm);

        public static readonly Parser<char, StartLine> RequestLine =
            Map((method, requestTarget, version) => new RequestLine(method, requestTarget, version) as StartLine,
                Method.Before(SimpleParsers.Space),
                RequestTarget.Before(SimpleParsers.Space),
                Version.Before(SimpleParsers.Crlf)
            );

      

      
    }
}
