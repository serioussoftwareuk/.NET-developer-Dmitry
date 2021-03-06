﻿using System.Net.Http;
using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace API.Services
{
    public interface IAlphaVantageHttpClient
    {
        Task<QuotesHttpModel> GetDailyQuotes(string symbol);
        Task<QuotesHttpModel> GetHourlyQuotes(string market);
    }
    
    public class AlphaVantageHttpClient : IAlphaVantageHttpClient
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly BaseConfiguration _config;

        public AlphaVantageHttpClient(IOptions<BaseConfiguration> config)
        {
            _config = config?.Value;
        }

        public async Task<QuotesHttpModel> GetDailyQuotes(string market)
        {
            var response = await _client
                .GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={market}&apikey={_config.ApiKey}");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<QuotesHttpModel>(await response.Content.ReadAsStringAsync());
        }
        
        public async Task<QuotesHttpModel> GetHourlyQuotes(string market)
        {
            var response = await _client
                .GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={market}&apikey={_config.ApiKey}&interval=60min");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<QuotesHttpModel>(await response.Content.ReadAsStringAsync());
        }
    }
}