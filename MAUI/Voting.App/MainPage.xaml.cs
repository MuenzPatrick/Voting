using FluentVoting.Implementations;
using FluentVoting.Interfaces;

namespace Voting.App
{
    public partial class MainPage : ContentPage
    {
        private IVotingBase voting;

        private bool IsInitiator;

        private ulong[] votingSelection = new ulong[2];

        private int UserId { get; set; }

        public MainPage()
        {
            voting = new VotingBase();
            InitializeComponent();
            GetRolePickerItems();
            GetChoicesCollection();
            SetVotingEventListener();
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
            RolePicker.IsVisible = false;
            b_SendRole.IsVisible = false;
            sl_Choices.IsVisible = false;
            if (RolePicker.SelectedIndex == 0) IsInitiator = true;
            UserId = new Random().Next();
            if(IsInitiator) ShowVotingStart();
            await voting.Connect();
        }

        private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            votingSelection = e.CurrentSelection.ToString() switch
            {
                "Yes" => new ulong[]{ 1, 0 },
                "No" => new ulong[] { 0, 1 },
                _ => new ulong[] { 0, 0 }
            };
        }

        private void SetVotingEventListener()
        {
            voting.OnAbstimmungDoneEvent += () => { };
            voting.OnAbstimmungStartedEvent += () => { };
            voting.OnAbstimmungStatusChanged += (started, stopped) => { };
            voting.OnAbstimmungStoppedEvent += () => { };
        }

        private void ShowVotingStart()
        {
            b_StartVoting.IsVisible = true;
        }

        private async void OnButtonStartVotingClicked(object sender, EventArgs e)
        {
            await voting.Start();
            b_StartVoting.IsVisible = false;
            sl_Choices.IsVisible = true;
            b_SendChoice.IsVisible = true;
        }

        private async void OnButtonSendVotingClicked(object sender, EventArgs e)
        {
            await voting.Vote(votingSelection, UserId);
            sl_Choices.IsVisible = false;
            b_SendChoice.IsVisible = false;
        }
    }
}