using Microsoft.Research.SEAL;
using FluentVoting.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Voting.Contracts;
using Voting.Shared;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FluentVoting.Implementations
{
    public class VotingBase : IVotingBase
    {
        public HubConnection HubConnection { get; private set;  }

        public bool IsStarted => this.HubConnection.State == HubConnectionState.Connected;

        public event Action? OnAbstimmungStartedEvent;

        public event Action? OnAbstimmungStoppedEvent;

        public event Action? OnAbstimmungDoneEvent;

        public event Action<bool, bool>? OnAbstimmungStatusChanged;

        private Microsoft.Research.SEAL.PublicKey? publicKey;

        public async Task<IVotingBase> Connect()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7136/voting")
                .AddNewtonsoftJsonProtocol()
                .Build();
            await HubConnection.StartAsync();
            BuildListener();
            return this;
        }

        private void BuildListener()
        {
            HubConnection.On("StartVoting", () =>
            {
                OnAbstimmungStartedEvent?.Invoke();
            });
            HubConnection.On("StopVoting", () => { OnAbstimmungStoppedEvent?.Invoke(); });

            HubConnection.On("AbstimmungDone", () => { OnAbstimmungDoneEvent?.Invoke(); });

            HubConnection.On<bool, bool>("GetVotingStatus",
                (started, stopped) => { OnAbstimmungStatusChanged?.Invoke(started, stopped); });

            HubConnection.On<byte[]>("GetPublicKey", (pk) =>
            {
                //publicKey = pk;
                //JsonSerializer.Deserialize<PublicKey>(pk);
                //string test = pk.data.ToString();
                //var xy = new PublicKey();
                //xy.lo
                var stream = new MemoryStream(pk);
                var publicKey = new PublicKey();
                var sm = new SealManager();
                publicKey.Load(sm.Context, stream);
                //publicKey.Load(null, (MemoryStream)pk);
                //var pub = JsonConvert.DeserializeObject<PublicKey>(test);
                //Console.WriteLine(pk);
                //publicKey = new Microsoft.Research.SEAL.PublicKey((PublicKey)pk);
            });
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

            await HubConnection.SendAsync("GetPublicKey");
            while (publicKey is null) ;
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
