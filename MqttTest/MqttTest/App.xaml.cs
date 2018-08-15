using MqttTest.ViewModel;
using Xamarin.Forms;

namespace MqttTest
{
	public partial class App : Application
    {

        public static MqttClientViewModel MqttClientViewModel { get; set; }


        public App ()
		{
			InitializeComponent();
            MqttClientViewModel = new MqttClientViewModel();
            MainPage = new NavigationPage(new MqttTest.MainPage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
