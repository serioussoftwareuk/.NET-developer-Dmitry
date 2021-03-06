﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace API.Repositories
{
    public interface IQuoteRepository
    {
        Task<List<QuoteDTO>> GetLastWeekQuotes(string symbol, QuoteType type, int days = 7);
        Task InsertQuotes(List<QuoteDTO> quotes);
    }
    
    public class QuoteRepository : IQuoteRepository
    {
        private readonly BaseConfiguration _config;

        public QuoteRepository(IOptions<BaseConfiguration> config)
        {
            _config = config?.Value;
        }
        
        public async Task<List<QuoteDTO>> GetLastWeekQuotes(string symbol, QuoteType type, int days = 7)
        {
            var sql = $"SELECT * FROM quotes " +
                      $"WHERE symbol = '{symbol}' " +
                      $"AND date > '{DateTime.Now.AddDays(-days):MM-dd-yyyy HH:mm:ss}' " +
                      $"AND type = {type:d} " +
                      $"ORDER BY date DESC";

            await using var conn = new NpgsqlConnection(_config.ConnectionString);
            
            var quotes = await conn.QueryAsync<QuoteDTO>(sql);

            return quotes.ToList();
        }

        public async Task InsertQuotes(List<QuoteDTO> quotes)
        {
            if (quotes == null || quotes.Count == 0) return;
            
            var values = string.Join(",", quotes.Select(x => $"('{x.Date:MM-dd-yyyy HH:mm:ss}', " +
                                                             $"{x.Type:d}, " +
                                                             $"'{x.Symbol}', " +
                                                             $"{x.Open.ToString(CultureInfo.InvariantCulture)}, " +
                                                             $"{x.Close.ToString(CultureInfo.InvariantCulture)}, " +
                                                             $"{x.High.ToString(CultureInfo.InvariantCulture)}, " +
                                                             $"{x.Low.ToString(CultureInfo.InvariantCulture)}, " +
                                                             $"{x.Volume.ToString(CultureInfo.InvariantCulture)})"));
            
            var sql = $"INSERT INTO quotes (date, type, symbol, open, close, high, low, volume) values {values}" +
                      $"ON CONFLICT (date, symbol, type) DO UPDATE SET " +
                      $"open = EXCLUDED.open," +
                      $"close = EXCLUDED.close," +
                      $"high = EXCLUDED.high," +
                      $"low = EXCLUDED.low," +
                      $"volume = EXCLUDED.volume";

            await using var conn = new NpgsqlConnection(_config.ConnectionString);
            
            await conn.ExecuteAsync(sql);
        }
    }
}