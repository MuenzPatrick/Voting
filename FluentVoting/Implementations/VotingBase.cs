using System.Reflection.Metadata.Ecma335;
using System.Transactions;
using FluentVoting.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Research.SEAL;
using Voting.Contracts;
using Voting.Shared;

namespace FluentVoting.Implementations
{
    public class VotingBase : IVotingBase
    {
        public HubConnection HubConnection { get; private set;  }

        public event Action? OnAbstimmungStartedEvent;

        public event Action? OnAbstimmungStoppedEvent;

        public event Action? OnAbstimmungDoneEvent;

        public event Action<bool, bool>? OnAbstimmungStatusChanged;

        private Microsoft.Research.SEAL.PublicKey publicKey = new();

        public async Task<IVotingBase> Connect()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7136/voting")
                .Build();
            await HubConnection.StartAsync();
            BuildListener();
            return this;
        }

        private void BuildListener()
        {
            HubConnection.On("StartVoting", () => { OnAbstimmungStartedEvent?.Invoke(); });
            HubConnection.On("StopVoting", () => { OnAbstimmungStoppedEvent?.Invoke(); });

            HubConnection.On("AbstimmungDone", () => { OnAbstimmungDoneEvent?.Invoke(); });

            HubConnection.On<bool, bool>("GetVotingStatus",
                (started, stopped) => { OnAbstimmungStatusChanged?.Invoke(started, stopped); });
        }

        public async Task<IVotingBase> Start()
        {
            await HubConnection.SendAsync("StartVoting");
            return this;
        }

        public async Task<IVotingBase> Stop()
        {
            await HubConnection.SendAsync("StopVoting");
            return this;
        }

        public async Task<IVotingBase> Vote(ulong[] voting, int userId)
        {

            HubConnection.On<Microsoft.Research.SEAL.PublicKey>("GetPublicKey", (pk) =>
            {
                publicKey = pk;
            });

            await HubConnection.SendAsync("GetPublicKey");
            var SealManager = new SealManager();
            var stimmZettel = new Stimmzettel();
            
            stimmZettel.Abstimmungen = new List<Ciphertext>()
            {
                SealManager.Encrypt((int)voting[0], publicKey),
                SealManager.Encrypt((int)voting[1], publicKey)
            };

            stimmZettel.SumAbstimmungen = SealManager.AddCiphers(stimmZettel.Abstimmungen);
            stimmZettel.AbstimmungsVektor = SealManager.Encrypt(voting, publicKey);

            await HubConnection.SendAsync("Abstimmung", userId, stimmZettel);

            return this;
        }
    }
}
