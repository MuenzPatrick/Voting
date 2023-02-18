using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentVoting.Interfaces
{
    public interface IVotingStarted : IVotingBase
    {
        public IVotingBase Stop();
        public IVotingBase Vote();

    }
}
