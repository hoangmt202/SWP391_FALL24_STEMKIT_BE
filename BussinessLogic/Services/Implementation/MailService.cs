using BusinessLogic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using BusinessLogic.DTOs.Mail;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
using DataAccess.Data;
using Azure.Identity;

namespace BusinessLogic.Services.Implementation
{
    public class MailService : IMailService
    {
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        public MailService(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<bool> SendReceipt(SendMailRequest sendMailRequest)
        {
            try
            {
                var userName = sendMailRequest.UserName;
                var cartDto = await _cartService.GetCartAsync(userName);
                var labFileUrl = sendMailRequest.LabFileUrl ?? "Không có file Lab.";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("StemKit <StemKit24@gmail.com>");
                    mail.To.Add(sendMailRequest.Email.Trim());
                    mail.Subject = "Kit Lab";
                    mail.Body = $"<body style=\"font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8e5f6;\">" +
                        $"<div style=\"max-width: 600px; margin: 20px auto; background-color: #ffffff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\">" +
                        $"<h2 style=\"margin-bottom: 20px\">Xin chào,</h2>" +
                        $"<p style=\"font-size: 16px\">Cảm ơn vì đã tin tưởng và mua hàng của Stem Kit, hệ thống xin gửi đến cho bạn bài Lab đi kèm với sản phẩm</p>" +
                        $"<div style=\"background-color: #f8f9fa; border-radius: 5px; padding: 15px; margin-bottom: 20px;\">" +
                        $"<p style=\"margin: 0\"><strong>Bài Lab:</strong></p>" +
                        $"<p style=\"margin: 0\">{labFileUrl}</p>" +
                        $"</div>" +
                        $"<p style=\"margin-top: 20p\">Một lần nữa Stem Kit chân thành cảm ơn quý khách đã trải nghiệm và tin tưởng...</p>" +
                        $"</div></body>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("StemKit24@gmail.com", "yhle msbb nkxq kten");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
