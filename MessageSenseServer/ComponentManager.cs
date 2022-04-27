using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenseServer
{
    

    public class ComponentManager
    {
    
        public IConfiguration Configuration { get; private set; }

        public ComponentManager() {
            BuildConfiguration();
        }

        private void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            Configuration = builder.Build();
        }

    }
}
