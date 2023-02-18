using Microsoft.AspNetCore.SignalR.Client;

namespace Voting.App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private HubConnection? hubConnection;

        public MainPage()
        {
            InitializeComponent();
            GetRolePickerItems();
            GetChoicesCollection();
            StartSignalRConnection();
        }

        private void GetRolePickerItems()
        {
            var items = new List<string>() { "Initiator", "User" };
            RolePicker.ItemsSource = items;
            RolePicker.SelectedIndex= 0;
        }

        private void GetChoicesCollection()
        {
            var choices = new List<string>() { "Yes", "No" };
            ChoicesCollection.ItemsSource = choices;
        }
        private async void OnButtonRoleSubmitClicked(object sender, EventArgs e)
        {
            sl_Choices.IsVisible = true;
        }

        private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StartSignalRConnection()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7136/voting")
                .Build();
        }
    }
}