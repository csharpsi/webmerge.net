using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebMerge.Client;

namespace WebMerge.Tests
{
    [TestFixture]
    public class WebMergeClientTests
    {
        private IWebMergeClient client;
        private HttpMessageHandler messageHandler;
        private Mock<IApiConfigurator> config;

        [SetUp]
        public void SetUp()
        {
            messageHandler = new TestingEnabledHttpMessageHandler();
            config = new Mock<IApiConfigurator>();
            client = new WebMergeClient(messageHandler, config.Object);
        }

        [Test]
        public async Task MergeDocumentAsync()
        {
            
        }
    }
}
