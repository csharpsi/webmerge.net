# webmerge.net
C# library for the WebMerge API

[![Build status](https://ci.appveyor.com/api/projects/status/9rk7abm8qyi7hlus?svg=true)](https://ci.appveyor.com/project/csharpsi/webmerge-net)

**Note** This library is not finished yet, least of all because there is no documentation!

You will need to set up an account with [WebMerge](https://www.webmerge.me) and have an API Key and an API Secret.

For more information on the WebMerge API, read through their [documentation](https://www.webmerge.me/developers)

## Installation 

***Not yet available***

```
Install-Package WebMerge.Net
```

## Authentication
Authenticating with the WebMerge C# SDK is as simple as adding an `appSetting` to either your web.config or app.config file with the key 'WebMerge.ApiKey' and the value of your API Key and an *Environment Variable* with the key 'WebMerge.ApiSecret' and the value of your API Secret.

```xml
<appSettings>
    <add key="WebMerge.ApiKey" value="3K83KMN1AL6M1MXMVVCKR66BP9NA" />
</appSettings>
``` 

**Do not set the API Secret in code that you check in, unless you want people stealing your subscription ;)**

[How to set environment variables in Windows](http://lmgtfy.com/?q=windows+10+set+environment+variable)

The webmerge.net library will take these values and create the correct Authorization header.

## Usage

.NET 4.5 or higher required. This documentation assumes knowledge of [Asynchronus Programming](https://msdn.microsoft.com/en-us/library/hh191443.aspx)

You'll need to add the namespace

```c#
using WebMerge.Client;
```

Other namespaces you may need:

```c#
using WebMerge.Client.Enums;
using WebMerge.Client.RequestModels;
using WebMerge.Client.ResponseModels;
using WebMerge.Client.Utils;
```

Create a client object:

```c#
using(var client = new WebMergeClient())
{
    // access API methods from the client object
}
```

Alternativley, if you are using a dependency injection container, you could inject the `IWebMergeClient` interface into your constructor, making sure that the container properly disposes the object when it's done with it

``` c#
private readonly IWebMergeClient webMergeClient;

public MyClass(IWebMergeClient webMergeClient) 
{
    this.webMergeClient = webMergeClient;
}
```

## Documents
[API Docs](https://www.webmerge.me/developers/documents)

#### MergeDocumentAndDownloadAsync `async Task<Stream>`

*Merge the given document with the given object and download it (with download=1).*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The identifier for the document template to merge data into|
|documentKey|`string`|The document key|
|mergeObject|`object`|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|`bool`|Set to true to test the document merge without using your allowance (test=1)|

**Example**  
Assuming the document template contains `{$FirstName}` and `{$LastName}` tokens

``` c#
using(var client = new WebMergeClient())
{
    var person = new {FirstName = "Jack", LastName = "Daniel" };
    var documentStream = await client.MergeDocumentAndDownloadAsync(documentId, documentKey, person);
    
    // Write stream to disk, pipe to output etc
}
```

#### MergeDocumentAsync `async Task<ActionResponse>`

*Merge a document without downloading (without download=1)*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The identifier for the document template to merge data into|
|documentKey|`string`|The document key|
|mergeObject|`object`|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|`bool`|Set to true to test the document merge without using your allowance (test=1)|

**Example**  
Assuming the document template contains `{$FirstName}` and `{$LastName}` tokens

```c#
using(var client = new WebMergeClient())
{
    var person = new {FirstName = "Jack", LastName = "Daniel" };
    var result = await client.MergeDocumentAsync(documentId, documentKey, person);
    
    if(result.Success)
    {
        // hooray!
    }
}
```

#### CreateDocumentAsync `async Task<Document>`

*Create a new document template (notifications not implemented)*

|Argument|Type|Description|
|:---|:---|:---|
|request|`DocumentRequest`|An instance of either `HtmlDocumentRequest` or `FileDocumentRequest`|

**HTML Example**

```c#
using(var client = new WebMergeClient())
{
    var request = new HtmlDocumentRequest("Proposal", "<h1>Dear {$FirstName},</h1>");
    var document = await client.CreateDocumentAsync(request);
}
```

**File Example**

```c#
using(var client = new WebMergeClient())
{
    var fileBytes = System.IO.File.ReadAllBytes("C:\\Documents\\ProposalTemplate.docx");
    var request = new FileDocumentRequest("Proposal", fileBytes, DocumentType.Docx);
    var document = await client.CreateDocumentAsync(request);
}
```

#### UpdateDocumentAsync `async Task<Document>`

*Update the given document using data from the given request object*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The identifier for the document to update|
|request|`DocumentUpdateRequest`|An instance of `DocumentUpdateRequest`. Properties that are null will be ignored from the request|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var request = new DocumentUpdateRequest();
    request.Name = "Employment Contract";
    
    // update the name of a document with id = 42
    var document = await client.UpdateDocumentAsync(42, request);
}
```

#### GetDocumentListAsync `async Task<List<Document>>`

*Get a list of your documents*

|Argument|Type|Description|
|:---|:---|:---|
|search|`string`|*(optional)* Search term to filter results by|
|folder|`string`|*(optional)* Filter documents by folder name|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var documents = await client.GetDocumentListAsync();
    
    // or
    var contracts = await client.GetDocumentListAsync("Contract");
    
    // or proposals in Client A's folder
    var proposals = await client.GetDocumentListAsync("Proposal", "ClientA");
    
    // or all documents in the Agreements folder (using named parameter)
    var agreements = await client.GetDocumentListAsync(folder: "Agreements");
}
```

#### GetDocumentAsync `async Task<Document>`

*Retrieve a document using it's identifier*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The document's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var document = await client.GetDocumentAsync(42);
}
```

#### GetDocumentFieldsAsync `async Task<List<Field>>`

*Get the fields associated with the given document*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The document's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var fields = await client.GetDocumentFieldsAsync(42);
}
```


#### GetFileForDocumentAsync `async Task<DocumentFile>`

*Get the file associated with the given document. File contents are automatically decoded into a byte array*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The document's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var fields = await client.GetDocumentFieldsAsync(42);
}
```
#### CopyDocumentAsync `async Task<Document>`

*Copy the given document to a new one*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The document's identifier|
|name|`string`|The name for the copied document|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var newDocument = await client.CopyDocumentAsync(42, "Proposal 2");
}
```

#### DeleteDocumentAsync `async Task<ActionResponse>`

*Delete the given document*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|`int`|The document's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var result = await client.DeleteDocumentAsync(42);
}
```

## Data Routes

[API Docs](https://www.webmerge.me/developers/routes)

#### MergeDataRouteWithSingleDownloadAsync `async Task<Stream>`

*Merge a data route with the given data and download as a single document. If multiple files are returned, an exception is thrown*

|Argument|Type|Description|
|:---|:---|:---|
|dataRouteId|`int`|The route's identifier|
|dataRouteKey|`string`|The route's key|
|mergeObject|`object`|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|`bool`|*(optional)* Set to true to test the document merge without using your allowance (test=1)|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var mergeFields = new {FirstName = "Jack", LastName = "Daniel"};
    var documentStream = await client.MergeDataRouteWithSingleDownloadAsync(42, "foobar", mergeFields);
}
```

#### MergeDataRouteAsync `async Task<ActionResponse>`

*Merge a data route with the given data*

|Argument|Type|Description|
|:---|:---|:---|
|dataRouteId|`int`|The route's identifier|
|dataRouteKey|`string`|The route's key|
|mergeObject|`object`|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|`bool`|*(optional)* Set to true to test the document merge without using your allowance (test=1)|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var mergeFields = new {FirstName = "Jack", LastName = "Daniel"};
    var result = await client.MergeDataRouteAsync(42, "foobar", mergeFields);
    
    if(result.Success)
    {
        // yay!
    }
}
```

#### MergeDataRouteWithMultipleDownloadAsync `async Task<MultipleFileRouteRequestState>`

*Merge a data route with the given merge fields and download 2 or more files. The files will be available as byte arrays*

|Argument|Type|Description|
|:---|:---|:---|
|dataRouteId|`int`|The route's identifier|
|dataRouteKey|`string`|The route's key|
|mergeObject|`object`|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|`bool`|*(optional)* Set to true to test the document merge without using your allowance (test=1)|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var mergeFields = new {FirstName = "Jack", LastName = "Daniel"};
    var result = await client.MergeDataRouteWithMultipleDownloadAsync(42, "foobar", mergeFields);
    
    foreach(var file in result.Files)
    {
        System.IO.File.WriteAllBytes($"C:\\Documents\\{file.Name}.pdf", file.FileContents);
    }
}
```

#### GetDataRouteListAsync `async Task<List<DataRoute>>`

*Get a list of your data routes*

**Example**

```c#
using(var client = new WebMergeClient())
{
    var routes = await client.GetDataRouteListAsync();
}
```

#### GetDataRouteAsync `async Task<DataRoute` 

*Get a data route with the given identitfier*

|Argument|Type|Description|
|:---|:---|:---|
|dataRouteId|`int`|The route's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var route = await client.GetDataRouteAsync(123);
}
```

#### GetDataRouteFieldsAsync `async Task<List<Field>>`

*Get the fields associated with the given data route*

|Argument|Type|Description|
|:---|:---|:---|
|dataRouteId|`int`|The route's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var fields = await client.GetDataRouteFieldsAsync(123);
}
```

#### DeleteDataRouteAsync `async Task<ActionResponse>`

*Delete the data route with the given identifier*

|Argument|Type|Description|
|:---|:---|:---|
|dataRouteId|`int`|The route's identifier|

**Example**

```c#
using(var client = new WebMergeClient())
{
    var result = await client.DeleteDataRouteAsync(123);
}
```

### TODO

* Documentation for building from source
