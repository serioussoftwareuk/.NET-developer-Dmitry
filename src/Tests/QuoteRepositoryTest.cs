using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using API.Models;
using API.Repositories;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests
{
    public class QuoteRepositoryTest
    {
        [Fact]
        public async Task InsertQuotesTest()
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
                    Symbol = "TEST",
                    Type = QuoteType.Base
                }
            });

            var result = await repo.GetLastWeekQuotes("TEST", QuoteType.Base);
            
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
                    Close = 12543453.3452345234M,
                    Type = QuoteType.Base
                }
            });

            var result = await repo.GetLastWeekQuotes("TESTW", QuoteType.Base);
            
            Assert.True(result.Count == 0);
        }
    }
}