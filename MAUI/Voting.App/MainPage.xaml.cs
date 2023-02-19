using Android.Views;
using FluentVoting.Implementations;
using FluentVoting.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Voting.App
{
    public partial class MainPage : ContentPage
    {
        private IVotingBase voting;

        private bool IsInitiator;

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
            sl_Choices.IsVisible = true;
            if (RolePicker.SelectedIndex == 0) IsInitiator = true;
            if(IsInitiator) ShowVotingStart();
            await voting.Connect();
        }

        private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
        }
    }
}