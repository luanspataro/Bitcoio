using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Bitcoio.Models
{
    public class BitcoioModel
    {
        [Display(Name = "Selecione um valor")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Por favor, digite um valor.")]
        public decimal PurchaseValue { get; set; }


        [Display(Name = "Selecione uma data")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Por favor, selecione uma data.")]
        public DateTime PurchaseDate { get; set; }
    }

    public class Bitcoin
    {
        public decimal Price { get; set; }
        public DateTime PriceDate { get; set; }
    }

    public class ProfitResult
    {
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public decimal Profit { get; set; }
        public decimal Total { get; set; }
    }

}
