using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using WebMerge.Client;
using WebMerge.Client.Enums;
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
        
        [SetUp]
        public void SetUp()
        {
            messageHandler = new TestingEnabledHttpMessageHandler();
            config = new Mock<IApiConfigurator>();
            
            config.Setup(x => x.ApiKey).Returns("API_KEY");
            config.Setup(x => x.ApiSecret).Returns("API_SECRET");
            config.Setup(x => x.BaseUri).Returns(new Uri("https://test.io"));

            var httpClient = new HttpClient(messageHandler);

            client = new WebMergeClient(httpClient, config.Object);
        }

        [Test]
        public async Task MergeDocumentAsyncWithoutDownload()
        {
            var content = JsonConvert.SerializeObject(new RequestState { Success = true });
            messageHandler.AddResponse(new Uri("https://test.io/merge/123/key"), content);

            var result = await client.MergeDocumentAsync(123, "key", new Dictionary<string, object>(), false);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task MergeDocumentAsyncWithDownload()
        {
            var data = new byte[] {0x2, 0x4};
            messageHandler.AddResponse(new Uri("https://test.io/merge/123/key?download=1"), data);

            var result = await client.MergeDocumentAsync(123, "key", new Dictionary<string, object>());

            Assert.NotNull(result);
            Assert.That(result, Is.EquivalentTo(data));
        }

        [Test]
        public async Task MergeDocumentInTestMode()
        {
            var data = new byte[] { 0x2, 0x4 };
            messageHandler.AddResponse(new Uri("https://test.io/merge/123/key?download=1&test=1"), data);

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

            messageHandler.AddResponse(new Uri("https://test.io/merge/123/key?download=1"), data);

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
            var content = JsonConvert.SerializeObject(new RequestState { Success = true });
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
                Assert.That(body.OutputType, Is.EqualTo(DocumentOutputType.Pdf));
                Assert.That(body.FileContents, Is.Null);
                Assert.That(body.Html, Is.EqualTo("<h1>{$Test}</h1>"));
            };

            var request = new HtmlDocumentRequest("Test", "<h1>{$Test}</h1>");

            var document = await client.CreateDocumentAsync(request);

            //var document = await client.CreateDocument("Test").FromHtml("<h1>{$Test}</h1>");

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

            var request = new FileDocumentRequest("Test", fileBytes, DocumentType.Docx);
            var document = await client.CreateDocumentAsync(request);

            Assert.NotNull(document);
            Assert.That(document.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task CreateDocumentFromFileOnDisk()
        {
            messageHandler.AddResponse(new Uri("https://test.io/api/documents"), DefaultResponseDocumentJson);

            var fileBytes = Encoding.UTF8.GetBytes("This is a file");
            const string path = "D:\\path\\to\\file.docx";

            FileHelper.ExistsFunc = p => p.Equals(path);
            FileHelper.FileReadFunc = p => fileBytes;
            
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

            var request = new FileDocumentRequest("Test", path, DocumentType.Docx, DocumentOutputType.Email)
            {
                Folder = "contracts",
                OutputName = "From {$FirstName}"
            };

            var document = await client.CreateDocumentAsync(request);

            Assert.NotNull(document);
            Assert.That(document.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task UpdateDocumentWithNewHtmlContent()
        {
            messageHandler.AddResponse(new Uri("https://test.io/api/documents/123"), DefaultResponseDocumentJson);

            messageHandler.RequestSent += req =>
            {
                var body = req.Content.ReadAsStringAsync().Result;
                Assert.NotNull(body);

                Assert.That(body, Is.EqualTo(@"{""html"":""<h1>This is a {$Test}</h1>""}"));
            };

            var request = new DocumentUpdateRequest
            {
                Html = "<h1>This is a {$Test}</h1>"
            };

            var document = await client.UpdateDocumentAsync(123, request);
            
            Assert.NotNull(document);
            Assert.That(document.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetDocumentList()
        {
            const string exampleResponse = @"
            [
                {
                    ""id"":""436346"",
                    ""key"":""firm3"",
                    ""type"":""pdf"",
                    ""name"":""1040 EZ"",
                    ""output"":""pdf"",
                    ""size"":"""",
                    ""size_width"":""8.5"",
                    ""size_height"":""11"",
                    ""active"":""1"",
    	            ""url"":""https://www.webmerge.me/merge/2436346/firm3""
                },{
                    ""id"":""436347"",
                    ""key"":""7icxf"",
                    ""type"":""pdf"",
                    ""name"":""1040 EZ - COPY"",
                    ""output"":""pdf"",
                    ""size"":"""",
                    ""size_width"":""8.5"",
                    ""size_height"":""16"",
                    ""active"":""0"",
    	            ""url"":""https://www.webmerge.me/merge/436347/7icxf""
                }
            ]";

            messageHandler.AddResponse(new Uri("https://test.io/api/documents"), exampleResponse);

            var documents = await client.GetDocumentListAsync();

            Assert.That(documents, Has.Count.EqualTo(2));
            var doc1 = documents[0];
            var doc2 = documents[1];

            Assert.That(doc1.Id, Is.EqualTo(436346));
            Assert.That(doc1.Key, Is.EqualTo("firm3"));
            Assert.That(doc1.DocumentType, Is.EqualTo(DocumentType.Pdf));
            Assert.That(doc1.Name, Is.EqualTo("1040 EZ"));
            Assert.That(doc1.OutputType, Is.EqualTo(DocumentOutputType.Pdf));
            Assert.That(doc1.SizeWidth, Is.EqualTo(8.5d));
            Assert.That(doc1.SizeHeight, Is.EqualTo(11d));
            Assert.That(doc1.IsActive, Is.EqualTo(true));
            Assert.That(doc1.Url, Is.EqualTo("https://www.webmerge.me/merge/2436346/firm3"));

            Assert.That(doc2.Id, Is.EqualTo(436347));
            Assert.That(doc2.Key, Is.EqualTo("7icxf"));
            Assert.That(doc2.DocumentType, Is.EqualTo(DocumentType.Pdf));
            Assert.That(doc2.Name, Is.EqualTo("1040 EZ - COPY"));
            Assert.That(doc2.OutputType, Is.EqualTo(DocumentOutputType.Pdf));
            Assert.That(doc2.SizeWidth, Is.EqualTo(8.5d));
            Assert.That(doc2.SizeHeight, Is.EqualTo(16d));
            Assert.That(doc2.IsActive, Is.EqualTo(false));
            Assert.That(doc2.Url, Is.EqualTo("https://www.webmerge.me/merge/436347/7icxf"));
        }

        [Test]
        public async Task GetHtmlDocument()
        {
            const string exampleResponse = @"
            {
                ""id"":""436347"",
                ""key"":""7icxf"",
                ""type"":""pdf"",
                ""name"":""1040 EZ - COPY"",
                ""output"":""pdf"",
                ""size"":"""",
                ""size_width"":""8.5"",
                ""size_height"":""16"",
                ""active"":""0"",
    	        ""url"":""https://www.webmerge.me/merge/436347/7icxf"",
                ""html"": ""<p>This is some HTML with a {$Token}</p>""
            }";

            messageHandler.AddResponse(new Uri("https://test.io/api/documents/436347"), exampleResponse);

            var document = await client.GetDocumentAsync(436347);

            Assert.NotNull(document);
            Assert.That(document.Id, Is.EqualTo(436347));
            Assert.That(document.Key, Is.EqualTo("7icxf"));
            Assert.That(document.DocumentType, Is.EqualTo(DocumentType.Pdf));
            Assert.That(document.Name, Is.EqualTo("1040 EZ - COPY"));
            Assert.That(document.OutputType, Is.EqualTo(DocumentOutputType.Pdf));
            Assert.That(document.SizeWidth, Is.EqualTo(8.5d));
            Assert.That(document.SizeHeight, Is.EqualTo(16d));
            Assert.That(document.IsActive, Is.EqualTo(false));
            Assert.That(document.Url, Is.EqualTo("https://www.webmerge.me/merge/436347/7icxf"));
            Assert.That(document.Html, Is.EqualTo("<p>This is some HTML with a {$Token}</p>"));
        }

        [Test]
        public async Task GetDocumentFields()
        {
            const string exampleResponse = @"
            [
                {""key"":""aflekjf409t3j4mg30m409m"", ""name"":""FirstName""},
                {""key"":""3to3igj3g3gt94j9304jfqw"", ""name"":""LastName""},
                {""key"":""t43j0grjaslkfje304vj9we"", ""name"":""Email""},
                {""key"":""3jg34gj0gj3gjq0gj0r9gje"", ""name"":""Phone""}
            ]";

            messageHandler.AddResponse(new Uri("https://test.io/api/documents/42/fields"), exampleResponse);

            var fields = await client.GetDocumentFieldsAsync(42);

            Assert.That(fields, Has.Count.EqualTo(4));
            Assert.That(fields[0].Name, Is.EqualTo("FirstName"));
            Assert.That(fields[0].Key, Is.EqualTo("aflekjf409t3j4mg30m409m"));
        }

        [Test]
        public async Task GetFileForDocument()
        {
            var fileBytes = Encoding.UTF8.GetBytes("This is a file");
            var fileContents = Convert.ToBase64String(fileBytes);

            var exampleResponse = $@"
            {{
                ""type"":""pdf"",
                ""last_update"":""2015-03-24 16:34:20"",
                ""contents"":""{fileContents}""
            }}";

            messageHandler.AddResponse(new Uri("https://test.io/api/documents/42/file"), exampleResponse);

            var file = await client.GetFileForDocumentAsync(42);

            Assert.NotNull(file);
            Assert.That(file.DocumentType, Is.EqualTo(DocumentType.Pdf));
            Assert.That(file.LastUpdated, Is.EqualTo(new DateTime(2015, 3, 24, 16, 34, 20)));
        }

        [Test]
        public async Task CopyDocument()
        {
            const string exampleResponse = @"
            {
                ""id"":""789789"",
                ""key"":""asdl4k"",
                ""type"":""html"",
                ""name"":""1040 EZier - CA"",
                ""output"":""pdf"",
                ""size"":"""",
                ""size_width"":""8.5"",
                ""size_height"":""11"",
                ""active"":""1"",
                ""url"":""https://www.webmerge.me/merge/789789/asdl4k"",
                ""fields"":[
    	            {""key"":""aflekjf409t3j4mg30m409m"", ""name"":""FirstName""},
    	            {""key"":""3to3igj3g3gt94j9304jfqw"", ""name"":""LastName""},
    	            {""key"":""t43j0grjaslkfje304vj9we"", ""name"":""Email""},
    	            {""key"":""3jg34gj0gj3gjq0gj0r9gje"", ""name"":""Phone""}
                ]
            }";

            messageHandler.AddResponse(new Uri("https://test.io/api/documents/42/copy"), exampleResponse);

            var result = await client.CopyDocument(42, "1040 EZier - CA");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("1040 EZier - CA"));
        }

        [Test]
        public async Task DeleteDocument()
        {
            const string exampleResponse = @"
            {
                ""success"" : ""1""
            }";

            messageHandler.AddResponse(new Uri("https://test.io/api/documents/42"), exampleResponse);

            var result = await client.DeleteDocument(42);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task MergeDataRouteWithSingleDownload()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a file"));
            
            messageHandler.AddResponse(new Uri("https://test.io/route/42/foobar?download=1"), stream);

            var result = await client.MergeDataRouteWithSingleDownloadAsync(42, "foobar", new {Testing = "ye"});

            Assert.That(result, Is.Not.Null);

            Assert.That(result, Is.EqualTo(stream));
        }

        [Test]
        public void MergeDataRouteWithSingleDownloadButMultipleFilesProvided()
        {
            var obj = new MultipleFileRouteRequestState
            {
                Success = true,
                Files = new List<DataRouteFile>
                {
                    new DataRouteFile
                    {
                        FileContents = new byte[] {0x2, 0x4},
                        Name = "Not really a file"
                    }
                }
            };

            var content = JsonConvert.SerializeObject(obj);

            messageHandler.AddResponse(new Uri("https://test.io/route/42/foobar?download=1"), content);

            Assert.ThrowsAsync<WebMergeException>(() => client.MergeDataRouteWithSingleDownloadAsync(42, "foobar", new { Testing = "ye" }));
        }

        [Test]
        public async Task MergeDataRoute()
        {
            const string exampleResponse = @"
            {
                ""success"" : ""1""
            }";

            messageHandler.AddResponse(new Uri("https://test.io/route/42/foo"), exampleResponse);

            var result = await client.MergeDataRouteAsync(42, "foo", new {Foo = "Bar"});

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task MergeDataRouteWithMultipleDownload()
        {
            var file1 = Convert.ToBase64String(Encoding.UTF8.GetBytes("File1"));
            var file2 = Convert.ToBase64String(Encoding.UTF8.GetBytes("File2"));

            var exampleResponse = $@"
            {{
                ""success"":1,
                ""files"": [
                    {{""name"":""Invoice.pdf"", ""file_contents"": ""{file1}""}},
                    {{""name"":""Thank You.pdf"", ""file_contents"": ""{file2}""}}
                ]
            }}";

            messageHandler.AddResponse(new Uri("https://test.io/route/42/bar?download=1"), exampleResponse);

            var result = await client.MergeDataRouteWithMultipleDownloadAsync(42, "bar", new {Foo = "Bar"});

            Assert.That(result.Success, Is.True);
            Assert.That(result.Files, Has.Count.EqualTo(2));
            Assert.That(result.Files[0].FileContents, Is.EquivalentTo(Convert.FromBase64String(file1)));
            Assert.That(result.Files[1].FileContents, Is.EquivalentTo(Convert.FromBase64String(file2)));
        }
    }
}
