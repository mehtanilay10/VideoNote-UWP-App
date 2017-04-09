using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Animations;
using VideoNote.Controller;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VideoNote.Views
{
    /// <summary>
    /// Home page that contain MediaElement & RichEditBox 
    /// </summary>
    public sealed partial class HomePage : Page
    {
        // Contains local settings for entire app
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public HomePage()
        {
            this.InitializeComponent();
            try
            {
                // Update app theme as per local settings
                if (Convert.ToBoolean(localSettings.Values["app_theme"]) == true)
                {
                    this.RequestedTheme = ElementTheme.Light;
                    homeVideoPanel.Background = new SolidColorBrush(Windows.UI.Colors.White);
                }
                else
                    this.RequestedTheme = ElementTheme.Dark;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }

        #region Events
        /// <summary>
        /// on page load retrive notes & videos and display it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtain video file name from local storage
                string video_uri = localSettings.Values["video_uri"]?.ToString();
                if (video_uri != null)
                {
                    // Load Video from local folder
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await localFolder.GetFileAsync(localSettings.Values["video_uri"].ToString());
                    if (file != null)
                    {
                        await LoadFileAsync(file);

                        // Show Video
                        homeMainPanel.Visibility = Visibility.Visible;
                        homeUpdateSetting.Visibility = Visibility.Collapsed;

                        // Ratate & Zoom for landscape
                        if (Convert.ToBoolean(localSettings.Values["video_orientation"]) == true)
                        {
                            float height = (float)mediaViewer.ActualHeight;
                            float width = (float)mediaViewer.ActualWidth;
                            await homeVideoPanel
                                .Rotate(value: 90, centerY: height / 5, centerX: width / 2)
                                .Scale(scaleX: 1.5F, scaleY: 1.5F, centerY: height / 2, centerX: width / 2)
                                .StartAsync();
                        }
                    }
                    // Set Note data
                    string data = await LoadNoteDataAsync();
                    noteTextBox.Document.SetText(Windows.UI.Text.TextSetOptions.CheckTextLimit, data);
                }
                else
                    MainController.ShowMessageBox("Error!", "Unable to load video! Pls select video from settings.");

                // Hide save button if auto save enable
                if (Convert.ToBoolean(localSettings.Values["note_auto_save"]) == true)
                    btnSaveFile.Visibility = Visibility.Collapsed;
                else
                    btnSaveFile.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// on pivot change pause or save note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void homeMainPanel_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            if (args.Item.Header.Equals("Note") && Convert.ToBoolean(localSettings.Values["video_pause"]) == true)
            {
                // If switch to note and video pause option is enabled then pause it
                mediaViewer.Pause();
            }
            else if (args.Item.Header.Equals("Video"))
            {
                // If switch to video and auto save optio is enabled then save file
                if (Convert.ToBoolean(localSettings.Values["note_auto_save"]) == true)
                    SaveToFile(false);
                // on video tab play video by default
                mediaViewer.Play();
            }
        }


        /// <summary>
        /// saving note to local storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile(true);
        }


        /// <summary>
        /// download richedittext data to sd card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDownloadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrive text data from textbox
                string data = string.Empty;
                noteTextBox.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out data);

                // Show save dialog
                Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("Text File", new List<string>() { ".txt" });
                savePicker.SuggestedFileName = localSettings.Values["video_uri"].ToString() + "_note.txt";
                StorageFile file = await savePicker.PickSaveFileAsync();
                await FileIO.WriteTextAsync(file, data);

                // Show Notification
                MainController.ShowToast("Downloaded!", "Note downloaded successfully");
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }

        #endregion Events

        #region Methods

        /// <summary>
        /// save contents to local storage
        /// </summary>
        /// <param name="showNotification">specify wherther show toast notification or not</param>
        private async void SaveToFile(bool showNotification)
        {
            try
            {
                // Retrive text data from textbox
                string data = string.Empty;
                noteTextBox.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out data);
                // If textbox have content
                if (data.Length != 0 || data != string.Empty)
                {
                    // Load Note from local folder
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await localFolder.CreateFileAsync(
                                        localSettings.Values["video_uri"].ToString() + "_note.txt",
                                        CreationCollisionOption.ReplaceExisting);
                    if (file != null)
                    {
                        await FileIO.WriteTextAsync(file, data);
                        // Show Notification
                        if (showNotification)
                            MainController.ShowToast("Saved!", "Note saved to Local data successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Load video file to MediaElement in async manner
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        internal async Task LoadFileAsync(StorageFile file)
        {
            try
            {
                // If file exixt
                if (file != null)
                {
                    // Load video in Media Element
                    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    mediaViewer.SetSource(stream, file.ContentType);
                    mediaViewer.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MainController.ShowMessageBox("Error!", ex.Message);
            }
        }


        /// <summary>
        /// Retrive data of note file
        /// </summary>
        /// <returns>text of note file</returns>
        private async Task<string> LoadNoteDataAsync()
        {
            // Retrive text data from textbox
            string data = string.Empty;
            noteTextBox.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out data);

            // Load Note from local folder
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync(localSettings.Values["video_uri"].ToString() + "_note.txt");
            if (file != null)
                return await FileIO.ReadTextAsync(file);
            return string.Empty;
        }

        #endregion Methods
    }
}
