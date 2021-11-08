using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VibelightApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionPage : ContentPage
    {
        readonly Bluetooth bluetooth = new Bluetooth();
        public ConnectionPage()
        {

            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            BindingContext = this;

            GenerateButtons();

        }
        private async void Refresh_Clicked(object sender, EventArgs e)
        {
      

            await PageAnimation.ButtonPress(Refresh, 100);

            GenerateButtons();

        }
        private void GenerateButtons()
        {
            MyButtons.Children.Clear();
            List<string> bonded = new List<string>();
            bonded = bluetooth.PairedDevices();

            for (int i = 0; i < bonded.Count; i++)
            {

                var btn = new Button()
                {
                    Text = bonded[i],
                    StyleId = i.ToString(),
                    CornerRadius = 5,
                    BackgroundColor = Color.FromHex("A6CCFAFF"),
                    FontFamily = "sf",
                    FontSize = 16,
                    TextColor = Color.FromHex("343434"),
                    HeightRequest = 40,
                    TextTransform = TextTransform.None,

                };
                btn.Clicked += DynamicButtonClicked;
                MyButtons.Children.Add(btn);

            }

        }
        private async void DynamicButtonClicked(object sender, EventArgs e)
        {
            Connect.IsEnabled = false;

            var myBtn = sender as Button;
            for (int i = 0; i < MyButtons.Children.Count; i++)
            {
                Button tempBtn = MyButtons.Children[i] as Button;
                if (tempBtn.Text == myBtn.Text)
                {
                    tempBtn.BackgroundColor = Color.FromHex("A6FFE4FD");
                }
                else
                {
                    tempBtn.BackgroundColor = Color.FromHex("A6CCFAFF");
                }
            }

            Status.Text = "Trying to connect";
            Connect.IsEnabled = await bluetooth.Connect(myBtn.Text);
            if (Connect.IsEnabled) { Status.Text = "Connected"; }
            else { Status.Text = "Failed"; }
        }
        private async void Connect_Clicked(object sender, EventArgs e)
        {
            await Task.WhenAll(
           PageAnimation.ButtonPress(Connect, 100),
           PageAnimation.Unload(MainLayout, 10)
          );

            Loading.Opacity = 0;
            Loading.IsVisible = true;
            Loading.Text = "Connecting..";
            Loading.TranslationY = -20;

            await Task.WhenAll(
            Loading.TranslateTo(0, 0, 150, Easing.SinInOut),
            Loading.FadeTo(1, 150, Easing.SinInOut));

            if (await bluetooth.SendCredentialsAndGetState(Ssid.Text, Password.Text, 100))
            {
                await Task.Delay(6000);
                await Task.WhenAll(
               Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
               Loading.FadeTo(0, 150, Easing.SinInOut)

                );
                Loading.Text = "Connected";
                await Task.WhenAll(
               Loading.TranslateTo(0, 0, 150, Easing.SinInOut),
               Loading.FadeTo(1, 150, Easing.SinInOut)

                );

                
                App.Controllers = await App.Message.DiscoverDevicesAsync();

                if (App.Controllers.Count > 0)
                {
                    App.SelectedDevice = App.Controllers[0];
                }
                else
                {
                    App.SelectedDevice = null;
                }


                await Task.WhenAll(
                 Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
                 Loading.FadeTo(0, 150, Easing.SinInOut)

                  );

                 await Navigation.PushAsync(new TabbedPages.Effects(), false);
                this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                GC.Collect();
            }
            else
            {
                await Task.WhenAll(
                      Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
                      Loading.FadeTo(0, 150, Easing.SinInOut)

                       );
                Loading.Text = "Failed";
                await Task.WhenAll(
               Loading.TranslateTo(0, 0, 150, Easing.SinInOut),
               Loading.FadeTo(1, 150, Easing.SinInOut)

             );
                await Task.WhenAll(
                 Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
                 Loading.FadeTo(0, 150, Easing.SinInOut)

                  );

                PageAnimation.Load(MainLayout, 70);
            }

        }
        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {


        }

        private void Ssid_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private async void Skip_Clicked(object sender, EventArgs e)
        {

            await Task.WhenAll(
            PageAnimation.ButtonPress(Skip, 100),
            PageAnimation.Unload(MainLayout, 30)

        );
             await Navigation.PushAsync(new TabbedPages.Effects(), false);
             this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);

           
            GC.Collect();
        }


    }
}