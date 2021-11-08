using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VibelightApp.TabbedPages.EffectCollection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Interactive : ContentPage
    {
        private bool MicrophoneToggled = false;
        private bool OrientationSensorToggled = false;

        public Interactive()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            PageAnimation.Load(MainLayout, 30);
        }

        protected override void OnAppearing()
        {

        }

        protected async override void OnDisappearing()
        {

        }

        protected override bool OnBackButtonPressed()
        {
            ToggleOffOrientationSensor();

            App.Mic.StopReading();


            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

            return false;
        }

        private async void OnHueChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + Convert.ToString(Convert.ToInt32(Hue.Value)) + ",-1," + "-1," + "-1," + "-1");
            Hue.ThumbColor = Hue.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Hue.Value * 1.41), 80, 80, 200);
        }

        private async void OnSaturationChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + "-1," + Convert.ToString(Convert.ToInt32(Saturation.Value)) + ",-1," + "-1," + "-1");
        }

        private async void OnBrightnessChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + "-1," + "-1," + Convert.ToString(Convert.ToInt32(Brightness.Value)) + ",-1," + "-1");
        }

        private async void OnPositionChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + "-1," + "-1," + "-1," + Convert.ToString(Convert.ToInt32(Position.Value)) + ",-1");
        }

        private async void OnSizeChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + "-1," + "-1," + "-1," + "-1," + Convert.ToString(Convert.ToInt32(Size.Value)));
        }

        private async void Microphone_Clicked(object sender, EventArgs e)
        {

            if (OrientationSensorToggled || MicrophoneToggled) { Position.IsEnabled = true; Size.IsEnabled = true; Hue.IsEnabled = true; App.Mic.StopReading(); MicrophoneToggled = false; Microphone.BackgroundColor = Color.FromHex("414141"); return; }
            MicrophoneToggled = true;
            Microphone.BackgroundColor = Color.FromHex("645566");

            Position.Value = 5000;
            Position.IsEnabled = false;
            Size.IsEnabled = false;
            Hue.IsEnabled = false; ;




            App.Mic.StartReading("param:[0_255],-1,-1,-1,[0_10000]");

        }

        private void OrientationSensor_Clicked(object sender, EventArgs e)
        {
            if (OrientationSensorToggled || MicrophoneToggled) { Position.IsEnabled = true; OrientationSensorToggled = false; ToggleOffOrientationSensor(); OrientationSensors.BackgroundColor = Color.FromHex("414141"); return; }
            OrientationSensorToggled = true;
            OrientationSensors.BackgroundColor = Color.FromHex("645566");

            ToggleOnOrientationSensor();
            Position.IsEnabled = false;

        }

        private void ToggleOffOrientationSensor()
        {
            try
            {
                if (OrientationSensor.IsMonitoring)
                {
                    OrientationSensor.ReadingChanged -= OrientationSensorChanged;

                    OrientationSensor.Stop();

                }

            }
            catch (Exception ex) { }
            
        }
        private void ToggleOnOrientationSensor()
        {
            try
            {
                if (!OrientationSensor.IsMonitoring)
                {
                    OrientationSensor.ReadingChanged += OrientationSensorChanged;

                    OrientationSensor.Start(SensorSpeed.Fastest);

                }

            }
            catch (Exception ex) { }

        }
        async void OrientationSensorChanged(object sender, OrientationSensorChangedEventArgs e)
        {

            var data = e.Reading;
            int Yaxis = Convert.ToInt32((data.Orientation.Z + 1) * 10000);

            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + "-1," + "-1," + "-1," + Yaxis + ",-1");

        }
    }
}