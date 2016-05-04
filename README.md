# webmerge.net
C# library for the WebMerge API

[![Build status](https://ci.appveyor.com/api/projects/status/9rk7abm8qyi7hlus?svg=true)](https://ci.appveyor.com/project/csharpsi/webmerge-net)

**Note** This library is not finished yet, least of all because there is no documentation!

You will need to set up an account with [WebMerge](https://www.webmerge.me) and have an API Key and an API Secret.

For more information on the WebMerge API, read through their [documentation](https://www.webmerge.me/developers)

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

You'll need to add the namespace

```c#
using WebMerge.Client;
```

Other namespaces you may need to add are:

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

## Documents
[API Docs](https://www.webmerge.me/developers/documents)

### Merge Document

**MergeDocumentAndDownloadAsync**  
Merge a document with `download=1` set


### Create Document (notifications not implemented)

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