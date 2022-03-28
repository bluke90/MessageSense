namespace MessageSense;

public partial class SetupPage : ContentPage
{
	public SetupPage()
	{
		InitializeComponent();
	}

	private void GetInputedData()
    {
		var username = userNameEntry.Text;
		var firstName = firstNameEntry.Text;
		var lastName = lastNameEntry.Text;

		Microsoft.Maui.Essentials.Preferences.Set("username", username);
		Microsoft.Maui.Essentials.Preferences.Set("firstName", firstName);
		Microsoft.Maui.Essentials.Preferences.Set("lastName", lastName);

	}
}