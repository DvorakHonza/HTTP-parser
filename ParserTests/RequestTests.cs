using HTTP_Parser.Parsers;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ParserTests
{
    public class RequestTests
    {
        private readonly ITestOutputHelper output;
        public RequestTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HttpRequest_Test()
        {
            string input;
            using (FileStream fs = new FileStream("Resources/Requests/http_get.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }

        [Fact]
        public void HttpRequestToOriginServer()
        {
            string input;
            using (FileStream fs = new FileStream("Resources/Requests/http_request_to_origin_server.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }

        [Fact]
        public void HttpRequestToIPv4Address()
        {
            string input;
            using (FileStream fs = new FileStream("Resources/Requests/http_request_to_IPv4_address.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }

        [Fact]
        public void HttpRequestPost()
        {
            string input;
            using (FileStream fs = new FileStream("Resources/Requests/http_request_post.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }

        [Fact]
        public void HttpRequestWithMultipleQueries()
        {
            string input;
            using (FileStream fs = new FileStream("Resources/Requests/http_request_with_multiple_queries.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }
    }
}
