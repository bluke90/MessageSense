namespace MessageSense;

public partial class App : Application
{

	private AppManager _appManager;

	public App()
	{
		_appManager = new AppManager();

		InitializeComponent();

		// AddTestMessage();

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

	private void AddTestMessage()
    {
		Models.Message message = new Models.Message()
		{
			DateTime = DateTime.Now,
			RecipientToken = "0001",
			Data = "It's going good! How about yourself??",
			SenderToken = "18580206",
			Read = false
		};

		_appManager.MessageSenseData.Messages.Add(message);
		_appManager.MessageSenseData.SaveChanges();
	}

}
