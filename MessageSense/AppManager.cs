using MessageSense.ClientNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense
{
    public class AppManager
    {
        public Data.MessageSenseData MessageSenseData { get; set; }

        public AppManager()
        {
            MessageSenseData = new Data.MessageSenseData();
        }

        public void StartNet()
        {
            Task.Run(() => AsyncConnector.StartClient());
        }


    }
}
