﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace API.Models
{
    public class QuotesHttpModel
    {
        [JsonProperty("Meta Data")]
        public MetaData MetaData { get; set; }
        
        [JsonProperty("Time Series (60min)")]
        public Dictionary<string, Quote> TimeSeries { get; set; }
    }

    public class MetaData
    {
        [JsonProperty("1. Information")]
        public string Information { get; set; }
        
        [JsonProperty("2. Symbol")]
        public string Symbol { get; set; }
        
        [JsonProperty("3. Last Refreshed")]
        public string LastRefreshed { get; set; }

        [JsonProperty("4. Output Size")]
        public string OutputSize { get; set; }
        
        [JsonProperty("5. Time Zone")]
        public string TimeZone { get; set; }
    }

    public class Quote
    {
        [JsonProperty("1. open")]
        public decimal Open { get; set; }

        [JsonProperty("2. high")]
        public decimal High { get; set; }

        [JsonProperty("3. low")]
        public decimal Low { get; set; }

        [JsonProperty("4. close")]
        public decimal Close { get; set; }

        [JsonProperty("5. volume")]
        public decimal Volume { get; set; }

        public QuoteDTO ToDto(string symbol, QuoteType type, DateTime date) =>
            new QuoteDTO
            {
                Date = date,
                Open = Open,
                High = High,
                Low = Low,
                Close = Close,
                Volume = Volume,
                Symbol = symbol,
                Type = type
            };
    }

}