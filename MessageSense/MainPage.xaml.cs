using Microsoft.Maui.Essentials;

namespace MessageSense;
public partial class MainPage : ContentPage
{
	private AppManager _appManager;

	public MainPage(AppManager appManager)
	{
		InitializeComponent();
		_appManager = appManager;
	}

	private void CountNewMessages()
	{
		var newMsgCount = _appManager.MessageSenseData.Messages.Where(m => m.Read == false).Count();
		newMsgLbl.Text = $"New Messages: {newMsgCount}";
	}

	private void OnContactsPage(object sender, EventArgs e)
	{
		Application.Current.MainPage = new ContactsPage(_appManager);
	}
}

