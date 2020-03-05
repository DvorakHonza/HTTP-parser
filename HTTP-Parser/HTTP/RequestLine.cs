using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using HTTP_Parser.HTTP.RequestTargets;

namespace HTTP_Parser.HTTP
{
    public class RequestLine : StartLine
    {
        public string Method { get; }
        public RequestTarget RequestTarget { get; }
        public double Version { get; }

        public RequestLine( string Method, RequestTarget RequestTarget, double Version)
        {
            this.Method = Method;
            this.RequestTarget = RequestTarget;
            this.Version = Version;
        }

        public override string ToString() => $"{Method} {RequestTarget} HTTP/{Version}";
    }
}
