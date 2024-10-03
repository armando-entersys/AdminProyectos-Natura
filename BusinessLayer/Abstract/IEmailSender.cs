using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(List<string> toEmails, string category, Dictionary<string, string> dynamicValues);
    }
}
