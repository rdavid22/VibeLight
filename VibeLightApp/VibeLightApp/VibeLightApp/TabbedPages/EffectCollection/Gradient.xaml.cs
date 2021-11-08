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
    public partial class Gradient : ContentPage
    {
        public int Counter = 0;
        public Random rand = new Random();

        public Gradient()
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

            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

            return false;
        }

      


        private async void OnBrightnessChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Brightness.Value) + ",-1"));
        }

        private async void OnSaturationChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Saturation.Value) + ",-1,-1,-1"));
        }

        private async void OnDelayingChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Delaying.Value)));
        }


        private async void StayButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,-1,-1,0,-1,-1");

        }

        private async void FrowardButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,-1,-1,1,-1,-1");
        }

        private async void Backwards_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,-1,-1,2,-1,-1");
        }



        private async void Color1ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color1.Value == 0)
            {
                Color1.ThumbColor = Color1.MinimumTrackColor =Color.Gray;
            }
            else
            {
                Color1.ThumbColor = Color1.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color1.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + Convert.ToString(Convert.ToInt32(Color1.Value) + ",-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1"));
        }
        private async void Color2ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color2.Value == 0)
            {
                Color2.ThumbColor = Color2.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color2.ThumbColor = Color2.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color2.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1," + Convert.ToString(Convert.ToInt32(Color2.Value) + ",-1,-1,-1,-1,-1,-1,-1,-1,-1,-1"));
        }
        private async void Color3ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color3.Value == 0)
            {
                Color3.ThumbColor = Color3.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color3.ThumbColor = Color3.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color3.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1," + Convert.ToString(Convert.ToInt32(Color3.Value) + ",-1,-1,-1,-1,-1,-1,-1,-1,-1"));
        }
        private async void Color4ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color4.Value == 0)
            {
                Color4.ThumbColor = Color4.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color4.ThumbColor = Color4.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color4.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1," + Convert.ToString(Convert.ToInt32(Color4.Value) + ",-1,-1,-1,-1,-1,-1,-1,-1"));
        }
        private async void Color5ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color5.Value == 0)
            {
                Color5.ThumbColor = Color5.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color5.ThumbColor = Color5.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color5.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Color5.Value) + ",-1,-1,-1,-1,-1,-1,-1"));
        }
        private async void Color6ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color6.Value == 0)
            {
                Color6.ThumbColor = Color6.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color6.ThumbColor = Color6.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color6.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Color6.Value) + ",-1,-1,-1,-1,-1,-1"));
        }
        private async void Color7ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color7.Value == 0)
            {
                Color7.ThumbColor = Color7.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color7.ThumbColor = Color7.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color7.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Color7.Value) + ",-1,-1,-1,-1,-1"));
        }
        private async void Color8ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Color8.Value == 0)
            {
                Color8.ThumbColor = Color8.MinimumTrackColor = Color.Gray;
            }
            else
            {
                Color8.ThumbColor = Color8.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Color8.Value * 1.41), 80, 80, 200);
            }
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1,"+Convert.ToString(Convert.ToInt32(Color8.Value)+",-1,-1,-1,-1"));
        }
    }

}