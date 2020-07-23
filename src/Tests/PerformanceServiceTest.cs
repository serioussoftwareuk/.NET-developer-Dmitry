using System.Linq;
using System.Threading.Tasks;
using API;
using API.Repositories;
using API.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests
{
    public class PerformanceServiceTest
    {
        private readonly IPerformanceService _service;

        public PerformanceServiceTest()
        {
            var config = Options.Create(new BaseConfiguration
            {
                ConnectionString = "User ID=sa;Password=sa;Host=localhost;Port=5431;Database=dev;Pooling=true;",
                ApiKey = "8WC4PZDT3P3116N3",
                SP = "SPY" 
            });
            
            var client = new AlphaVantageHttpClient(config);
            
            var repo = new QuoteRepository(config);
            
            _service = new PerformanceService(client, repo, config);
        }
        
        [Fact]
        public async Task CalculateTest()
        {
            var result = await _service.CalculateAsync("AAPL");
            
            Assert.NotNull(result);
            Assert.Equal("AAPL", result.Base.Symbol);
            Assert.Equal("SPY", result.SP.Symbol);
            Assert.Equal(7, result.Base.Items.Count);
            Assert.Equal(7, result.SP.Items.Count);
            Assert.Equal(0, result.Base.Items.First().Value);
            Assert.Equal(0, result.SP.Items.First().Value);
        }
    }
}