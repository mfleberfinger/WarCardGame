using System;
using System.Collections.Generic;
using DeacomWarVisual;

public class Game
{
	//game state variables
	private bool war;
	private Card[] initialDeck; //defines a randomly ordered deck of 52 cards
	private Queue<Card> deck1, deck2; //hold each player's cards
	private Stack<Card> risk1, risk2; //cards on the table during a round
	private MainWindow visualizer; //object to provide output for the user
    private bool gameOver;

	public Game(MainWindow mainWindow)
	{
		//create the deck
		initialDeck = new Card[52];
		for(int i = 0; i < 4; i++)
			for(int j = 2; j < 15; j++)
			{
				//"ij" is effectively base 13 shifted by 2
				initialDeck[i*13+j-2] = new Card();
				initialDeck[i*13+j-2].val = j;
				initialDeck[i*13+j-2].suit = ((Suit)i);
			}

		visualizer = mainWindow;
		NewGame();
	}

	//reset game state variables, shuffle the deck, split the deck
	public void NewGame()
	{
        gameOver = false;
        war = false;
		deck1 = new Queue<Card>();
		deck2 = new Queue<Card>();
		risk1 = new Stack<Card>();
		risk2 = new Stack<Card>();

		//shuffle the deck
		Random rand = new Random();
		for(int i = 0; i < 52; i++)
			Swap<Card>(ref initialDeck[i], ref initialDeck[rand.Next(52)]);

		//distribute the cards
		for(int i = 0; i < 52; i++)
		{
			if(i % 2 == 0)
				deck1.Enqueue(initialDeck[i]);
			else
				deck2.Enqueue(initialDeck[i]);
		}

		visualizer.NewGame(); //reset visuals
	}

	//if a war is not active each player draws one card
	//if war is active (war == true), each player draws two cards
	//returns true if game is in progress and false if game has ended
	public bool Draw()
	{
		if(deck1.Count > 0 && deck2.Count > 0) //make sure both players have cards
		{
			if(war)
			{
				//players put cards face down on the table
				risk1.Push(deck1.Dequeue());
				visualizer.AddCard(1);
				risk2.Push(deck2.Dequeue());
				visualizer.AddCard(2);
			}

			if(!GameOver()) //check victory conditions
			{
				//players put cards face up on the table
				risk1.Push(deck1.Dequeue());
				visualizer.AddCard(1, risk1.Peek());
				risk2.Push(deck2.Dequeue());
				visualizer.AddCard(2, risk2.Peek());
            }
		}
		else //attempted to draw after game over
			return false;
		return true;
	}

    //if card values are not equal, cards are awarded to the winner of the
	//	round (ends a war if in progress)
    //if card values are equal, a war starts (war flag set)
    //Draw() must be called with a return value of true before each call to Evaluate()
    public void Evaluate()
    {
        if(risk1.Peek().val > risk2.Peek().val) //player 1 wins round
		{
			//player 1's risked cards go to player 1's deck
			visualizer.RemoveCards(1, 1);
			while(risk1.Count > 0)
				deck1.Enqueue(risk1.Pop());
			//player 2's risked cards go to player 1's deck
			visualizer.RemoveCards(2, 1);
			while(risk2.Count > 0)
				deck1.Enqueue(risk2.Pop());
			war = false;
		}
		else if(risk1.Peek().val < risk2.Peek().val) //player 2 wins round
		{
			//player 2's risked cards go to player 2's deck
			visualizer.RemoveCards(2, 2);
			while(risk2.Count > 0)
				deck2.Enqueue(risk2.Pop());
			//player 1's risked cards go to player 2's deck
			visualizer.RemoveCards(1, 2);
			while(risk1.Count > 0)
				deck2.Enqueue(risk1.Pop());
			war = false;
		}
		else //no cards move during this round, war flag set
			war = true;

		GameOver(); //check victory conditions
    }

//checks for victory conditions and informs user of the winner if the game
//	has ended
private bool GameOver()
{
    if(gameOver)
        return true;

    if(deck1.Count > 0 && deck2.Count < 1)
    {
	    //player 1 wins
	    visualizer.Win(1);
        gameOver = true;
	    return true;
    }
    if(deck1.Count < 1 && deck2.Count > 0)
    {
        //player 2 wins
        visualizer.Win(2);
        gameOver = true;
        return true;
    }
    if(deck1.Count < 1 && deck2.Count < 1)
    {
	    //tie (unlikely)
	    visualizer.Win(-1);
        gameOver = true;
	    return true;
    }

    return false;
}
	//swaps the values of two variables
	private void Swap<T>(ref T a, ref T b)
	{
		T tmp = a;
		a = b;
		b = tmp;
	}
}
