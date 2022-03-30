using Microsoft.EntityFrameworkCore;


namespace MessageSense;

public partial class MessagePage : ContentPage
{
	private AppManager _appManager;
	private Models.Contact _contact;
	private string _myToken;

	public MessagePage(AppManager appManager, Models.Contact contact)
	{
		InitializeComponent();
		_appManager = appManager;
		_contact = contact;
		ContactName.Text = contact.Name;

		_myToken = Microsoft.Maui.Essentials.Preferences.Get("token", "0");
		if (_myToken == null || _myToken == "0") throw new Exception("Error Retreiving Users Token...");

		PopulateMessages();
	}

	private List<Models.Message> _sendMessages;
	private List<Models.Message> _recMessages;
	private List<Models.Message> _messages;


	private async void PopulateMessages()
    {
		_messages = new List<Models.Message>();
;
		string contactToken = _contact.Token;

		_sendMessages = await _appManager.MessageSenseData.Messages.Where(m => m.SenderToken == _myToken && m.RecipientToken == contactToken).ToListAsync();
		_recMessages = await _appManager.MessageSenseData.Messages.Where(m => m.SenderToken == contactToken && m.RecipientToken == _myToken).ToListAsync();

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
			var lbl = GenMsgLbl(msg, _myToken);
			msgStack.Add(lbl);
        }
	}

	private static Label GenMsgLbl(Models.Message msg, string token)
    {
		var lbl = new Label()
		{
			Text = msg.Data,
			TextColor = Colors.White,
			BackgroundColor = Colors.DarkGoldenrod,
			Padding = new Thickness(5, 4, 5, 4),
			FontSize = 14,
			HorizontalTextAlignment = TextAlignment.Start,
			VerticalTextAlignment = TextAlignment.Start,
		};
		if (msg.SenderToken != token)
        {
			lbl.BackgroundColor = Color.FromArgb("#1d66db");
        }

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