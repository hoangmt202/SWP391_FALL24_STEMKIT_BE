using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Payment
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PaymentUrl { get; set; }  // URL thanh toán từ PayOS
        public string OrderCode { get; set; }   // Mã đơn hàng từ PayOS
    }
}
