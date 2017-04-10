using System;
using System.Collections.Generic;
using VideoNote.Models;
using VideoNote.Views;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideoNote
{
    /// <summary>
    /// Manage hamburger menu
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Contains local settings for entire app
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();
            //localSettings.Values.Clear();

            // Set Menu
            hamburgerMenuControl.ItemsSource = MenuItem.GetMainItems();
            hamburgerMenuControl.OptionsItemsSource = MenuItem.GetOptionsItems();
            hamburgerMenuControl.PaneBackground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0x88, 0x17, 0x17, 0x17));

            // Set Theme
            if (Convert.ToBoolean(localSettings.Values["app_theme"]) == true)
                this.RequestedTheme = ElementTheme.Light;
            else
                this.RequestedTheme = ElementTheme.Dark;

            if (localSettings.Values.Count == 0)
            {
                // Assign Default Settings & show update settings
                localSettings.Values["app_theme"] = false;
                localSettings.Values["video_orientation"] = false;
                localSettings.Values["video_pause"] = true;
                localSettings.Values["note_auto_save"] = true;
            }
            // Redirect to Home page by defailt
            contentFrame.Navigate(typeof(HomePage));
        }

        #region Events

        /// <summary>
        /// Event handler for Hamburger Menu, change page in contentFrame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            // Open different page as per user selected in hamburger menu
            var menuItem = e.ClickedItem as MenuItem;
            contentFrame.Navigate(menuItem.PageType);
            hamburgerMenuControl.IsPaneOpen = false;
        }

        /// <summary>
        /// When open from file explorer then open video with default settings
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var args = e.Parameter as Windows.ApplicationModel.Activation.IActivatedEventArgs;
            if (args != null)
            {
                if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.File)
                {
                    // Obtain files from arguments
                    var fileArgs = args as Windows.ApplicationModel.Activation.FileActivatedEventArgs;
                    string strFilePath = fileArgs.Files[0].Path;
                    var file = (StorageFile)fileArgs.Files[0];

                    // Redirect to home and load video
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    await file.CopyAsync(localFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    localSettings.Values["video_uri"] = file.Name;
                    contentFrame.Navigate(typeof(HomePage));
                }
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (hamburgerMenuControl.IsPaneOpen == true)
            {
                hamburgerMenuControl.IsPaneOpen = false;
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        #endregion Events
    }

}
