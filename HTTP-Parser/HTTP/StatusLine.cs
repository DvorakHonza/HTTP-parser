using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class StatusLine : IStartLine
    {
        public float Version { get; }
        public int StatusCode { get; }
        public string ReasonPhrase { get; }

        public StatusLine(float Version, int StatusCode, string ReasonPhrase)
        {
            this.Version = Version;
            this.StatusCode = StatusCode;
            this.ReasonPhrase = ReasonPhrase;
        }
    }
}
