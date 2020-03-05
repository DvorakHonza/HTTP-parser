using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP.RequestTargets
{
    public class AuthorityForm : RequestTarget
    {
        public string Host { get; }
        public int Port { get; }

        public AuthorityForm(string Host, int Port)
        {
            this.Host = Host;
            this.Port = Port;
        }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }

    }
}
