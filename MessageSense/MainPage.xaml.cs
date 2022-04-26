using Microsoft.Maui;

namespace MessageSense;
public partial class MainPage : ContentPage
{
	private AppManager _appManager;

	public MainPage(AppManager appManager)
	{
		InitializeComponent();
		_appManager = appManager;
		InitAppManagerServices();
		CheckPermissions();
		CountNewMessages();
	}

	private async void InitAppManagerServices() {
		await _appManager.InitPacketHandler();
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

	private void CheckPermissions()
    {
		Dispatcher.Dispatch(async () =>
		{
			var status_internet = await Permissions.CheckStatusAsync<Permissions.NetworkState>();
			var status_read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
			var status_write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

			if (status_internet != PermissionStatus.Granted) {
				status_internet = await Permissions.RequestAsync<Permissions.NetworkState>();				
			}
			if (status_read != PermissionStatus.Granted) {
				status_read = await Permissions.RequestAsync<Permissions.StorageRead>();
			}
			if (status_write != PermissionStatus.Granted) {
				status_write = await Permissions.RequestAsync<Permissions.StorageWrite>();
			}

		});
    }
}

