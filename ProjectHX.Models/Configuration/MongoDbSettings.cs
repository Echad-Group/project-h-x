using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Models.Configuration
{
    public class MongoDbSettings
    {

        public string ConnectionString { get; set; } = string.Empty;

        public string Database { get; set; } = string.Empty;
    }
}
