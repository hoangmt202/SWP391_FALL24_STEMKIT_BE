using BusinessLogic.DTOs.Payment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Implementation
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentService> _logger;
        private const string PayosUrl = "https://api.payos.com/v1/payment-requests";
        private const string ClientId = "58657f6c-82ae-4326-a44c-961c3e132dc8";
        private const string ApiKey = "26926c75-dd21-43a3-928c-458f667456aa";
        private const string ChecksumKey = "33111bdee334111cc717ddbfc983a9660b2c7116146b9b5fc9e86eb45f9d0c62";

        public PaymentService(HttpClient httpClient, ILogger<PaymentService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PaymentResult> CreatePaymentRequestAsync(decimal amount, string orderId, string description)
        {
            try
            {
                var paymentRequest = new
                {
                    orderCode = orderId,
                    amount = (long)(amount * 100), // Chuyển đổi sang số tiền nhỏ nhất (VD: đồng)
                    description = description,
                    returnUrl = "https://your-website.com/payment/callback",
                    cancelUrl = "https://your-website.com/payment/cancel",
                    signature = GenerateSignature(orderId, amount)
                };

                _httpClient.DefaultRequestHeaders.Add("x-client-id", ClientId);
                _httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);

                var jsonContent = JsonConvert.SerializeObject(paymentRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(PayosUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var paymentResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return new PaymentResult
                    {
                        Success = true,
                        PaymentUrl = paymentResponse.checkoutUrl,
                        OrderCode = paymentResponse.orderCode,
                        Message = "Payment request created successfully"
                    };
                }

                return new PaymentResult
                {
                    Success = false,
                    Message = $"Failed to create payment request: {responseContent}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment request for OrderId: {OrderId}", orderId);
                return new PaymentResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        private string GenerateSignature(string orderId, decimal amount)
        {
            // Implement signature generation according to PayOS documentation
            var dataToSign = $"{orderId}|{amount}|{ClientId}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ChecksumKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
