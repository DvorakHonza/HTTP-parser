using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class RequestLine : IStartLine
    {
        public string Method { get; }
        public string RequestTarget { get; }
        public double Version { get; }

        public RequestLine( string Method, string RequestTarget, double Version)
        {
            this.Method = Method;
            this.RequestTarget = RequestTarget;
            this.Version = Version;

        }

        public override string ToString() => $"{Method} {RequestTarget} HTTP/{Version}";
    }
}
