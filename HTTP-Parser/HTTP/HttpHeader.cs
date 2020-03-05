using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class HttpHeader
    {
        public StartLine StartLine { get; }
        public ImmutableDictionary<string, string> HeaderFields { get; }

        public HttpHeader(StartLine StartLine, ImmutableDictionary<string, string> HeaderFields)
        {
            this.StartLine = StartLine;
            this.HeaderFields = HeaderFields;
        }

        public override string ToString()
        {
            return $"{StartLine.ToString()}\r\n{string.Join("\r\n", HeaderFields.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
        }

        //TODO add full body check
        public bool HasBody()
        {
            switch(StartLine)
            {
                case RequestLine _:
                    if (HeaderFields.ContainsKey("Content-Length") || HeaderFields.ContainsKey("Transfer-Encoding"))
                        return true;
                    return false;
                case StatusLine statusLine:
                    if (statusLine.StatusCode < 199 || statusLine.StatusCode == 204 || statusLine.StatusCode == 304)
                        return false;
                    return false;
                default:
                    return false;
            }
        }
        public int GetMessageBodyLength()
        {
            if (HasBody())
                return int.Parse(HeaderFields.GetValueOrDefault("Content-Length"));
            return 0;
        }

    }
}
