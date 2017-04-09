using System;
using Windows.UI.Notifications;

namespace VideoNote.Controller
{
    public class MainController
    {
        public static async void ShowMessageBox(string title, string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        public static void ShowToast(string title, string message)
        {
            string toastXmlString = "<toast>"
                                   + "<visual version='1'>"
                                   + "<binding template='ToastGeneric'>"
                                   + $"<text id='1'>{title}</text>"
                                   + $"<text id='2'>{message}</text>"
                                   + "<image placement='appLogoOverride' src='https://unsplash.it/64?image=883' />"
                                   + "</binding>"
                                   + "</visual>"
                                   + "</toast>";
            
            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            toastDOM.LoadXml(toastXmlString);

            ToastNotification toast = new ToastNotification(toastDOM);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
