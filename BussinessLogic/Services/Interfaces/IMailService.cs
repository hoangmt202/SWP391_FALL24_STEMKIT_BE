using BusinessLogic.DTOs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendReceipt(SendMailRequest sendMailRequest);
    }
}
