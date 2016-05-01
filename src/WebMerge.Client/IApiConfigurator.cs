using System;

namespace WebMerge.Client
{
    public interface IApiConfigurator
    {
        string ApiKey { get; } 
        string ApiSecret { get; }
        Uri BaseUri { get; }
    }
}