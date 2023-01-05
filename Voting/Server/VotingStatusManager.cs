using Voting.Contracts;
using Voting.Shared;

namespace Voting.Server;

public class VotingStatusManager
{
    public LinkedList<Stimmzettel> StimmzettelList { get; set; } = new();

    public HashSet<Guid> AbgegebeneStimmen { get; set; } = new();

    public SealManager SealManager { get; set; } = new();

    public bool IsVotingStarted { get; set; }
    public bool IsVotingSealed { get; set; }

    public VotingStatusManager()
    {

    }
}
