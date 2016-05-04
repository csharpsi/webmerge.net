using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using WebMerge.Client.Enums;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Tests
{
    [TestFixture]
    public class SerialiserTests
    {
        [Test]
        public void CanCorrectlyConvertJsonStringToDocument()
        {
            const string json =
                @"{
                ""id"":""123"",
                ""key"":""dockey"",
                ""type"":""pptx"",
                ""name"":""Document Name"",
                ""output"":""email"",
                ""size"":null,
                ""size_width"":null,
                ""size_height"":null,
                ""active"": ""1"",
                ""url"":""https://www.test.io"",
                ""fields"": [ 
                    {""key"": ""456"", ""name"": ""FieldName""}
                ]
            }";

            var document = JsonConvert.DeserializeObject<Document>(json);

            Assert.NotNull(document);

            Assert.That(document.DocumentType, Is.EqualTo(DocumentType.Pptx));
            Assert.That(document.OutputType, Is.EqualTo(DocumentOutputType.Email));
            Assert.That(document.Id, Is.EqualTo(123));
            Assert.That(document.Name, Is.EqualTo("Document Name"));
            Assert.That(document.Fields, Has.Count.EqualTo(1));
            Assert.That(document.Fields[0].Name, Is.EqualTo("FieldName"));
            Assert.That(document.Fields[0].Key, Is.EqualTo("456"));
            Assert.That(document.IsActive, Is.True);
        }

        [Test]
        public void CanCorrectlySerialiseADocument()
        {
            const string expectedJson = @"{""id"":""123"",""key"":""dockey"",""type"":""pptx"",""name"":""Document Name"",""output"":""email"",""size"":null,""size_width"":null,""size_height"":null,""active"":""1"",""url"":""https://www.test.io"",""fields"":[{""key"":""456"",""name"":""FieldName""}]}";

            var document = new Document
            {
                Id = 123,
                Key = "dockey",
                Name = "Document Name",
                DocumentType = DocumentType.Pptx,
                OutputType = DocumentOutputType.Email,
                IsActive = true,
                Url = "https://www.test.io",
                Fields = new List<Field>
                {
                    new Field("456", "FieldName")
                }
            };

            var json = JsonConvert.SerializeObject(document);

            Assert.AreEqual(expectedJson, json);
        }
    }
}