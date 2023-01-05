using Microsoft.Research.SEAL;

namespace Voting.Contracts
{
    public class Stimmzettel
    {
        public List<Ciphertext> Abstimmungen { get; set; } = new();
        
        public Ciphertext SumAbstimmungen { get; set; }

        public Ciphertext AbstimmungsVektor { get; set; }

        public int Hash { get; set; }
    }
}
