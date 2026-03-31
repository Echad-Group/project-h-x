using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Models.Configuration
{
    public class JwtConfig
    {
        public JwtConfig() { }
        public string Key { get; set; } = string.Empty;
        public JwtIssuers Issuers { get; set; } = new();
        public JwtAudience Audiences { get; set; } = new();
    }

    public class JwtIssuers
    {
        public JwtIssuers() { }
        public string Local { get; set; } = string.Empty;
        public string Hosted { get; set; } = string.Empty;
        public string Temp { get; set; } = string.Empty;
    }

    public class JwtAudience
    {
        public JwtAudience() { }
        public string Local { get; set; } = string.Empty;
        public string Hosted { get; set; } = string.Empty;
        public string Temp { get; set; } = string.Empty;
    }
}
