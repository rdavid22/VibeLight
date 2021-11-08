using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VibelightApp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Effects : ContentPage
    {
        public Effects()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
            GenerateEffectButtons();
            PageAnimation.Load(EffectButtons, 100);
           
        }

        private void GenerateEffectButtons()
        {
            string[] EffectNames = { "Rocket", "Fire", "Gradient", "Lightning","Interactive" };

            EffectButtons.Children.Clear();

            for (int i = 0; i < EffectNames.Length; i++)
            {
                Button CurrentButton = new Button()
                {
                    Text = EffectNames[i],
                    StyleId = (i + 1).ToString(),
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("a5c8cf"),
                    FontFamily = "sf",
                    FontSize = 16,
                    Padding = new Thickness(0, 0, 0, 0),
                    Margin = new Thickness(3, 3, 3, 3),
                    TextTransform = TextTransform.None,
                    TextColor = Color.FromHex("454545"),
                    HeightRequest = 37
                };
                CurrentButton.Clicked += EffectButton_Clicked;
                EffectButtons.Children.Add(CurrentButton);
            }
        }

        public object GetInstance(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }

        private async void EffectButton_Clicked(object sender, EventArgs e)
        {
            Button ClickedButton = (Button)sender;

            // await Navigation.PushAsync(Activator.CreateInstance(Type.GetType("TabbedPages.EffectCollection.Fire")) as Page, true);

            if (ClickedButton.Text == "Rocket")
            {
     
                if (await App.Message.MessageSendAsync(App.SelectedDevice, "start:rocket"))
                {
                    await Navigation.PushAsync(new TabbedPages.EffectCollection.Rocket(), true);
                }
                else { await DisplayAlert("Error", "Device not found", "OK"); }
                
            }

            else if (ClickedButton.Text == "Fire")
            {
  
                if (await App.Message.MessageSendAsync(App.SelectedDevice, "start:fire"))
                {
                    await Navigation.PushAsync(new TabbedPages.EffectCollection.Fire(), true);
                }
                else { await DisplayAlert("Error", "Device not found", "OK"); }
                
            }

            else if (ClickedButton.Text == "Gradient")
            {
                if (await App.Message.MessageSendAsync(App.SelectedDevice, "start:gradient"))
                {
                    await Navigation.PushAsync(new TabbedPages.EffectCollection.Gradient(), true);
                }
                else { await DisplayAlert("Error", "Device not found", "OK"); }
            }

            else if (ClickedButton.Text == "Lightning")
            {
                if (await App.Message.MessageSendAsync(App.SelectedDevice, "start:lightning"))
                {
                    await Navigation.PushAsync(new TabbedPages.EffectCollection.Lightning(), true);
                }
                else { await DisplayAlert("Error", "Device not found", "OK"); }
            }
            else if (ClickedButton.Text == "Interactive")
            {
                if (await App.Message.MessageSendAsync(App.SelectedDevice, "start:interactive"))
                {
                    await Navigation.PushAsync(new TabbedPages.EffectCollection.Interactive(), true);
                }
                else { await DisplayAlert("Error", "Device not found", "OK"); }
            }
        }

        private async void Clear_Clicked(object sender, EventArgs e)
        {
            if (!await App.Message.MessageSendAsync(App.SelectedDevice, "end"))
            { await DisplayAlert("Error", "Device not found", "OK"); }
        }

        async void OnHueChanged(object sender, ValueChangedEventArgs args)
        {
            Hue.ThumbColor = Hue.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Hue.Value * 1.41), 80, 80, 200);
            await App.Message.MessageSendAsync(App.SelectedDevice, "lamp:" + Convert.ToString(Convert.ToInt32(Hue.Value)) + "," + Convert.ToString(Convert.ToInt32(Saturation.Value)) + "," + Convert.ToString(Convert.ToInt32(Brightness.Value)));
        }

        async void OnSaturationChanged(object sender, ValueChangedEventArgs args)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "lamp:" + Convert.ToString(Convert.ToInt32(Hue.Value)) + "," + Convert.ToString(Convert.ToInt32(Saturation.Value)) + "," + Convert.ToString(Convert.ToInt32(Brightness.Value)));
        }

        async void OnBrightnessChanged(object sender, ValueChangedEventArgs args)
        {
             await App.Message.MessageSendAsync(App.SelectedDevice, "lamp:" + Convert.ToString(Convert.ToInt32(Hue.Value)) + "," + Convert.ToString(Convert.ToInt32(Saturation.Value)) + "," + Convert.ToString(Convert.ToInt32(Brightness.Value)));
        }

        private async void List_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TabbedPages.DeviceSelectPage(), true);
        }

        protected override void OnAppearing()
        {
            if (App.SelectedDevice == null)
            {
                ListButton.Text = " None ";
            }
            else
            {
                ListButton.Text = "  " + App.SelectedDevice.Alias + "  ";
            }
           
            PageAnimation.Load(MainView, 100);

        }

        protected async override void OnDisappearing()
        {

           // await PageAnimation.Unload(MainView, 10);

        }

        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return false;
        }

        private async void SettingsButton_Clicked(object sender, EventArgs e)
        {

          
       
            await Navigation.PushAsync(new SettingsPage(), true);
            
        }

        private async void AddNewButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConnectionPage(), true);
            this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
        }
    }
}