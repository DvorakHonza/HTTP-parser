using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<byte>;

namespace HTTP_Parser
{
    public static class HttpParser
    {
        private static Parser<byte, T> Shit<T>(Parser<byte, T> token) => Try(token).;
    }
}
