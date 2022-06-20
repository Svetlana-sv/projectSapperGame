using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SapperGame
{
    public partial class App : Application
    {
        public static String difficulty = "Легкий";
        public App()
        {
            InitializeComponent();

            Restart();
        }

        public void Restart()
        {
            MainPage = new NavigationPage(new MainPage(this));
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
