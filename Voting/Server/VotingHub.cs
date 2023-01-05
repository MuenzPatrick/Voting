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
        await Clients.Caller.SendAsync("GetVotingStatus", VotingStatusManager.IsVotingStarted, VotingStatusManager.IsVotingSealed);
    }

    public async Task GetPublicKey()
    {
        await Clients.Caller.SendAsync("GetPublicKey", VotingStatusManager.SealManager.PublicKey);
    }
}