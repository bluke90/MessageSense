namespace MessageSense;

public partial class App : Application
{

	private AppManager _appManager;

	public App()
	{
		_appManager = new AppManager();

		InitializeComponent();

		

		var user = Microsoft.Maui.Essentials.Preferences.Get("username", null);
		var fName = Microsoft.Maui.Essentials.Preferences.Get("firstName", null);
		var lName = Microsoft.Maui.Essentials.Preferences.Get("lastName", null);

		if (user == null || fName == null || lName == null)
		{
			MainPage = new SetupPage(_appManager);
		} else
        {
			MainPage = new MainPage(_appManager);
		}
	}
}
