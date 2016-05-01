using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using WebMerge.Client;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Tests
{
    [TestFixture]
    public class WebMergeClientTests
    {
        private IWebMergeClient client;
        private TestingEnabledHttpMessageHandler messageHandler;
        private Mock<IApiConfigurator> config;

        [SetUp]
        public void SetUp()
        {
            messageHandler = new TestingEnabledHttpMessageHandler();
            config = new Mock<IApiConfigurator>();

            config.Setup(x => x.ApiKey).Returns("API_KEY");
            config.Setup(x => x.ApiSecret).Returns("API_SECRET");
            config.Setup(x => x.BaseUri).Returns(new Uri("https://test.io"));

            client = new WebMergeClient(messageHandler, config.Object);
        }

        [Test]
        public async Task MergeDocumentAsyncWithoutDownload()
        {
            var content = new DocumentMergeResponse {Success = true};
            messageHandler.AddResponse(new Uri("https://test.io/merge/123/key"), content);

            var result = await client.MergeDocumentAsync(123, "key", new Dictionary<string, object>(), false);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task MergeDocumentAsyncWithDownload()
        {
            var data = new byte[] {0x2, 0x4};
            messageHandler.AddByteArrayResponse(new Uri("https://test.io/merge/123/key?download=1"), data);

            var result = await client.MergeDocumentAsync(123, "key", new Dictionary<string, object>());

            Assert.NotNull(result);
            Assert.That(result, Is.EquivalentTo(data));
        }
    }
}
