using MqttTest.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MqttTest.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PublishMessage : ContentPage
	{
		public PublishMessage ()
		{
            BindingContext = App.MqttClientViewModel;
			InitializeComponent ();
		}
	}
}