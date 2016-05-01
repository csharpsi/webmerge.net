using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebMerge.Client;
using WebMerge.Client.Enums;
using WebMerge.Client.Factory;
using WebMerge.Client.RequestModels;
using WebMerge.Client.ResponseModels;
using WebMerge.Client.Utils.FileSystem;

namespace WebMerge.Tests
{
    [TestFixture]
    public class WebMergeClientTests
    {
        private const string DefaultResponseDocumentJson =
            @"{
                ""id"":""123"",
                ""key"":""dockey"",
                ""type"":""html"",
                ""name"":""Test"",
                ""output"":""pdf"",
                ""size"":null,
                ""size_width"":null,
                ""size_height"":null,
                ""active"": ""1"",
                ""url"":""https://www.test.io"",
                ""fields"": [ 
                    {""key"": ""456"", ""name"": ""FieldName""}
                ]
            }";

        private IWebMergeClient client;
        private TestingEnabledHttpMessageHandler messageHandler;
        private Mock<IApiConfigurator> config;
        private Mock<IFileHelper> fileHelper;

        [SetUp]
        public void SetUp()
        {
            messageHandler = new TestingEnabledHttpMessageHandler();
            config = new Mock<IApiConfigurator>();
            fileHelper = new Mock<IFileHelper>();

            config.Setup(x => x.ApiKey).Returns("API_KEY");
            config.Setup(x => x.ApiSecret).Returns("API_SECRET");
            config.Setup(x => x.BaseUri).Returns(new Uri("https://test.io"));

            var httpClient = new HttpClient(messageHandler);

            client = new WebMergeClient(httpClient, config.Object, new DocumentCreatorFactory(httpClient, fileHelper.Object));
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

        [Test]
        public async Task MergeDocumentInTestMode()
        {
            var data = new byte[] { 0x2, 0x4 };
            messageHandler.AddByteArrayResponse(new Uri("https://test.io/merge/123/key?download=1&test=1"), data);

            var result = await client.MergeDocumentAsync(123, "key", new Dictionary<string, object>(), testMode: true);

            Assert.NotNull(result);
            Assert.That(result, Is.EquivalentTo(data));
        }

        [Test]
        public async Task MergeDocument()
        {
            var data = new byte[] { 0x2, 0x4 };
            var requestEventFired = false;

            messageHandler.RequestSent += req =>
            {
                var body = req.Content.ReadAsAsync<Dictionary<string, object>>().Result;

                Assert.That(body.ContainsKey("FirstName"), Is.True, "Missing 'FirstName' key from request body");
                Assert.That(body.ContainsKey("LastName"), Is.True, "Missing 'LastName' key from request body");
                Assert.That(body.ContainsKey("Age"), Is.True, "Missing 'Age' key from request body");
                Assert.That(body["FirstName"], Is.EqualTo("Jack"));
                Assert.That(body["LastName"], Is.EqualTo("Daniel"));
                Assert.That(body["Age"], Is.EqualTo(31));

                requestEventFired = true;
            };

            messageHandler.AddByteArrayResponse(new Uri("https://test.io/merge/123/key?download=1"), data);

            var requestBody = new Dictionary<string, object>
            {
                ["FirstName"] = "Jack",
                ["LastName"] = "Daniel",
                ["Age"] = 31
            };

            await client.MergeDocumentAsync(123, "key", requestBody);

            Assert.That(requestEventFired, Is.True);
        }

        [Test]
        public async Task AuthHeaderIsCorrect()
        {
            var content = new DocumentMergeResponse { Success = true };
            messageHandler.AddResponse(new Uri("https://test.io/merge/123/key"), content);

            messageHandler.RequestSent += req =>
            {
                var header = req.Headers.Authorization;
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes("API_KEY:API_SECRET"));

                Assert.NotNull(header);
                Assert.That(header.Scheme, Is.EqualTo("Basic"));
                Assert.That(header.Parameter, Is.EqualTo(token));
            };

            await client.MergeDocumentAsync(123, "key", new Dictionary<string, object>(), false);
        }

        [Test]
        public async Task CreateDocumentFromHtml()
        {
            messageHandler.AddResponse(new Uri("https://test.io/api/documents"), DefaultResponseDocumentJson);

            messageHandler.RequestSent += req =>
            {
                var body = req.Content.ReadAsAsync<DocumentRequest>().Result;
                Assert.NotNull(body);

                Assert.That(body.DocumentType, Is.EqualTo(DocumentType.Html));
                Assert.That(body.FileContents, Is.Null);
                Assert.That(body.Html, Is.EqualTo("<h1>{$Test}</h1>"));
            };

            var document = await client.CreateDocument("Test").FromHtml("<h1>{$Test}</h1>");

            Assert.NotNull(document);
            Assert.That(document.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task CreateDocumentFromFile()
        {
            messageHandler.AddResponse(new Uri("https://test.io/api/documents"), DefaultResponseDocumentJson);

            var fileBytes = Encoding.UTF8.GetBytes("This is a file");
            var expectedFileContents = Convert.ToBase64String(fileBytes);

            messageHandler.RequestSent += req =>
            {
                var body = req.Content.ReadAsAsync<DocumentRequest>().Result;
                Assert.NotNull(body);

                Assert.That(body.DocumentType, Is.EqualTo(DocumentType.Docx));
                Assert.That(body.Html, Is.Null);
                Assert.That(body.FileContents, Is.EqualTo(expectedFileContents));
            };

            var document = await client.CreateDocument("Test").FromFile(fileBytes, DocumentType.Docx);

            Assert.NotNull(document);
            Assert.That(document.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task CreateDocumentFromFileOnDisk()
        {
            messageHandler.AddResponse(new Uri("https://test.io/api/documents"), DefaultResponseDocumentJson);

            var fileBytes = Encoding.UTF8.GetBytes("This is a file");
            const string path = "D:\\path\\to\\file.docx";

            fileHelper.Setup(x => x.Exists(It.Is<string>(c => c.Equals(path)))).Returns(true);
            fileHelper.Setup(x => x.ReadAllBytes(It.Is<string>(c => c.Equals(path)))).Returns(fileBytes);

            var expectedFileContents = Convert.ToBase64String(fileBytes);

            messageHandler.RequestSent += req =>
            {
                var body = req.Content.ReadAsAsync<DocumentRequest>().Result;
                Assert.NotNull(body);

                Assert.That(body.DocumentType, Is.EqualTo(DocumentType.Docx));
                Assert.That(body.Html, Is.Null);
                Assert.That(body.FileContents, Is.EqualTo(expectedFileContents));
                Assert.That(body.Folder, Is.EqualTo("contracts"));
                Assert.That(body.OutputType, Is.EqualTo(DocumentOutputType.Email));
                Assert.That(body.OutputName, Is.EqualTo("From {$FirstName}"));
                Assert.That(body.Name, Is.EqualTo("Test"));
            };

            var document = await client.CreateDocument("Test", DocumentOutputType.Email, "From {$FirstName}", "contracts").FromFile(fileBytes, DocumentType.Docx);

            Assert.NotNull(document);
            Assert.That(document.Name, Is.EqualTo("Test"));
        }
    }
}
