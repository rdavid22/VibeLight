using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VibelightApp
{

    public interface IAndroidBluetooth
    {
        List<string> PairedDevices();
        Task<bool> Connect(string device);
        Task Send(string message);
        Task<string> Read();
    }

    public interface IAndroidWifi
    {
        string GetIp();
    }

    public interface IAndroidMicrophone
    {
        Task<bool> InitializeMicrophoneAsync();
        Task StartMicrophoneAsync();
        Task StopMicrophoneAsync();
        int GetAmplitude();
        bool IsInitialized();
        bool IsStarted();
    }

    class Bluetooth
    {
        public List<string> PairedDevices()
        {
            List<string> controllers = new List<string>();

            if (Device.RuntimePlatform == Device.Android)
            {
                controllers = DependencyService.Get<IAndroidBluetooth>().PairedDevices();
                return controllers;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
            return controllers;
        }
        public async Task<bool> Connect(string device)
        {

            if (Device.RuntimePlatform == Device.Android)
            {
                if (await DependencyService.Get<IAndroidBluetooth>().Connect(device))
                {
                    return true;
                }
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
            return false;
        }
        public async Task Send(string message)
        {

            if (Device.RuntimePlatform == Device.Android)
            {
                await DependencyService.Get<IAndroidBluetooth>().Send(message);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }

        }

        public async Task<string> Read()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return await DependencyService.Get<IAndroidBluetooth>().Read();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }

            return "";
        }
        public async Task<bool> SendCredentialsAndGetState(string ssid, string pw, int TimeoutSec)
        {
            TimeoutSec *= 1000;
            Stopwatch stopwatch = new Stopwatch();
            string message = "";
            bool connected = false;

            await Task.Run(async () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await Send("ssid:" + ssid);
                    await Task.Delay(2000);
                    await Send("pw:" + pw);

                    stopwatch.Stop();
                    stopwatch.Reset();
                    stopwatch.Start();

                    while (message != "connected" && message != "failed" && stopwatch.ElapsedMilliseconds < TimeoutSec / 3)
                    {
                        message = await Read();
                    }
                    if (message == "connected") { connected = true; break; }
                }
            });
            if (connected) { return true; }

            return false;
        }
    }

    static class WiFi
    {
        static public string GetIPadress()
        {
            string ip = "";

            if (Device.RuntimePlatform == Device.Android)
            {
                ip = DependencyService.Get<IAndroidWifi>().GetIp();
                return ip;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
            return ip;
        }

    }
    public class Microphone
    {

        //DependencyService.Get<IAndroidMicrophone>().glob();
        public async Task<bool> Initialize()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return await DependencyService.Get<IAndroidMicrophone>().InitializeMicrophoneAsync();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }

            return false;
        }

        public async Task Stop()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                await DependencyService.Get<IAndroidMicrophone>().StopMicrophoneAsync();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
        }
        public async Task Start()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                await DependencyService.Get<IAndroidMicrophone>().StartMicrophoneAsync();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
        }

        public int Read()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return DependencyService.Get<IAndroidMicrophone>().GetAmplitude();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
            return 0;
        }

        public bool IsInitialized()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return DependencyService.Get<IAndroidMicrophone>().IsInitialized();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
            return false;
        }
        public bool IsStarted()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return DependencyService.Get<IAndroidMicrophone>().IsStarted();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
            return false;
        }
    }

}
