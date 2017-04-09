using System;
using VideoNote.Controller;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideoNote.Views
{
    /// <summary>
    /// manage all eventsof setting page
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        // Contains local settings for entire app
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public SettingPage()
        {
            this.InitializeComponent();
            try
            {
                // Load theme as per local settings
                if (Convert.ToBoolean(localSettings.Values["app_theme"]) == true)
                    this.RequestedTheme = ElementTheme.Light;
                else
                    this.RequestedTheme = ElementTheme.Dark;

                // Default config for loading
                loadingPanel.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0x88, 0x88, 0x88, 0x88));
                loadingPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }

        #region Events

        /// <summary>
        /// On page load retriv local settings and display on controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Manage toggle control
                tglAppTheme.IsOn = Convert.ToBoolean(localSettings.Values["app_theme"]);
                tglOrientation.IsOn = Convert.ToBoolean(localSettings.Values["video_orientation"]);
                tglSaveNote.IsOn = Convert.ToBoolean(localSettings.Values["note_auto_save"]);
                tglPause.IsOn = Convert.ToBoolean(localSettings.Values["video_pause"]);

                // Manage video name
                string video_uri = localSettings.Values["video_uri"]?.ToString();
                if (video_uri != null)
                    txtVideoUri.Text = video_uri;
                else
                    txtVideoUri.Text = "Select any Video file!";
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Update setting on save note toggle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tglSaveNote_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                localSettings.Values["note_auto_save"] = tglSaveNote.IsOn;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Update setting on video pasue toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tglPause_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                localSettings.Values["video_pause"] = tglPause.IsOn;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Update settings on app theme toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tglAppTheme_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                localSettings.Values["app_theme"] = tglAppTheme.IsOn;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Update video orientation setting on toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tglOrientation_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                localSettings.Values["video_orientation"] = tglOrientation.IsOn;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Open video picker to select file and copy to local storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnChooseVideo_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            try
            {
                // show Open dialog
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                openPicker.FileTypeFilter.Add(".mp4");
                openPicker.FileTypeFilter.Add(".webm");
                openPicker.FileTypeFilter.Add(".mpeg");
                openPicker.FileTypeFilter.Add(".3gp");
                openPicker.FileTypeFilter.Add(".mkv");

                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    // Copy file to local settings
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    loadingPanel.Visibility = Visibility.Visible;
                    await file.CopyAsync(localFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    loadingPanel.Visibility = Visibility.Collapsed;
                    progressRing.IsActive = false;

                    // Update video name on setting
                    txtVideoUri.Text = file.Name;
                    localSettings.Values["video_uri"] = file.Name;

                }
                else
                    txtVideoUri.Text = "Select any Video file!";
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Clear app data including local setting & local storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Remove data & close app
                await ApplicationData.Current.ClearAsync();
                App.Current.Exit();
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }

        #endregion Events
    }
}
