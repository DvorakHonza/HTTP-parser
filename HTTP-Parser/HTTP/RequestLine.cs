using HTTP_Parser.HTTP.RequestTargets;

namespace HTTP_Parser.HTTP
{
    public class RequestLine : StartLine
    {
        public string Method { get; }
        public RequestTarget RequestTarget { get; }
        public double Version { get; }

        public RequestLine( string method, RequestTarget requestTarget, double version)
        {
            Method = method;
            RequestTarget = requestTarget;
            Version = version;
            Type = MessageType.Request;
        }

        public override string ToString() => $"{Method} {RequestTarget} HTTP/{Version}";
    }
}
