using System.Security.Principal;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "wtf";

		public static int BetRequest(JObject gameState)
		{
            var state = gameState.ToObject<MainState>();

		    if (state.community_cards.Count == 0)
		        return PreFlopStrategy(state);
            else if (state.community_cards.Count == 3)
                return FlopStrategy(state);

            return 0;
		}

	    private static int FlopStrategy(MainState state)
	    {
	        var player = state.players[state.in_action];
	        var card1 = player.hole_cards[0];
	        var card2 = player.hole_cards[1];

	        for (int i = 0; i < 3; i++)
	        {
	            if (card1.rank.Equals(state.community_cards[i].rank))
	                return 10000;
	            if (card2.rank.Equals(state.community_cards[i].rank))
	                return 10000;
	        }
	        return 0;
	    }


	    private static bool GoodHoleCards(HoleCard card1, HoleCard card2)
	    {
            var equalCards = card1.rank == card2.rank;
            var card1High = RankToValue.Convert(card1.rank) >= 10;
            var card2High = RankToValue.Convert(card2.rank) >= 10;
            var card1Ace = RankToValue.Convert(card1.rank) >= 14;
            var card2Ace = RankToValue.Convert(card2.rank) >= 14;

            if (equalCards ||
                card1Ace && card2High ||
                card2Ace && card1High)
                return true;
	        return false;
	    }

        private static bool MediumHoleCards(HoleCard card1, HoleCard card2)
        {
            var equalCards = card1.rank == card2.rank;
            var card1High = RankToValue.Convert(card1.rank) >= 10;
            var card2High = RankToValue.Convert(card2.rank) >= 10;

            if (equalCards ||
                (card1High && card2High))
                return true;
            return false;
        }

        private static int PreFlopStrategy(MainState state)
	    {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];

	        if ((float)state.small_blind/(float)player.stack > (1/7f))
	            return 10000;
            else if (state.small_blind > 60)
            {
                if (GoodHoleCards(card1, card2) ||
                    MediumHoleCards(card1, card2))
                    return 10000;
            }
            else if (state.small_blind > 30)
            {
                if (GoodHoleCards(card1, card2))
                    return 10000;
            }
            else if (state.current_buy_in <= state.small_blind * 4)
            {
                if (GoodHoleCards(card1, card2))
                    return state.current_buy_in;
            }
            else if (state.current_buy_in <= state.small_blind * 2)
            {
                if (MediumHoleCards(card1, card2))
                    return state.current_buy_in;
            }

            return 0;
        }

		public static void ShowDown(JObject gameState)
		{
			//TODO: Use this method to showdown
		}
	}
}

