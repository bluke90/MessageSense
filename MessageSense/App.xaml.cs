using Microsoft.Maui;


namespace MessageSense;

public partial class App : Application
{

	private AppManager _appManager = null;

	public App() {
		InitializeComponent();

		var user = Preferences.Get("username", null);
		var fName = Preferences.Get("firstName", null);
		var lName = Preferences.Get("lastName", null);
		var appUser = Preferences.Get("appUser", null);

		if (user == null || fName == null || lName == null || appUser == null) {
			MainPage = new SetupPage(_appManager);
		} else {
			_appManager = new AppManager();
			MainPage = new MainPage(_appManager);
		}
	}
}