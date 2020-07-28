using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public interface IQuoteService
    {
        Task<List<QuoteDTO>> GetWeeklyHourQuotesAsync(string symbol, QuoteType type);
    }
    
    public class QuoteService : IQuoteService
    {
        private readonly IAlphaVantageHttpClient _alphaVantageHttpClient;
        private readonly IQuoteRepository _quoteRepository;

        public QuoteService(IAlphaVantageHttpClient alphaVantageHttpClient, IQuoteRepository quoteRepository)
        {
            _alphaVantageHttpClient = alphaVantageHttpClient;
            _quoteRepository = quoteRepository;
        }
        
        public async Task<List<QuoteDTO>> GetWeeklyHourQuotesAsync(string symbol, QuoteType type)
        {
            var quotes = await _quoteRepository.GetLastWeekQuotes(symbol, type);

            if (!ValidateDbQuotes(quotes))
            {
                quotes = await FetchQuotes(symbol, type);
                await _quoteRepository.InsertQuotes(quotes);
            }

            return quotes;
        }

        private bool ValidateDbQuotes(List<QuoteDTO> quotes)
        {
            var latestQuote = quotes
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();
            
            if(latestQuote != null)
                return DateTime.Now - latestQuote.Date < TimeSpan.FromHours(2);

            return false;
        }
        
        private async Task<List<QuoteDTO>> FetchQuotes(string symbol, QuoteType type)
        {
            var apiQuotes = await _alphaVantageHttpClient.GetHourlyQuotes(symbol);
            
            if(apiQuotes?.TimeSeries == null)
                return null;
            
            return MergeQuotes(apiQuotes, symbol, type);
        }

        private List<QuoteDTO> MergeQuotes(QuotesHttpModel apiQuotes, string symbol, QuoteType type)
        {
            var quotes = apiQuotes.TimeSeries
                .Select(x => x.Value.ToDto(symbol, type, DateTime.Parse($"{x.Key} -5").ToUniversalTime()))
                .Where(x => x.Date >= DateTime.UtcNow.AddDays(-7))
                .ToList();

            var result = new List<QuoteDTO>();
            int i;
            for (i = 1; i < quotes.Count; i += 2)
            {
                if (quotes[i - 1].Date.Date == quotes[i].Date.Date)
                {
                    result.Add(new QuoteDTO
                    {
                        Symbol = quotes[i].Symbol,
                        Date = quotes[i].Date,
                        Type = type,
                        Open = quotes[i - 1].Open,
                        Close = quotes[i].Close,
                        High = Math.Max(quotes[i].High, quotes[i - 1].High),
                        Low = Math.Min(quotes[i].Low, quotes[i - 1].Low),
                        Volume = quotes[i - 1].Volume + quotes[i].Volume
                    });
                }
                else
                {
                    result.Add(quotes[i - 1]);
                    i--;
                }
            }

            if (i == quotes.Count && i > 0)
            {
                result.Add(quotes[i - 1]);
            }

            return result;
        }
    }
}