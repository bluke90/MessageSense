using Microsoft.EntityFrameworkCore;
using MessageSense.ClientNet;
using System.Collections.ObjectModel;

namespace MessageSense;

public partial class MessagePage : ContentPage
{
	private AppManager _appManager;
	private Models.Contact _contact;

	public bool isSending;


	public ObservableCollection<Message> _messages;
	public Queue<Message> _msgQueue;

	private Thread _refreshThread;

	private Data.MessageSenseData _context;

	public MessagePage(AppManager appManager, Models.Contact contact)
	{
		InitializeComponent();
		_context = new Data.MessageSenseData();
		_appManager = appManager;
		_contact = contact;
		ContactName.Text = contact.Name;
		_messages = new ObservableCollection<Message>();
		_msgQueue = new Queue<Message>();
		_messages.CollectionChanged += PopulateQueuedMessage;
		_refreshThread = new Thread(() => BackgroundRefreshThread());
		_refreshThread.Start();
		isSending = false;
		// ClearMessages(contact, _context);
	}

	private async void BackgroundRefreshThread()
    {
		var contactToken = _contact.Token;
		try
		{
			var data = new Data.MessageSenseData();
			while (true)
			{
				foreach (var message in await data.Messages
					.Where(m => m.SenderToken == contactToken || m.RecipientToken == contactToken)
					.OrderBy(m => m.DateTime).ToListAsync()) {

					if (!_messages.Contains(message))
					{
						_msgQueue.Enqueue(message);
						_messages.Add(message);
					}
				}
				Thread.Sleep(1000);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			Thread.Sleep(1000);
			BackgroundRefreshThread();
		}
    }

	private void PopulateQueuedMessage(object sender, EventArgs args)
    {
		for (int i = 0; i <= _msgQueue.Count; i++)
        {
			var msg = _msgQueue.Dequeue(); // should peek and dequeue only if success
			App.Current.Dispatcher.Dispatch(() => PopulateNewMessage(msg));
        }
    }


	private void PopulateNewMessage(Message msg)
    {
		var lbl = GenMsgLbl(msg, _appManager.AppUser.ContactToken);
		msgStack.Add(lbl);
    }
	

	private void OnSendMsg(object sender, EventArgs e)
    {
		try
		{
			var msgData = msgEntry.Text;
			if (msgData.Length == 0) return;

			Message msg = new Message()
			{
				Data = msgData,
				DateTime = DateTime.Now,
				SenderToken = _appManager.AppUser.ContactToken,
				RecipientToken = _contact.Token
			};

			_context.Messages.Add(msg);
			_context.SaveChanges();

			Task.Run(() => msg.SendStoreMessageRequest(_appManager));
		}catch (Exception ex)
        {
			Console.WriteLine("Exception Location Details: Method => OnSendMsg | File => MessagePage.Xaml.cs | Line => 84");
			Console.WriteLine(ex.ToString());
        }


	}

	private void OnGoBack(object sender, EventArgs e)
    {
		_refreshThread = null;
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

	private static async void ClearMessages(Models.Contact contact, Data.MessageSenseData context)
    {
		var _messages = await context.Messages.Where(m => m.SenderToken == contact.Token || m.RecipientToken == contact.Token).ToListAsync();
		foreach (var msg in _messages)
        {
			context.Messages.Remove(msg);
        }
		await context.SaveChangesAsync();
    }


}