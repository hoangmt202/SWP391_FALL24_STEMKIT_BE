using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Mail
{
    public class SendMailRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string LabFileUrl { get; set; }
    }
}

