using Dyme.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VibelightApp
{
    public class MicrophoneHandler : Microphone
    {
        SimpleServiceManager simpleService = new SimpleServiceManager("glob");
        SimpleServiceOptions options = new SimpleServiceOptions();
        private bool Reading = false;
        private double TargetSensitivity = 0; // dinamikusan álítodik
        private int RefreshRate = 5; // ms
        private int TargetAmplitude = 3500;
        private int SmoothingValue = 10;

        public MicrophoneHandler()
        {
           
            Construct();
            options.ActionNamesAndTitles = new Dictionary<string, string>()
            {
            { "Action1", "Stop" }

            };

        } 

        private async void Construct()
        {
            await Initialize();
        }


        public async void StartReading(string message)
        {
            if (!IsInitialized() || Reading) { return; }

            simpleService = await SimpleServiceManager.Start<MyService>("Effect Running", "", options);

            long SmoothedValue = 0;
            double SensitivityValue = 20;
            string FormattedSend = "";
            long cycle = 0;
            int[] ValueArray = new int[SmoothingValue];

            Reading = true;

            await Start();

            await Task.Run(async () =>
            {
                while (Reading)
                {
                    await Task.Delay(RefreshRate);
                    FormattedSend = "";
                    string send = message;
                    SmoothedValue = 0;
                    if (cycle == SmoothingValue)
                    {
                        cycle = 0;
                    }
                    for (int i = 0; i < SmoothingValue; i++)
                    {
                        SmoothedValue = SmoothedValue + ValueArray[i];

                    }
                    SmoothedValue = SmoothedValue / SmoothingValue;
                    ValueArray[cycle] = Convert.ToInt32(Convert.ToDouble(Read()) * SensitivityValue);
                    FormattedSend = "";

                    MicrophoneSensitivity(ref SensitivityValue, ref SmoothedValue);
                    while (send.Contains("["))
                    {

                        long minimum = Convert.ToInt64(send.Substring(send.IndexOf("[") + 1, send.IndexOf("_") - send.IndexOf("[") - 1));
                        long maximum = Convert.ToInt64(send.Substring(send.IndexOf("_") + 1, send.IndexOf("]") - send.IndexOf("_") - 1));
                        long difference = Math.Abs(maximum - minimum);
                        long finalValue = 0;
                        if (maximum > minimum)
                        {
                            finalValue = Convert.ToInt64(minimum + (Convert.ToDouble(SmoothedValue) / 10000 * Convert.ToDouble(difference)));

                        }
                        else
                        {
                            finalValue = Convert.ToInt64(minimum - (Convert.ToDouble(SmoothedValue) / 10000 * Convert.ToDouble(difference)));

                        }
                        FormattedSend += send.Substring(0, send.IndexOf("[")) + Convert.ToString(finalValue);

                        send = send.Remove(0, send.IndexOf("]") + 1);
                        if (!send.Contains("["))
                        {
                            FormattedSend += send;
                        }

                    }
                    // Console.WriteLine(FormattedSend)
                    cycle++;
                    App.Message.MessageSendAsync(App.SelectedDevice, FormattedSend);

                }
            });
            simpleService.StopService(removeNotification: true);
            await Stop();

        }



        private void MicrophoneSensitivity(ref double sensitivity, ref long amplitude)
        {


            if (amplitude != 0)
            {
                long OverFlowValue = amplitude;

                TargetSensitivity = (TargetAmplitude / Convert.ToDouble(amplitude)) * sensitivity;

                if (amplitude > 10000)
                {
                    amplitude = 10000;
                }
                double AddingValue = Math.Abs(sensitivity - TargetSensitivity);
                AddingValue = AddingValue * 0.001;

                if (sensitivity > TargetSensitivity && sensitivity > 1)
                {
                    sensitivity = sensitivity - AddingValue;
                }
                else if (sensitivity < 100)
                {
                    sensitivity = sensitivity + AddingValue;

                }
            }
        }

        public void StopReading()
        {
            Reading = false;
           
        }
    }
    public class MyService : ISimpleService
    {
        public void OnExecuteAction(string actionName, IList<string> args)
        {
            // Event handler for actions declared in the "SimpleServiceOptions"
            if (actionName == "Action1") { Exit(); }

        }

        public void OnStart(SimpleServiceCore core)
        {
        }



        public void OnStop(IList<string> args)
        {

        }

        public void OnTapNotification()
        {
        }
        #region SERVICE METHODS

        private void Exit()
        {
            App.Message.MessageSendAsync(App.SelectedDevice,"end");
            App.Mic.StopReading();
            MessagingCenter.Send<object, string>(this, SimpleServiceCore.CALL_STOP_SERVICE, null);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        #endregion
    }
}


