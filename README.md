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

### Merge Document

#### MergeDocumentAndDownloadAsync `async Task<Stream>`

*Merge the given document with the given object and download it (with download=1).*

|Argument|Type|Description|
|:---|:---|:---|
|documentId|int|The identifier for the document template to merge data into|
|documentKey|string|The document key|
|mergeObject|object|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|bool|Set to true to test the document merge without using your allowance (test=1)|

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
|documentId|int|The identifier for the document template to merge data into|
|documentKey|string|The document key|
|mergeObject|object|The object that maps with the merge fields in the document template. e.g. a property named `FirstName` would map to a merge field `{$FirstName}`|
|testMode|bool|Set to true to test the document merge without using your allowance (test=1)|

**Example**  
Assuming the document template contains `{$FirstName}` and `{$LastName}` tokens

```c#
using(var client = new WebMergeClient())
{
    var person = new {FirstName = "Jack", LastName = "Daniel" };
    var result = await client.MergeDocumentAsync(documentId, documentKey, person);
    
    if(result.Success)
    {
        // rejoyce!
    }
    else
    {
        // weep
    }
}
```

### Create Document  
*(notifications not implemented)*

#### CreateDocumentAsync `Task<Document>`

*Create a new document template*

|Argument|Type|Description|
|:---|:---|:---|
|request|DocumentRequest|An instance of either `HtmlDocumentRequest` or `FileDocumentRequest`|

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

### Update document

### List documents

### Get a document

### Get the fields for a document

### Get the file for a document

### Copy a document

### Delete a document

## Data Route

### Merge a data route


## Not done yet

#### Data Routes
* Get a data route
* Get the fields for a data route
* Delete a data route
