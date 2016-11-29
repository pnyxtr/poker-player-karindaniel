using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "A little better player";

		public static int BetRequest(JObject gameState)
		{
            var state = gameState.ToObject<MainState>();
            var player = state.players[state.in_action];

            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];


		    if (state.current_buy_in > 200)
		    {
		        var equalCards = card1.rank == card2.rank;
		        var card1High = RankToValue.Convert(card1.rank) >= 10;
                var card2High = RankToValue.Convert(card2.rank) >= 10;
		        var card1Ace = RankToValue.Convert(card1.rank) >= 14;
                var card2Ace = RankToValue.Convert(card2.rank) >= 14;

                if (equalCards ||
                    card1Ace && card2High ||
                    card2Ace && card1High)
		            return 10000;                
		    }

            return 0;
		}

		public static void ShowDown(JObject gameState)
		{
			//TODO: Use this method to showdown
		}
	}
}

