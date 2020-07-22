using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var client = new AlphaVantageHttpClient(Options.Create(new BaseConfiguration
            {
                ApiKey = "8WC4PZDT3P3116N3"
            }));

            var result = await client.GetDailyQuotes("AAPL");
            
            Assert.NotNull(result);
        }
        
        [Fact]
        public async Task Test2()
        {
            var repo = new QuoteRepository(Options.Create(new BaseConfiguration
            {
                ConnectionString = "User ID=sa;Password=sa;Host=localhost;Port=5431;Database=dev;Pooling=true;"
            }));

            await repo.InsertQuotes(new List<QuoteDTO>
            {
                new QuoteDTO
                {
                    Date = DateTime.Now,
                    Symbol = "TEST"
                }
            });

            var result = await repo.GetQuotes("TEST");
            
            Assert.True(result.Count >= 1);
        }
        
        [Fact]
        public async Task LastWeekTest()
        {
            var repo = new QuoteRepository(Options.Create(new BaseConfiguration
            {
                ConnectionString = "User ID=sa;Password=sa;Host=localhost;Port=5431;Database=dev;Pooling=true;"
            }));

            await repo.InsertQuotes(new List<QuoteDTO>
            {
                new QuoteDTO
                {
                    Date = DateTime.Now.AddDays(-8),
                    Symbol = "TESTW",
                    Close = 12543453.3452345234M
                }
            });

            var result = await repo.GetQuotes("TESTW");
            
            Assert.True(result.Count == 0);
        }
    }
}