using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.Bluetooth;
using VibelightApp.Droid;
using System.Threading.Tasks;
using Java.Util;
using Java.IO;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothAndroid))]

namespace VibelightApp.Droid
{
    [Activity(Label = "BluetoothAndroid")]
    class BluetoothAndroid : IAndroidBluetooth
    {
        private BluetoothSocket MainSocket;

        private readonly BluetoothAdapter MainAdapter = BluetoothAdapter.DefaultAdapter;

        public List<string> PairedDevices()
        {
            List<string> bonded = new List<string>();

            foreach (var temp in MainAdapter.BondedDevices)
            {
                if (temp.Name.StartsWith("VibelightController"))
                {
                    bonded.Add(temp.Name);
                }
            }
            return bonded;
        }

        public async Task<bool> Connect(string DeviceName)
        {
            BluetoothDevice device;
            device = (from bd in MainAdapter.BondedDevices
                      where bd.Name.Equals(DeviceName)
                      select bd).FirstOrDefault();

            if (device == null) { return false; }

            try
            {
                MainSocket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                await MainSocket.ConnectAsync();

                if (MainSocket.IsConnected)
                {
                    return true;
                }
            }
            catch (Exception) { return false; }

            return false;
        }

        public async Task Send(string message)
        {
            byte[] toSendArray = new byte[100];
            for (int i = 0; i < message.Length; i++)
            {
                toSendArray[i] = Convert.ToByte(message[i]);
            }
            await MainSocket.OutputStream.WriteAsync(toSendArray, 0, message.Length);
        }

        public async Task<string> Read()
        {
            var Reader = new InputStreamReader(MainSocket.InputStream);
            var Buffer = new BufferedReader(Reader);

            return await Buffer.ReadLineAsync(); 
        }
    }
}