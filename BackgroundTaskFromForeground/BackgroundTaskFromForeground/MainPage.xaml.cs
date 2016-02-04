using BackgroundTask;
using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BackgroundTaskFromForeground
{
    public sealed partial class MainPage : Page
    {
        private static readonly string TileRegistrationName = "TileUpdateTask";

        // This will hold the trigger reference that will allow us to run the task on demand
        private ApplicationTrigger backgroundTrigger;

        public MainPage()
        {
            this.InitializeComponent();

            this.RegisterBackgroundTask();
        }

        /// <summary>
        /// We proceed to register the task if it hasn't been registered yet. Otherwise, we fetch the registration
        /// and retrieve the ApplicationTrigger from there.
        /// </summary>
        private async void RegisterBackgroundTask()
        {
            try
            {
                if (!BackgroundTaskRegistration.AllTasks.Any(reg => reg.Value.Name == TileRegistrationName))
                {
                    // Configure task parameters
                    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                    builder.TaskEntryPoint = typeof(TileUpdateTask).FullName;
                    builder.Name = TileRegistrationName;

                    // Remember to set an ApplicationTrigger so we can run it on demand later
                    this.backgroundTrigger = new ApplicationTrigger();
                    builder.SetTrigger(backgroundTrigger);
                    builder.Register();

                    MessageDialog infoDialog = new MessageDialog("Background task successfully registered.", "Info");
                    await infoDialog.ShowAsync();
                }
                else
                {
                    // Fetch registration details and trigger if already existing
                    var registration = BackgroundTaskRegistration.AllTasks.FirstOrDefault(reg => reg.Value.Name == TileRegistrationName).Value as BackgroundTaskRegistration;
                    this.backgroundTrigger = registration.Trigger as ApplicationTrigger;

                    MessageDialog infoDialog = new MessageDialog("Background task registration data successfully retrieved.", "Info");
                    await infoDialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageDialog errorDialog = new MessageDialog("There was an error while trying to register the background task.", "Error");
                errorDialog.ShowAsync();
            }
        }

        private async void UpdateTile_Click(object sender, RoutedEventArgs e)
        {
            // Task hasn't been registered, show message
            if (backgroundTrigger == null)
            {
                MessageDialog infoDialog = new MessageDialog("Can't run task if it hasn't been registered.", "Info");
                await infoDialog.ShowAsync();

                return;
            }

            var taskParameters = new ValueSet();

            // Send a custom message if we have typed one
            if (!string.IsNullOrEmpty(MessageText.Text))
            {
                taskParameters.Add("Message", MessageText.Text);
            }

            // Fetch execution results and inform user accordingly
            var taskResult = await backgroundTrigger.RequestAsync(taskParameters);

            switch (taskResult)
            {
                default:
                case ApplicationTriggerResult.Allowed:
                    MessageDialog infoDialog = new MessageDialog("Background task successfully executed.", "Info");
                    await infoDialog.ShowAsync();
                    break;
                case ApplicationTriggerResult.CurrentlyRunning:
                case ApplicationTriggerResult.DisabledByPolicy:
                case ApplicationTriggerResult.UnknownError:
                    MessageDialog errorDialog = new MessageDialog("Error running background task: " + taskResult.ToString(), "Error");
                    await errorDialog.ShowAsync();
                    break;
            }
        }
    }
}
