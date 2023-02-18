using System.Reflection.Metadata.Ecma335;
using FluentVoting.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Voting.Shared;

namespace FluentVoting.Implementations
{
    public class VotingBase : IVotingBase
    {
        public HubConnection HubConnection { get; }
        public VotingBase(HubConnection hc)
        {
            HubConnection = hc;
        }
        public async Task<VotingStatus> Status()
        {
            if(HubConnection.State != HubConnectionState.Connected) return new VotingStatus();

            var votingStatus = new VotingStatus();
            
            HubConnection.On<VotingStatus>("GetVotingStatus", (vs) =>
            {
                votingStatus = vs;
            });
            
            await HubConnection.SendAsync("GetVotingStatus");
            
            return votingStatus;
        }
    }
}
