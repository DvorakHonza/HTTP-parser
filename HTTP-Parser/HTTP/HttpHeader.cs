using System.Collections.Immutable;
using System.Linq;

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
    }
}
