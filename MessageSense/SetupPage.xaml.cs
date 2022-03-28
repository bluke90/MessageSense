namespace MessageSense;

public partial class SetupPage : ContentPage
{
	private AppManager _appManager;

	public SetupPage(AppManager appManager)
	{
		InitializeComponent();
		_appManager = appManager;
	}

	private void GetInputedData(object sender, EventArgs e)
    {
		var username = userNameEntry.Text;
		var firstName = firstNameEntry.Text;
		var lastName = lastNameEntry.Text;

		Microsoft.Maui.Essentials.Preferences.Set("username", username);
		Microsoft.Maui.Essentials.Preferences.Set("firstName", firstName);
		Microsoft.Maui.Essentials.Preferences.Set("lastName", lastName);

		Application.Current.MainPage = new MainPage(_appManager);
	}
}