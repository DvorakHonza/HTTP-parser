    using HTTP_Parser.HTTP;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq;
using System.Collections.Generic;
    using System.Collections.Immutable;

    namespace HTTP_Parser.Parsers
{
    public static class HttpParser
    {
        private static readonly Parser<char, StartLine> StartLineParser =
            Try(StatusLineParser.StatusLine).Or(RequestLineParser.RequestLine);
       
        private static readonly Parser<char, HttpHeader> HttpHeaderParser =
            Map((startLine, headerFields) => new HttpHeader(startLine, headerFields), 
                StartLineParser, 
                HeaderFieldsParser.HeaderFields);

        private static readonly Parser<char, HttpMessage> HttpMessageParser =
            from header in HttpHeaderParser.Before(SimpleParsers.Crlf)
            from body in HttpGetBodyParser(header)
            select body.HasValue ? new HttpMessage(header, body.Value) : new HttpMessage(header);

        private static Parser<char, Maybe<byte[]>> HttpGetBodyParser(HttpHeader header)
        {
            if (header.HeaderFields.GetValueOrDefault("Transfer-Encoding", "") == "chunked")
            {
                return Any
                    .Select(res => (byte)res)
                    .Many()
                    .Select(res => res.ToArray())
                    .Before(End)
                    .Optional();
            }
            return Try(Any
                        .Select(res => (byte) res)
                        .Repeat(header.GetMessageBodyLength())
                        .Select(res => res.ToArray()))
                    .Optional();
        }

        public static Result<char, IEnumerable<HttpMessage>> Parse(string input) => HttpMessageParser.Many().Parse(input);
    }
}
