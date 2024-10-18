using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IEmailSender
    {
        void SendEmail(List<string> _toEmails, string _category, Dictionary<string, string> dynamicValues);
    }
}
