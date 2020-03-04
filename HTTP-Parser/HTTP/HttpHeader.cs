using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class HttpHeader
    {
        public IStartLine StartLine { get; }
        public ImmutableArray<HeaderField> HeaderFields { get; }
        public ImmutableDictionary<string, string> HeaderFields1 { get; }

        public HttpHeader(IStartLine StartLine, ImmutableArray<HeaderField> HeaderFields)
        {
            this.StartLine = StartLine;
            this.HeaderFields = HeaderFields;
        }

        public override string ToString()
        {
            return $"{StartLine.ToString()}\r\n{string.Join("\r\n", HeaderFields.Select(item => $"{item.ToString()}"))}";
        }
    }
}
