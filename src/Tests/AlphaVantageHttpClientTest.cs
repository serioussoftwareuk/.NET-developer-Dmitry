using System.Threading.Tasks;
using API;
using API.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests
{
    public class AlphaVantageHttpClientTest
    {
        [Fact]
        public async Task GetDailyQuotesTest()
        {
            var client = new AlphaVantageHttpClient(Options.Create(new BaseConfiguration
            {
                ApiKey = "8WC4PZDT3P3116N3"
            }));

            var result = await client.GetDailyQuotes("AAPL");
            
            Assert.NotNull(result);
        }
    }
}