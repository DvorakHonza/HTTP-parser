using HTTP_Parser;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ParserTests
{
    public class GetRequestParserTest
    {
        private readonly ITestOutputHelper output;
        public GetRequestParserTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HttpRequest_Test()
        {
            string myString;
            using (FileStream fs = new FileStream("Resources/http_request.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                myString = Convert.ToBase64String(bin);
            }

            byte[] rebin = Convert.FromBase64String(myString);
            string converted = Encoding.UTF8.GetString(rebin, 0, rebin.Length);
            var res = HttpParser.Parse(converted);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
            
        }

        [Fact]
        public void HttpResponse_Test()
        {
            string myString;
            using (FileStream fs = new FileStream("Resources/http_response.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                myString = Convert.ToBase64String(bin);
            }

            byte[] rebin = Convert.FromBase64String(myString);
            string converted = Encoding.UTF8.GetString(rebin, 0, rebin.Length);
            var res = HttpParser.Parse(converted);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }

    }
}
