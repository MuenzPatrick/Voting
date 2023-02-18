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
        public Task<VotingStatus> Status();
    }
}
