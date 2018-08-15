using System;
using Xamarin.Forms;

namespace MqttTest
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
            BindingContext = App.MqttClientViewModel;
			InitializeComponent();
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new View.PublishMessage());
        }
    }
}
