using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Cards
{
    public interface CardHandlerBase
    {
        List<CardBase> GetStartingDeck();
        List<CardBase> GetRewardPool(int floor);
    }
}
