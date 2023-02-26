using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Research.SEAL;
using Voting.Contracts;
using Voting.Shared;

namespace Voting.Server;

public class VotingHub : Hub
{
    public VotingStatusManager VotingStatusManager { get; set; }
    public VotingHub(VotingStatusManager votingStatusManager)
    {
        VotingStatusManager = votingStatusManager;
    }
    public async Task<bool> Abstimmung(Guid userId, Stimmzettel voting)
    {
        if (!VotingStatusManager.IsVotingStarted || VotingStatusManager.IsVotingSealed) return false;
        if (VotingStatusManager.AbgegebeneStimmen.Contains(userId)) return false;

        var lastStimmmzettel = VotingStatusManager.StimmzettelList.Any() ? VotingStatusManager.StimmzettelList.Last() : new Stimmzettel();// ?? new Stimmzettel();

        voting.Hash = HashingManager.HashStimmmzettel(voting, lastStimmmzettel);

        VotingStatusManager.StimmzettelList.AddLast(voting);
        VotingStatusManager.AbgegebeneStimmen.Add(userId);
        await Clients.Caller.SendAsync("AbstimmungDone");
        return true;
    }

    public async Task StartVoting()
    {
        if (VotingStatusManager.IsVotingSealed) return;
        VotingStatusManager.IsVotingStarted = true;
        await Clients.All.SendAsync("StartVoting");
    }

    public async Task FinalizeVoting()
    {
        VotingStatusManager.IsVotingStarted = true;
        VotingStatusManager.IsVotingSealed = true;
        await Clients.All.SendAsync("FinalizeVoting");
    }

    public async Task GetVotingStatus()
    {
        var VotingStatus = new VotingStatus()
        {
            Started = VotingStatusManager.IsVotingStarted,
            Finalized = VotingStatusManager.IsVotingSealed
        };
        await Clients.Caller.SendAsync("GetVotingStatus", VotingStatus);
    }

    public async Task GetPublicKey()
    {
        //Stream stream = new MemoryStream();
        Console.WriteLine("GetPublicKey");
        //byte[] b;
        //var save = VotingStatusManager.SealManager.PublicKey.Save(stream);
        //using BinaryReader br = new BinaryReader(stream);
        //b = br.ReadBytes((int)stream.Length);
        PublicKey obj = VotingStatusManager.SealManager.PublicKey;
        MemoryStream stream = new MemoryStream();
        obj.Save(stream);
        var arr = stream.ToArray();
        await Clients.Caller.SendAsync("GetPublicKey", arr);
    }
}