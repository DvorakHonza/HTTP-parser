using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class StatusLine : StartLine
    {
        public double Version { get; }
        public int StatusCode { get; }
        public string ReasonPhrase { get; }

        public StatusLine(double Version, int StatusCode, string ReasonPhrase)
        {
            this.Version = Version;
            this.StatusCode = StatusCode;
            this.ReasonPhrase = ReasonPhrase;
        }

        public override string ToString()
        {
            return $"HTTP/{Version} {StatusCode} {ReasonPhrase}";
        }
    }
}
