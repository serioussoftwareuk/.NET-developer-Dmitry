using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;
using API.Services;
using Moq;
using Xunit;

namespace Tests
{
    public class QuoteServiceTest
    {
        [Fact]
        public async Task GetWeeklyQuotesFullDbTest()
        {
            string symbol = "AAPL";

            var client = MockHttpClient(symbol);
            var repo = MockRepository(symbol, QuoteType.Base);
            
            var service = new QuoteService(client.Object, repo.Object);

            var quotes = await service.GetWeeklyHourQuotesAsync(symbol, QuoteType.Base);
            
            Assert.Equal(13, quotes.Count);
        }
        
        [Fact]
        public async Task GetWeeklyQuotesEmptyDbTest()
        {
            string symbol = "AAPL";

            var client = MockHttpClient(symbol);
            var repo = MockRepository(symbol, QuoteType.Base);
            
            repo
                .Setup(x => x.GetLastWeekQuotes(symbol, QuoteType.Base, 7))
                .ReturnsAsync(new List<QuoteDTO>());
            
            var service = new QuoteService(client.Object, repo.Object);

            var quotes = await service.GetWeeklyHourQuotesAsync(symbol, QuoteType.Base);
            
            Assert.Equal(9, quotes.Count);
        }

        private Mock<IQuoteRepository> MockRepository(string symbol, QuoteType type)
        {
            var repo = new Mock<IQuoteRepository>();

            repo
                .Setup(x => x.GetLastWeekQuotes(symbol, type, 7))
                .ReturnsAsync(MockDbQuotes(symbol, type));

            repo
                .Setup(x => x.InsertQuotes(It.IsAny<List<QuoteDTO>>()))
                .Verifiable();

            return repo;
        }

        private Mock<IAlphaVantageHttpClient> MockHttpClient(string symbol)
        {
            var client = new Mock<IAlphaVantageHttpClient>();
            
            client
                .Setup(x => x.GetDailyQuotes(symbol))
                .ReturnsAsync(MockHttpQuotes(symbol));
            
            client
                .Setup(x => x.GetHourlyQuotes(symbol))
                .ReturnsAsync(MockHttpQuotes(symbol));

            return client;
        }
        
        private QuotesHttpModel MockHttpQuotes(string symbol) => new QuotesHttpModel
        {
            MetaData = new MetaData
            {
                Symbol = symbol
            },
            TimeSeries = new Dictionary<string, Quote>
            {
                { DateTime.Now.AddDays(-8).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddDays(-6).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddDays(-5).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddDays(-4).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddDays(-3).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddHours(-4).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddHours(-3).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddHours(-2).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.AddHours(-1).ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }},
                { DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), new Quote { Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }}
            }
        };
        
        private List<QuoteDTO> MockDbQuotes(string symbol, QuoteType type) => new List<QuoteDTO>
        {
            new QuoteDTO { Date = DateTime.Now.AddDays(-8), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-7), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-6), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-5), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-4), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-3), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-2), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-1), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddHours(-8), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddHours(-6), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddHours(-4), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now.AddHours(-2), Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 },
            new QuoteDTO { Date = DateTime.Now, Symbol = symbol, Type = type, Open = 1, Close = 1, High = 1, Low = 1, Volume = 1 }
        };
    }
}