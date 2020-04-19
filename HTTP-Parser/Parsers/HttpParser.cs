using HTTP_Parser.HTTP;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq;
using System.Collections.Generic;

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
            from body in Try(
                Any
                    .Select(res => (byte)res)
                    .Repeat(header.GetMessageBodyLength())
                    .Select(res => res.ToArray()))
                .Optional()
            select body.HasValue ? new HttpMessage(header, body.Value) : new HttpMessage(header);

        public static Result<char, IEnumerable<HttpMessage>> Parse(string input) => HttpMessageParser.Many().Parse(input);
    }
}
