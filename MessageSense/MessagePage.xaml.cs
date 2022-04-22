using Microsoft.EntityFrameworkCore;
using MessageSense.ClientNet;

namespace MessageSense;

public partial class MessagePage : ContentPage
{
	private AppManager _appManager;
	private Models.Contact _contact;

	public bool isSending;

	public MessagePage(AppManager appManager, Models.Contact contact)
	{
		InitializeComponent();
		_appManager = appManager;
		_contact = contact;
		ContactName.Text = contact.Name;
		PopulateMessages();
		Task.Run(() => BackgroundRefreshThread());
		isSending = false;
	}

	private async void BackgroundRefreshThread()
    {
		while (true)
        {
			await Task.Delay(1000);
			if (!isSending) {
				try {
					App.Current.Dispatcher.Dispatch(CheckForNewMessages);
				} catch (Exception e) {
					await DisplayAlert("Network Error", "Unable to check for new messages", "Ok");
					break;
				}
			}
			// End Loop
        }
    }

	private List<Message> _sendMessages;
	private List<Message> _recMessages;
	private List<Message> _messages;

	private async void PopulateMessages()
    {
		_messages = new List<Message>();

		string myToken = _appManager.AppUser.ContactToken;
		if (myToken == null || myToken == "0") throw new Exception("Error Retreiving Users Token...");
		string contactToken = _contact.Token;

		_messages = await _appManager.MessageSenseData.Messages.Where(m => m.SenderToken == contactToken || m.RecipientToken == contactToken).ToListAsync();

		msgStack.Clear();

		_messages = _messages.OrderBy(m => m.DateTime).ToList();
		foreach (var msg in _messages)
		{
			var lbl = GenMsgLbl(msg, myToken);
			msgStack.Add(lbl);
		}
	}
	
	private async void CheckForNewMessages()
    {
		var count = await _contact.SendPullMessageRequest(_appManager.AppUser);

		if (count > 0)
        {
			PopulateMessages();
        }
    }

	private async void OnSendMsg(object sender, EventArgs e)
    {
		var msgData = msgEntry.Text;
		if (msgData.Length == 0) return;

		Message msg = new Message() {
			Data = msgData,
			DateTime = DateTime.Now,
			SenderToken = _appManager.AppUser.ContactToken,
			RecipientToken = _contact.Token
		};

        _appManager.MessageSenseData.Messages.Add(msg);
		_appManager.MessageSenseData.SaveChanges();
		isSending = true;
		await msg.SendStoreMessageRequest(_appManager.AppUser);
		isSending = false;
		PopulateMessages();

    }

	private void OnGoBack(object sender, EventArgs e)
    {
		Application.Current.MainPage = new ContactsPage(_appManager);
    }


	private static Label GenMsgLbl(Message msg, string token)
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



}