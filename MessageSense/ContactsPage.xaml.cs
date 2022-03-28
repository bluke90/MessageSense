using Microsoft.EntityFrameworkCore;

namespace MessageSense;

public partial class ContactsPage : ContentPage
{
	private AppManager _appManager;

	public ContactsPage(AppManager appManager)
	{
		InitializeComponent();
		_appManager = appManager;
	}

	private List<Models.Contact> _contacts;

	private async void PopulateContacts()
    {
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
		Button button = new Button()
		{
			Text = contact.Name,
			TextColor = Colors.SeaShell,
			BindingContext = contact.Id.ToString(),
			BackgroundColor = Color.FromArgb("#080808"),
			BorderColor = Colors.MidnightBlue,
			BorderWidth = 1,
			VerticalOptions = LayoutOptions.FillAndExpand,
			HorizontalOptions = LayoutOptions.FillAndExpand
		};
		button.Clicked += ContactButtonClicked;
		return button;
	}

	private void ContactButtonClicked(object sender, EventArgs e)
    {
		string contactId = ((Button)sender).BindingContext as string;
		//App.Current.MainPage = new MsgPage(_appManager, contactId);
		DisplayAlert("Contact", $"You chose contact: {contactId}", "Okay");

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