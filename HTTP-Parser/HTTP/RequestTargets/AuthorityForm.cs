using Pidgin;

namespace HTTP_Parser.HTTP.RequestTargets
{
    public class AuthorityForm : RequestTarget
    {
        public string UserInfo { get; }
        public string Host { get; }
        public int? Port { get; }

        public AuthorityForm(string userInfo, string host, int port)
        {
            UserInfo = userInfo;
            Host = host;
            Port = port;
        }

        public AuthorityForm(string userInfo, string host)
        {
            UserInfo = userInfo;
            Host = host;
            Port = null;
        }

        public override string ToString()
        {
            if (Port == null)
            {
                return $"{UserInfo}@{Host}";
            }
            return $"{UserInfo}@{Host}:{Port}";
        }

    }
}
