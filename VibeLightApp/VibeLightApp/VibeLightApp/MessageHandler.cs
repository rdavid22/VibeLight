using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace VibelightApp
{
    public class MessageHandler : UDP
    {
        public async Task<bool> MessageSendAsync(DeviceList device, string message)
        {
            if (device == null) { return false; }

            if (message.StartsWith("start:") || message.StartsWith("end") || message.StartsWith("rename:") || message.StartsWith("reset"))
            {
                return await ValidationSendAsync(device.IP, message, 3000);
            }

            else
            {
                await SendAsync(device.IP, message);
                return true;
            }
        }

        public async Task<List<DeviceList>> DiscoverDevicesAsync()
        {
            List<DeviceList> Devices = new List<DeviceList>();

            await Task.Run(async () =>
            {


                string temp = await BroadcastAsyncAndGetResponse("helo", 4000);

                string[] TempArray = temp.Split('\n');

                for (int i = 0; i < TempArray.Length - 1; i++)
                {
                    string[] ParametersArray = TempArray[i].Split(',');

                    DeviceList device = new DeviceList
                    {
                        Device = ParametersArray[0],
                        IP = ParametersArray[1],
                        Led = ParametersArray[2],
                        Alias = ParametersArray[3],
                        ListID = Devices.Count.ToString()
                    };

                    Devices.Add(device);
                }
            });

            return Devices;
        }
        public List<DeviceList> DiscoverDevices()
        {
            List<DeviceList> Devices = new List<DeviceList>();



            string temp = BroadcastAndGetResponse("helo", 5000);

            string[] TempArray = temp.Split('\n');

            for (int i = 0; i < TempArray.Length - 1; i++)
            {
                string[] ParametersArray = TempArray[i].Split(',');

                DeviceList device = new DeviceList
                {
                    Device = ParametersArray[0],
                    IP = ParametersArray[1],
                    Led = ParametersArray[2],
                    Alias = ParametersArray[3],
                    ListID = Devices.Count.ToString()
                };

                Devices.Add(device);
            }


            return Devices;
        }
    }

    //                                      ඞ


}
