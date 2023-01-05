using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Voting.Server;
using Voting.Shared;
using System.Dynamic;
using Microsoft.AspNetCore.SignalR.Protocol;
using Voting.Contracts;
using IClientProxy = Microsoft.AspNetCore.SignalR.IClientProxy;

namespace VotingTests
{
    [TestClass]
    public class HubTests
    {
        public VotingHub VotingHub { get; set; }

        public SealManager SealManager { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            var votingStatusManager = new VotingStatusManager();
            VotingHub = new VotingHub(votingStatusManager);
            var mockClients = new Mock<IHubCallerClients>();
            VotingHub.Clients = mockClients.Object;
            var all = new Mock<IClientProxy>();
            //all.Setup(a => a.SendCoreAsync(It.IsAny<String>(), It.IsAny<object?[]>())).Returns();
            //all.broadcastMessage = new Action<string, string>((name, message) => {
            //});
            mockClients.Setup(m => m.All).Returns(all.Object);
            SealManager = votingStatusManager.SealManager;
        }
        [TestMethod]
        public async Task StartAbstimmung()
        {
            await VotingHub.StartVoting();
        }

        [TestMethod]
        public async Task AbstimmenBeforeStartFalse()
        {
            var stimme = new Stimmzettel();
            stimme.Abstimmungen.Add(SealManager.Encrypt(0));
            stimme.Abstimmungen.Add(SealManager.Encrypt(1));
            stimme.SumAbstimmungen = SealManager.AddCiphers(stimme.Abstimmungen);
            stimme.AbstimmungsVektor = SealManager.Encrypt(new List<ulong> { 0, 1 });
            Assert.AreEqual(false, await VotingHub.Abstimmung(new Guid(), stimme));
        }

        [TestMethod]
        public async Task AbstimmennNachStart()
        {
            await VotingHub.StartVoting();
            var stimme = new Stimmzettel();
            stimme.Abstimmungen.Add(SealManager.Encrypt(0));
            stimme.Abstimmungen.Add(SealManager.Encrypt(1));
            stimme.SumAbstimmungen = SealManager.AddCiphers(stimme.Abstimmungen);
            stimme.AbstimmungsVektor = SealManager.Encrypt(new List<ulong> { 0, 1 });
            Assert.AreEqual(true, await VotingHub.Abstimmung(new Guid(), stimme));
         }

        [TestMethod]
        public async Task AbstimmenMultiple()
        {
            await VotingHub.StartVoting();
            var stimme = new Stimmzettel();
            stimme.Abstimmungen.Add(SealManager.Encrypt(0));
            stimme.Abstimmungen.Add(SealManager.Encrypt(1));
            stimme.SumAbstimmungen = SealManager.AddCiphers(stimme.Abstimmungen);
            stimme.AbstimmungsVektor = SealManager.Encrypt(new List<ulong> { 0, 1 });
            var user = new Guid();
            Assert.AreEqual(true, await VotingHub.Abstimmung(user, stimme));
            Assert.AreEqual(false, await VotingHub.Abstimmung(user, stimme));
        }
    }
}
