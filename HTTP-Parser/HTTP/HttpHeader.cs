using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class HttpHeader
    {
        public IStartLine StartLine { get; }
        public ImmutableArray<HeaderField> HeaderFields { get; }

        public HttpHeader(IStartLine StartLine, ImmutableArray<HeaderField> HeaderFields)
        {
            this.StartLine = StartLine;
            this.HeaderFields = HeaderFields;
        }
    }
}
