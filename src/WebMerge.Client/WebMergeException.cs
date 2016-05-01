using System;

namespace WebMerge.Client
{
    public class WebMergeException : Exception
    {
        public WebMergeException()
        {
            
        }

        public WebMergeException(string message)
            : base($"[WebMerge Error]: {message}")
        {
            
        }
    }
}