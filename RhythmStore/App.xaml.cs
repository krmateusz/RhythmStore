using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool userChangedTheme = false; 

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!userChangedTheme)
            {
                SetThemeBasedOnTime();
            }
        }

        private void SetThemeBasedOnTime()
        {
            DateTime currentTime = DateTime.Now;

            if (currentTime.Hour >= 13 || currentTime.Hour < 6)
            {
                AppThemes.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));
            }
            else
            {
                AppThemes.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
            }
        }

        public void UserChangedTheme() 
        {
            userChangedTheme = true;
        }
    }
}