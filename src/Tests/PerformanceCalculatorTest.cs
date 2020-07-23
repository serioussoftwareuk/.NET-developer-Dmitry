using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using API.Services;
using Xunit;

namespace Tests
{
    public class PerformanceCalculatorTest
    {
        [Fact]
        public void CalculateTest()
        {
            var calculator = new PerformanceCalculator();

            var quotes = MockQuotes("AAPL"); 

            var result = calculator.Calculate(quotes);

            var expected = new List<decimal> { 0, 10, 20, 40, 30, 15, 0 };
            
            Assert.Equal(expected, result.Items.Select(x => x.Value).ToList());
        }
        
        private List<QuoteDTO> MockQuotes(string symbol) => new List<QuoteDTO>
        {
            new QuoteDTO { Date = DateTime.Now.AddDays(-6).Date, Symbol = symbol, Close = 100 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-5).Date, Symbol = symbol, Close = 110 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-4).Date, Symbol = symbol, Close = 120 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-3).Date, Symbol = symbol, Close = 140 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-2).Date, Symbol = symbol, Close = 130 },
            new QuoteDTO { Date = DateTime.Now.AddDays(-1).Date, Symbol = symbol, Close = 115 },
            new QuoteDTO { Date = DateTime.Now.Date, Symbol = symbol, Close = 100 }
        };
    }
}