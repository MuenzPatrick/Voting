using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.Contracts;

namespace Voting.Shared
{
    public static class Validator
    {
        public static bool ValidateList(LinkedList<Stimmzettel> list)
        {
            if (!list.Any()) return true;
            for (int i = 0; i < list.Count(); i++)
            {
                var currStimmzettel = list.ElementAt(i);
                var prevStimmzettel = i == 0 ? new Stimmzettel() : list.ElementAt(i-1);
                var hash = HashingManager.HashStimmmzettel(currStimmzettel, prevStimmzettel);
                if (hash != currStimmzettel.Hash) return false;
            }
            return true;
        }
    }
}
