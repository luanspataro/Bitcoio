using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Bitcoio.Integration.Response
{
    public class BitPrecoResponse
    {
        public decimal Avg { get; set; }
        public bool Error = false;
    }
}
