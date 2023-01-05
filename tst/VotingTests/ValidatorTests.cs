using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.Contracts;
using Voting.Server;
using Voting.Shared;

namespace VotingTests
{
    [TestClass]
    public class ValidatorTests
    {
        public VotingHub VotingHub { get; set; }
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
        }
        [TestMethod]
        public void ValidateEmptyList()
        {
            Assert.AreEqual(true, Validator.ValidateList(new LinkedList<Stimmzettel>()));
        }

        [TestMethod]
        public async Task ValidateListWith1Item()
        {
            var SealManager = new SealManager();

            await VotingHub.StartVoting();
            var stimme = new Stimmzettel();
            stimme.Abstimmungen.Add(SealManager.Encrypt(0));
            stimme.Abstimmungen.Add(SealManager.Encrypt(1));
            stimme.SumAbstimmungen = SealManager.AddCiphers(stimme.Abstimmungen);
            stimme.AbstimmungsVektor = SealManager.Encrypt(new List<ulong> { 0, 1 });
            await VotingHub.Abstimmung(new Guid(), stimme);
            Assert.AreEqual(true, Validator.ValidateList(VotingHub.VotingStatusManager.StimmzettelList));
        }

        [TestMethod]
        public async Task ValidateListWith2Item()
        {
            var SealManager = new SealManager();
            
            await VotingHub.StartVoting();
            var stimme = new Stimmzettel();
            stimme.Abstimmungen.Add(SealManager.Encrypt(0));
            stimme.Abstimmungen.Add(SealManager.Encrypt(1));
            stimme.SumAbstimmungen = SealManager.AddCiphers(stimme.Abstimmungen);
            stimme.AbstimmungsVektor = SealManager.Encrypt(new List<ulong> { 0, 1 });
            await VotingHub.Abstimmung(new Guid(), stimme);
            await VotingHub.Abstimmung(new Guid(), stimme);
            Assert.AreEqual(true, Validator.ValidateList(VotingHub.VotingStatusManager.StimmzettelList));
        }

        [TestMethod]
        public async Task ValidateInvalidList()
        {
            Assert.Fail();
        }
    }
}
