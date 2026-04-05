using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Models.Configuration
{
    public class MailerSendTemplateIds
    {
        public string AccountVerification { get; set; } = string.Empty;
        public string PasswordReset { get; set; } = string.Empty;
    }
}
