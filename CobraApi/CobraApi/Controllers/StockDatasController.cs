﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CobraApi.Models;
using System.Text.Json;

namespace CobraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockDatasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const string API_KEY = "cvi0sn9r01qks9q7hi0gcvi0sn9r01qks9q7hi10";

        public StockDatasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StockDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockData>>> GetStockDatas()
        {
            return await _context.StockDatas.ToListAsync();
        }

        // GET: api/StockDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockData>> GetStockData(int id)
        {
            var stockData = await _context.StockDatas.FindAsync(id);

            if (stockData == null)
            {
                return NotFound();
            }

            return stockData;
        }



        // POST: api/StockDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StockData>> PostStockData()
        {
            StockData stockData = new StockData
            {
                price = 25,
                date = DateTime.Now,
                FavoriteTickers_id = 5,
            };
            _context.StockDatas.Add(stockData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStockData", new { id = stockData.id }, stockData);
        }


        // POST: api/StockDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPost]
        public async Task<ActionResult<StockData>> PostStockData(StockData stockData)
        {
            _context.StockDatas.Add(stockData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStockData", new { id = stockData.id }, stockData);
        }*/


        // DELETE: api/StockDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockData(int id)
        {
            var stockData = await _context.StockDatas.FindAsync(id);
            if (stockData == null)
            {
                return NotFound();
            }

            _context.StockDatas.Remove(stockData);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("UpdateCurrentPrices")]
        public async Task<IActionResult> UpdateCurrentPrices()
        {
            var client = new HttpClient();
            var favoriteTickers = await _context.FavoriteTickers.ToListAsync();

            foreach (var ticker in favoriteTickers)
            {
                var url = $"https://finnhub.io/api/v1/quote?symbol={ticker.ticker}&token=" + API_KEY;
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync();
                var quote = JsonSerializer.Deserialize<QuoteResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (quote == null || quote.c == 0) continue;

                var newStockData = new StockData
                {
                    price = quote.c,
                    date = DateTime.Now,
                    FavoriteTickers_id = ticker.id
                };

                _context.StockDatas.Add(newStockData);
            }

            await _context.SaveChangesAsync();

            return Ok("Prices updated.");
        }

    }
}
