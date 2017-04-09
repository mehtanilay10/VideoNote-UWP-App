using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace VideoNote
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
//#if DEBUG
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                this.DebugSettings.EnableFrameRateCounter = true;
//            }
//#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

            if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == "VideoNote"))
                return;
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = "VideoNote";
            builder.TaskEntryPoint = "VideoNote.MainPage";
            builder.SetTrigger(new TimeTrigger(15, false));
            builder.Register();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainPage), args);
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        //public event EventHandler<BackRequestedEventArgs> BackRequested;

        //private void OnBackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    var eventHandler = this.BackRequested;
        //    if (eventHandler != null)
        //    {
        //        eventHandler(sender, e);
        //    }
        //    if (!e.Handled)
        //    {
        //        if (_rootFrame != null && _rootFrame.CanGoBack)
        //        {
        //            e.Handled = true;
        //            _rootFrame.GoBack();

        //        }
        //    }
        //}
    }
}
