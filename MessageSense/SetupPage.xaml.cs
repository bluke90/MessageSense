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

		Microsoft.Maui.Essentials.Preferences.Set("token", GenerateContactToken());


		Application.Current.MainPage = new MainPage(_appManager);
	}

	private static string GenerateContactToken()
    {
		int[] tokenArray = new int[8];

		Random rand = new Random();

		for (int i = 0; i < tokenArray.Length; i++)
        {
			tokenArray[i] = rand.Next(10);
        }
		string token = string.Empty;
		foreach(int numb in tokenArray)
        {
			token += numb.ToString();
        }
		return token;
    }
}