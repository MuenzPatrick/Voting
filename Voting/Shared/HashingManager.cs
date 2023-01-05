using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.Contracts;

namespace Voting.Shared
{
    public static class HashingManager
    {
        public static int HashStimmmzettel(Stimmzettel currentStimmzettel, Stimmzettel lastStimmmzettel)
        {
            HashCode hs = new HashCode();
            foreach (var abstimmung in currentStimmzettel.Abstimmungen)
            {
                hs.Add(abstimmung);
            }
             hs.Add(currentStimmzettel.SumAbstimmungen);
             hs.Add(currentStimmzettel.AbstimmungsVektor);
            if (lastStimmmzettel.Hash > 0) hs.Add(lastStimmmzettel.Hash);
            return hs.ToHashCode();
        }
    }
}
