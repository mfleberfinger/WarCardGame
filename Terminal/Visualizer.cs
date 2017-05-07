using System;
using System.Collections.Generic;

public class Visualizer
{

	//set up visuals showing no cards on table and two full decks
	public void NewGame()
	{
		Console.WriteLine("New Game Visuals Here.");
	}

	//shows card leaving appropriate deck and being placed on table
	//if card not supplied, the new card is face down
	public void AddCard(int player, Card card)
	{
		Console.WriteLine(card.val + " of " + card.suit + 
			" staked by player " + player + ".");
	}
	public void AddCard(int player)
	{
		Console.WriteLine("Facedown card staked by player " + player + ".");
	}

	//removes cards from the given risk stack and moves them to the given deck
	//displays the cards given in the Queue as cards are moved
	//cards queue must have no more items than the number of cards on player's side
	//risk and deck refer to the side of the table the cards are taken from
	//	and the deck the cards are placed in, respectively
	public void RemoveCards(int risk, int deck, Stack<Card> cards)
	{
		Console.WriteLine("Player " + deck + " took " + cards.Count + 
			" cards from player " + risk);
	}


	//outputs "player [player] wins" or similar or "Tie" depending on argument
	public void Win(int player)
	{
		if(player > 0)
			Console.WriteLine("Player " + player + " wins!");
		else
			Console.WriteLine("Tie!");
	}
}
