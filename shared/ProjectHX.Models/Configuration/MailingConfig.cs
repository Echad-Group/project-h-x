using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Models.Configuration
{
    public class MailingConfig
    {
        public MailingConfig()
        {

        }
        public string MailServer { get; set; } = string.Empty;
        public string SecureMailServer { get; set; } = string.Empty;
        public string AccountsEmailName { get; set; } = string.Empty;
        public string InfoEmailName { get; set; } = string.Empty;
        public string SupportEmailName { get; set; } = string.Empty;
        public string AccountsEmail { get; set; } = string.Empty;
        public string InfoEmail { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public string AccountsEmailPassword { get; set; } = string.Empty;
        public string SendGrid_SMTP_API { get; set; } = string.Empty;
        public int SSL_SMTP_Port { get; set; }
        public int SMTP_Port_If_Google { get; set; }
        public int SMTP_Port1 { get; set; }
        public int SMTP_Port2 { get; set; }
    }
}
