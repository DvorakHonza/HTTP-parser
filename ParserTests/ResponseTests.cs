using HTTP_Parser.Parsers;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ParserTests
{
    public class ResponseTests
    {
        private readonly ITestOutputHelper output;
        public ResponseTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HttpResponseWithoutBody()
        {
            string input;
            using (var fs = new FileStream("Resources/Responses/http_response_without_body.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
        }

        [Fact]
        public void HttpResponseWithTextTest()
        {
            string input;
            using (var fs = new FileStream("Resources/Responses/http_response_with_text.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
            //output.WriteLine(res.Value.MessageBody.Length.ToString());
        }

        [Fact]
        public void HttpResponseWithJpeg()
        {
            string input;
            using (var fs = new FileStream("Resources/Responses/http_response_with_jpeg.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }

            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
            //output.WriteLine(res.Value.MessageBody.Length.ToString());
        }

        [Fact]
        public void HttpResponseWithWebPage()
        {
            string input;
            using (var fs = new FileStream("Resources/Responses/http_response_webPage.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(res.Value.ToString());
            //output.WriteLine(res.Value.MessageBody.Length.ToString());
        }
    }
}
