using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;

namespace MessageSense;

public partial class ContactsPage : ContentPage
{
	private AppManager _appManager;

	public ContactsPage(AppManager appManager)
	{
		InitializeComponent();
		_appManager = appManager;
		PopulateContacts();

		string myToken = Preferences.Get("contactToken", "");
		contactToken.Text = $"Contact Token:\n {myToken.ToString()}";
	}

	private List<Models.Contact> _contacts;
	private DateTime _pressed;

	private async void PopulateContacts()
    {
		contactStack.Clear();
		_contacts = await _appManager.MessageSenseData.Contacts.ToListAsync();
		if (_contacts != null && _contacts.Count > 0) {
			foreach (var contact in _contacts)	{
				var btn = GenerateContactButton(contact);
				contactStack.Add(btn);
			}
		}
    }

	private Button GenerateContactButton(Models.Contact contact)
    {
		Button button = new Button() {
			Text = contact.Name,
			FontAttributes = FontAttributes.Bold,
			FontSize = 20,
			TextColor = Colors.Black,
			BindingContext = contact.Id.ToString(),
			BackgroundColor = Colors.DarkGoldenrod,
			//BorderColor = Color.FromArgb("#1d66db"),
			Margin = new Thickness(1),
			Padding = new Thickness(15),
			VerticalOptions = LayoutOptions.FillAndExpand,
			HorizontalOptions = LayoutOptions.FillAndExpand,
		};
		// button.Clicked += ContactButtonClicked;
		button.Pressed += ContactButtonPressed;
		button.Released += ContactButtonReleasedAsync;
		return button;
	}
	private void ContactButtonPressed(object sender, EventArgs e)
    {
		_pressed = DateTime.Now;
    }
	private async void ContactButtonReleasedAsync(object sender, EventArgs e)
    {
		var contactId = ((Button)sender).BindingContext as string;
		var cid = Convert.ToInt32(contactId);
		var contact = await _appManager.MessageSenseData.Contacts.FirstOrDefaultAsync(m => m.Id == cid);

		var duration = DateTime.Now - _pressed;

		if (duration.TotalMilliseconds > 1000) {
			var result = await DisplayAlert("Delete Contact", "Are you sure?", accept: "Yes", cancel: "No");
			if (result) {	
				_appManager.MessageSenseData.Contacts.Remove(contact);
				await _appManager.MessageSenseData.SaveChangesAsync();
				PopulateContacts();
            } else { return; }
        } else {
			App.Current.MainPage = new MessagePage(_appManager, contact);
		}
    }

	private async void ContactButtonClicked(object sender, EventArgs e)
    {
		string contactId = ((Button)sender).BindingContext as string;
		int cid = Convert.ToInt32(contactId);
		var contact = await _appManager.MessageSenseData.Contacts.FirstOrDefaultAsync(m => m.Id == cid);
		App.Current.MainPage = new MessagePage(_appManager, contact);

    }

	private async void AddContact(object sender, EventArgs e)
    {
		string username = await DisplayPromptAsync("New Contact", "Username:");
		string token = await DisplayPromptAsync("New Contact", "Contact Token:");

		MessageSense.Models.Contact contact = new MessageSense.Models.Contact()
		{
			Name = username,
			Token = token
		};

		_appManager.MessageSenseData.Contacts.Add(contact);
		await _appManager.MessageSenseData.SaveChangesAsync();

		PopulateContacts();
    }
}