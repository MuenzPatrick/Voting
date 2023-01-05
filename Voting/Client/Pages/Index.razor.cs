using System.Dynamic;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Research.SEAL;
using Voting.Contracts;
using Voting.Shared;

namespace Voting.Client.Pages
{
    public partial class Index
    {
        public bool IsInitiator { get; set; } = true;
        public bool IsRoleSet { get; set; }
        public bool IsAbstimmungStarted { get; set; }

        public bool IsAbstimmungFinalized { get; set; }

        private HubConnection? hubConnection;

        private Model model = new();

        private string[] options = new string[] {"yes", "no"};

        private PublicKey? publicKey;

        private Guid userId = new();

        protected override async Task OnInitializedAsync()
        {
            //SignalR
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/voting"))
                .Build();
            SetSignalREvents();
            await hubConnection.StartAsync();
            await hubConnection.SendAsync("GetVotingStatus");
        }

        private void SetSignalREvents()
        {
            hubConnection.On("StartVoting", () =>
            {
                IsAbstimmungStarted = true;
                StateHasChanged();
            });

            hubConnection.On("FinalizeVoting", () =>
            {
                IsAbstimmungFinalized = true;
                StateHasChanged();
            });
            hubConnection.On<bool, bool>("GetVotingStatus", (started, stopped) =>
            {
                IsAbstimmungStarted = started;
                IsAbstimmungFinalized = stopped;
                StateHasChanged();
            });

            hubConnection.On<PublicKey>("GetPublicKey", (pk) =>
            {
                publicKey = pk;
            });
        }
        public async Task StartAbstimmung()
        {
            await hubConnection.SendAsync("StartVoting");
        }

        public async Task Submit()
        {
            var stimme = new Stimmzettel();
            var SealManager = new SealManager();
            List<ulong> values;
            await hubConnection.SendAsync("GetPublicKey");
            if (model.Selection.Equals("yes"))
            {
                values = new List<ulong>() { 1, 0 };
                stimme.Abstimmungen = new List<Ciphertext>()
                {
                    SealManager.Encrypt(1, publicKey!),
                    SealManager.Encrypt(0, publicKey!)
                };
            }
            else
            {
                values = new List<ulong>() { 0,1 };
                stimme.Abstimmungen = new List<Ciphertext>()
                {
                    SealManager.Encrypt(0, publicKey!),
                    SealManager.Encrypt(1, publicKey!)
                };
            }

            stimme.SumAbstimmungen = SealManager.AddCiphers(stimme.Abstimmungen);
            stimme.AbstimmungsVektor = SealManager.Encrypt(values, publicKey!);
            await hubConnection.SendAsync("Abstimmung", userId, stimme);
        }
    }

    public class Model
    {
        public string Selection { get; set; }
    }
}
