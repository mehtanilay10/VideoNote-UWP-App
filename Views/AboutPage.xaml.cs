using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace VideoNote.Views
{
    /// <summary>
    /// About page containig details about app & developer
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        // Contains local settings for entire app
        Windows.Storage.ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public AboutPage()
        {
            this.InitializeComponent();
            try
            {
                // Update app theme as per local settings
                if (Convert.ToBoolean(localSettings.Values["app_theme"]) == true)
                    this.RequestedTheme = ElementTheme.Light;
                else
                    this.RequestedTheme = ElementTheme.Dark;
            }
            catch (Exception ex)
            {
                Controller.MainController.ShowMessageBox("Error!", ex.Message);
            }
        }
    }
}
