using System;

public class Controller
{
	public static int Main()
	{
		//initialize game
		Game game = new Game();
		string input = "";
		bool run = true;

		while(run)
		{
			//get user input: restart, play round, play entire game
			Console.WriteLine("Type restart, draw, or auto.");
			input = Console.ReadLine();
			if(input == "restart")
				game.NewGame();
			else if(input == "draw")
				game.Draw();
			else if(input == "auto")
				AutoPlay(game);
			else
				Console.WriteLine("Improper input.");
		}

		
		return 1;
	}

	private static void AutoPlay(Game game) //simulate an entire game without user input
	{
		while(game.Draw());
	}
}
