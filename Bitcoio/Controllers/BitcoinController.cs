using Bitcoio.Models;
using Bitcoio.Integration.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bitcoio.Service;

namespace Bitcoio.Controllers
{
    public class BitcoinController : Controller
    {
        private readonly BitcoinService _bitcoinService;

        public BitcoinController(BitcoinService bitcoinService)
        {
            _bitcoinService = bitcoinService;
        }

        public IActionResult Index()
        {
            return View(new BitcoioModel());
        }

        public IActionResult Info()
        {
            return View();
        }

        public IActionResult Contato()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetBitcoinPriceAsync([FromBody] BitcoioModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var btcData = _bitcoinService.BtcCalc(model.PurchaseDate);
                    BitPrecoResponse response = await _bitcoinService.Integration();


                    var profitData = _bitcoinService.ProfitCalc(model.PurchaseValue, btcData.Price, response.Last);

                    decimal amount = profitData.Amount;
                    decimal percentage = profitData.Percentage;
                    decimal profit = profitData.Profit;
                    decimal total = profitData.Total;

                    return Json(new
                    {
                        success = true,
                        amount = amount.ToString("0.#####"),
                        percentage = percentage,
                        profit = profit.ToString("C2", new System.Globalization.CultureInfo("pt-BR")),
                        total = total.ToString("C2", new System.Globalization.CultureInfo("pt-BR"))
                    });
                }
                catch (InvalidOperationException ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }
            return Json(new { success = false, error = "Preencha os campos corretamente." });
        }
    }
}
