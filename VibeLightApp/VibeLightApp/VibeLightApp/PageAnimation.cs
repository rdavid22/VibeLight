using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VibelightApp
{
    public static class PageAnimation
    {

        public static async void Load(StackLayout MainLayout,uint speed)
        {

            
            for (int i = 0; i < MainLayout.Children.Count; i++)
            {
                await Task.WhenAll
                (
                  MainLayout.Children[i].FadeTo(1, speed,Easing.SinInOut),
                  MainLayout.Children[i].TranslateTo(0, 0, speed, Easing.SinInOut)
                );
            }

        }
       
        public static async Task Unload(StackLayout MainLayout, uint speed)
        {

            for (int i = 0; i < MainLayout.Children.Count; i++)
            {
                await Task.WhenAll(
                  MainLayout.Children[i].FadeTo(0, speed, Easing.SinInOut),
                  MainLayout.Children[i].TranslateTo(0, -20, speed, Easing.SinInOut)
                  );

            }
        }
        public static async Task ButtonPress(Button myBtn, uint speed)
        {
            await myBtn.ScaleTo(1.05, speed, Easing.SinInOut);

            await myBtn.ScaleTo(1.0, speed, Easing.SinInOut);
        }

    }
}
