using Microsoft.EntityFrameworkCore;


namespace MessageSense;

public partial class MessagePage : ContentPage
{
	private AppManager _appManager;
	private Models.Contact _contact;
	public MessagePage(AppManager appManager, Models.Contact contact)
	{
		InitializeComponent();
		_appManager = appManager;
		_contact = contact;
		ContactName.Text = contact.Name;
		PopulateMessages();
	}

	private List<Models.Message> _sendMessages;
	private List<Models.Message> _recMessages;
	private List<Models.Message> _messages;


	private async void PopulateMessages()
    {
		_messages = new List<Models.Message>();

		string myToken = Microsoft.Maui.Essentials.Preferences.Get("token", "0");
		if (myToken == null || myToken == "0") throw new Exception("Error Retreiving Users Token...");
		string contactToken = _contact.Token;

		_sendMessages = await _appManager.MessageSenseData.Messages.Where(m => m.SenderToken == myToken && m.RecipientToken == contactToken).ToListAsync();
		_recMessages = await _appManager.MessageSenseData.Messages.Where(m => m.SenderToken == contactToken && m.RecipientToken == myToken).ToListAsync();

		foreach(var msg in _sendMessages)
        {
			_messages.Add(msg);
        }
		foreach (var msg in _recMessages)
		{
			_messages.Add(msg);
		}
		_messages = _messages.OrderBy(m => m.DateTime).ToList();
		foreach (var msg in _messages)
        {
			var lbl = GenMsgLbl(msg);
			msgStack.Add(lbl);
        }
	}

	private static Label GenMsgLbl(Models.Message msg)
    {
		var lbl = new Label()
		{
			Text = msg.Data
		};

		return lbl;
    }

	private void OnSendMsg(object sender, EventArgs e)
    {
		var msg = msgEntry.Text;
		if (msg.Length == 0) return;

		DisplayAlert("Message Sent", "You're message has been sent", "Okay");
    }

	private void OnGoBack(object sender, EventArgs e)
    {
		Application.Current.MainPage = new ContactsPage(_appManager);
    }
}