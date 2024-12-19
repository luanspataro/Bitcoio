using Bitcoio.Models;
using Bitcoio.Integration.Response;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Bitcoio.Service
{
    public class BitcoinService
    {
        public async Task<BitPrecoResponse> Integration()
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://api.bitpreco.com/btc-brl/ticker");
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

        public ProfitResult ProfitCalc(decimal purchasePrice, decimal purchaseRate, decimal actualPrice)
        {
            decimal amount = purchasePrice / purchaseRate;
            decimal percentage = (actualPrice / purchaseRate - 1) * 100;
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

            string relativePath = @"Data\BitcoinDatePriceBRL.xlsx";
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
