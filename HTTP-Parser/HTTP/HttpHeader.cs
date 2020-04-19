using System.Collections.Immutable;
using System.Linq;

namespace HTTP_Parser.HTTP
{
    public class HttpHeader
    {
        public StartLine StartLine { get; }
        public ImmutableDictionary<string, string> HeaderFields { get; }

        public HttpHeader(StartLine startLine, ImmutableDictionary<string, string> headerFields)
        {
            HeaderFields = headerFields;
            if (startLine.Type == MessageType.Request)
            {
                StartLine = startLine as RequestLine;
            }
            else
            {
                StartLine = startLine as StatusLine;
            }
        }

        public override string ToString()
        {
            return $"{StartLine}\r\n{string.Join("\r\n", HeaderFields.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
        }

        public bool HasBody()
        {
            switch (StartLine)
            {
                case RequestLine _:
                    return HeaderFields.ContainsKey("Content-Length") || HeaderFields.ContainsKey("Transfer-Encoding");
                case StatusLine statusLine:
                    return statusLine.StatusCode >= 199 && statusLine.StatusCode != 204 && statusLine.StatusCode != 304;
                default:
                    return false;
            }
        }
        public int GetMessageBodyLength()
        {
            if (HasBody())
                return int.Parse(HeaderFields.GetValueOrDefault("Content-Length", "0"));
            return 0;
        }
    }
}
