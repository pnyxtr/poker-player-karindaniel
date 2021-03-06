﻿using System;
using System.Linq;
using System.Security.Principal;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "speeding away";

		public static int BetRequest(JObject gameState)
		{
            var state = gameState.ToObject<MainState>();

		    if (state.community_cards.Count == 0)
		        return PreFlopStrategy(state);
            else if (state.community_cards.Count == 3)
                return FlopStrategy(state);
            else if (state.community_cards.Count == 4)
                return TurnStrategy(state);
            else if (state.community_cards.Count == 5)
                return RiverStrategy(state);

            return 0;
		}

	    private static int FlopStrategy2Players(MainState state)
	    {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];
            if (HighPair(card1, card2))
                return state.pot;
            for (int i = 0; i < 3; i++)
            {
                if (card1.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card1.rank) >= 10)
                    return state.pot;
                if (card2.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card2.rank) >= 10)
                    return state.pot;
            }

            if (AlmostFlush(state))
                return state.pot;

            if (state.current_buy_in == 0)
                return state.minimum_raise;

            return 0;
        }

	    private static int GetBoardHighestCard(MainState state)
	    {
	        int max = 0;
	        for (int i = 0; i < state.community_cards.Count; i++)
	        {
	            var r = RankToValue.Convert(state.community_cards[i].rank);
	            if (r > max)
	            {
	                max = r;
	            }
	        }
	        return max;
        }

        private static int FlopStrategyManyPlayers(MainState state)
        {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];
            var highestCard = GetBoardHighestCard(state);
            if (HighPair(card1, card2) && RankToValue.Convert(card1.rank) >= highestCard)
                return state.pot;
            for (int i = 0; i < 3; i++)
            {
                if (card1.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card1.rank) >= highestCard)
                    return state.pot;
                if (card2.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card2.rank) >= highestCard)
                    return state.pot;
            }

            if (AlmostFlush(state))
                return state.pot;

            if (state.current_buy_in == 0)
                return state.minimum_raise;

            return 0;
        }

        private static int FlopStrategy(MainState state)
	    {
            var numActivePlayers = state.players.Count(x => x.status == "active");
	        if (numActivePlayers <= 2)
	            return FlopStrategy2Players(state);

	        return FlopStrategyManyPlayers(state);
	    }

        private static int TurnStrategy(MainState state)
        {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];
            var highestCard = GetBoardHighestCard(state);
            if (HighPair(card1, card2) && RankToValue.Convert(card1.rank) >= highestCard)
                return state.pot;
            for (int i = 0; i < 4; i++)
            {
                if (card1.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card1.rank) >= highestCard)
                    return state.pot;
                if (card2.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card2.rank) >= highestCard)
                    return state.pot;
            }
            return 0;
        }

        private static int RiverStrategy(MainState state)
        {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];
            var highestCard = GetBoardHighestCard(state);
            if (HighPair(card1, card2) && RankToValue.Convert(card1.rank) >= highestCard)
                return state.pot;
            for (int i = 0; i < 5; i++)
            {
                if (card1.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card1.rank) >= highestCard)
                    return state.pot;
                if (card2.rank.Equals(state.community_cards[i].rank) && RankToValue.Convert(card2.rank) >= highestCard)
                    return state.pot;
            }
            return 0;
        }


	    public static bool HighPair(HoleCard card1, HoleCard card2)
	    {
            var equalCards = card1.rank == card2.rank;
            return equalCards && RankToValue.Convert(card1.rank) >= 9;
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

        private static bool PremiumHoleCards(HoleCard card1, HoleCard card2)
        {
            var equalCards = card1.rank == card2.rank;
            var card1High = RankToValue.Convert(card1.rank) >= 10;
            var card2High = RankToValue.Convert(card2.rank) >= 10;
            var card1Ace = RankToValue.Convert(card1.rank) >= 14;
            var card2Ace = RankToValue.Convert(card2.rank) >= 14;

            if ((equalCards && RankToValue.Convert(card1.rank) >= 9) ||
                (card1Ace && card2High) ||
                (card2Ace && card1High))
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

	    private static bool AlmostFlush(MainState state)
	    {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];
            string[] colors = { "spades", "clubs", "hearts", "diamonds"};

            foreach (var color in colors)
            {
                int count = 0;
                if (card1.suit == color)
                    count++;
                if (card2.suit == color)
                    count++;
                if (count == 0)
                    continue;
                
                for (int i = 0; i < state.community_cards.Count; i++)
                {
                    if (state.community_cards[i].suit == color)
                        count++;
                }
                if (count >= 4)
                    return true;
            }

	        return false;
	    }

        private static int PreFlopStrategy(MainState state)
	    {
            var player = state.players[state.in_action];
            var card1 = player.hole_cards[0];
            var card2 = player.hole_cards[1];
            var numActivePlayers = state.players.Count(x => x.status == "active");
                

	        if ((float)state.small_blind/(float)player.stack > (1/7f)) // All in if stack too small
	            return 10000;
            else if (state.small_blind >= 60) // if small blind large, all in on good cards
            {
                if (GoodHoleCards(card1, card2) ||
                    MediumHoleCards(card1, card2))
                    return 10000;
            }            
            else if (numActivePlayers <= 2)
            {
                if (state.current_buy_in > state.small_blind*2)
                {
                    if (!HighPair(card1, card2))
                        return 0;
                }
                if (GoodHoleCards(card1, card2))
                    return 10000;
            }

            if (state.current_buy_in <= state.small_blind * 4)
            {
                if (GoodHoleCards(card1, card2))
                    return state.current_buy_in;
            }
            if (state.current_buy_in <= state.small_blind * 2)
            {
                if (MediumHoleCards(card1, card2))
                    return state.current_buy_in;
            }
            if (state.pot <= state.small_blind * 4) // Bluff               
            {
                if (RankToValue.Convert(card1.rank) >= 10 ||
                    RankToValue.Convert(card2.rank) >= 10)
                    return state.current_buy_in+state.minimum_raise-player.bet;
            };
            if (PremiumHoleCards(card1, card2) && player.bet >= state.small_blind*3)
                return 10000;
            return 0;
        }

		public static void ShowDown(JObject gameState)
		{
			//TODO: Use this method to showdown
		}
	}
}

