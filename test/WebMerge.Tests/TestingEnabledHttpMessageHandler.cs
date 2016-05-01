using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebMerge.Tests
{
    public class TestingEnabledHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}