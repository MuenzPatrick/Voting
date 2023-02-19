using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.Shared;

namespace FluentVoting.Interfaces
{
    public interface IVotingBase
    {
        public HubConnection HubConnection { get; }

        public bool IsStarted => this.HubConnection.State == HubConnectionState.Connected;

        event Action OnAbstimmungStartedEvent;

        event Action? OnAbstimmungStoppedEvent;

        public Task<IVotingBase> Connect();
        public Task<IVotingBase> Start();

        public Task<IVotingBase> Stop();

        public Task<IVotingBase> Vote(ulong[] voting, int userId);
    }
}
