using MessageSense.ClientNet;
using Microsoft.Maui.Storage;

namespace MessageSense;

public partial class SetupPage : ContentPage
{
	private AppManager _appManager;

	public SetupPage(AppManager appManager)
	{
		InitializeComponent();
		_appManager = appManager;
	}

	private async void GetInputedData(object sender, EventArgs e)
    {
		var username = userNameEntry.Text;
		var firstName = firstNameEntry.Text;
		var lastName = lastNameEntry.Text;

		Preferences.Set("username", username);
		Preferences.Set("firstName", firstName);
		Preferences.Set("lastName", lastName);

		try {
			var appUser = await Authentication.NewUserNegotiation(username, firstName, _appManager);
			var prefData = appUser.SerializeAppUserObj();

			Preferences.Set("appUser", prefData);
			Preferences.Set("contactToken", appUser.ContactToken);
			Application.Current.MainPage = new MainPage(_appManager);
			return;
		} catch (Exception _) {
			await DisplayAlert("Error in server client negotiation", "Try again later", "okay");
			Application.Current.MainPage = new SetupPage(_appManager);
        }

	}

}