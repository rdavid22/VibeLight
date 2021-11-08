using System;
using System.Collections.Generic;
using System.Text;

namespace VibelightApp
{
    public class DeviceList
    {
        public string Device;
        public string Alias;
        public string IP;
        public string Led;
        public string ListID;
        public DeviceList(string deviceName, string aliasName, string ipAddress, string ledNumber, string listId)
        {
            Device = deviceName;
            Alias = aliasName;
            IP = ipAddress;
            Led = ledNumber;
            ListID = listId;
        }
        public DeviceList()
        {
          
        }

       

    }

}
