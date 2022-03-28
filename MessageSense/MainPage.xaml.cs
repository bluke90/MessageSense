using Microsoft.Maui.Essentials;

namespace MessageSense;
public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void CheckForProfile()
    {
		var user = Microsoft.Maui.Essentials.Preferences.Get("username", null);
		var fName = Microsoft.Maui.Essentials.Preferences.Get("firstName", null);
		var lName = Microsoft.Maui.Essentials.Preferences.Get("lastName", null);

		if(user == null || fName == null || lName == null)
        {
			Application.Current.MainPage = new SetupPage();
        }
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;
		CounterLabel.Text = $"Current count: {count}";

		SemanticScreenReader.Announce(CounterLabel.Text);
	}
}

