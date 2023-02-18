namespace FluentVoting.Interfaces
{
    public interface IVotingNotStarted : IVotingBase
    {
        public IVotingStarted Start();
    }
}
