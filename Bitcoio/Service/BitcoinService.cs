﻿using Bitcoio.Models;
using Bitcoio.Integration.Response;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Bitcoio.Service
{
    public class BitcoinService
    {
        private readonly HttpClient _httpClient;

        public BitcoinService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<BitPrecoResponse> Integration()
        {
            var response = await _httpClient.GetAsync($"https://api.bitpreco.com/btc-brl/ticker");
            var jsonString = await response.Content.ReadAsStringAsync();

            var jsonObject = JsonConvert.DeserializeObject<BitPrecoResponse>(jsonString);

            if (jsonObject != null)
            {
                return jsonObject;
            }

            return new BitPrecoResponse
            {
                Error = true
            };
        }

        public async Task<Bitcoin> GetBitcoinDataAsync(DateTime userDate)
        {
            DateTime planilhaLimite = new DateTime(2025, 2, 21);

            if (userDate <= planilhaLimite)
            {
                return BtcCalc(userDate);
            }
            else
            {
                string url = $"https://www.mercadobitcoin.net/api/BTC/day-summary/{userDate.Year}/{userDate.Month}/{userDate.Day}";

                var response = await _httpClient.GetAsync(url);
                var jsonString = await response.Content.ReadAsStringAsync();

                var jsonObject = JsonConvert.DeserializeObject<MercadoBitcoinResponse>(jsonString);

                if (jsonObject != null && !jsonObject.Error)
                {
                    return new Bitcoin
                    {
                        Price = jsonObject.Closing
                    };
                }
            }

            throw new InvalidOperationException("Erro: Dados não recebidos da API.");
        }

        public ProfitResult ProfitCalc(decimal purchasePrice, decimal purchaseRate, decimal actualPrice)
        {
            decimal amount = purchasePrice / purchaseRate;
            decimal percentage = (actualPrice - purchaseRate) / purchaseRate * 100;
            decimal profit = amount * actualPrice - purchasePrice;
            decimal total = amount * actualPrice;

            return new ProfitResult { Amount = amount, Percentage = percentage, Profit = profit, Total = total };
        }

        public Bitcoin BtcCalc(DateTime userDate)
        {
            var sheet = ReadXls();

            var items = sheet
                .FirstOrDefault(item => item.PriceDate.ToShortDateString() == userDate.ToShortDateString())
                ?? throw new InvalidOperationException("Data não encontrada.");

            return items;
        }

        private static List<Bitcoin> ReadXls()
        {
            var response = new List<Bitcoin>();

            string relativePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "BitcoinDatePriceBRL.xlsx");
            FileInfo existingFile = new FileInfo(relativePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;

                for (int row = 2; row <= rowCount; row++)
                {
                    var bitcoin = new Bitcoin();
                    bitcoin.Price = Convert.ToDecimal(worksheet.Cells[row, 2].Value);
                    bitcoin.PriceDate = Convert.ToDateTime(worksheet.Cells[row, 1].Value);

                    response.Add(bitcoin);
                }
            }
            return response;
        }
    }
}
