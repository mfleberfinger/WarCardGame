using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace DeacomWarVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game; //object containing state variables and functions to play war
        private bool evalStep; //determines whether to call Draw() or Evaluate() when "Next" button is clicked
        //track the number of cards on the table and in the deck for each player
        private int deckCount1, deckCount2;
        //cards on the table for player1 and player2
        private Stack<Image> risk1, risk2;
        private Storyboard story;

        public MainWindow()
        {
            InitializeComponent();
            game = new Game(this);
        }

        private void ResetClicked(object sender, EventArgs e)
        {
            game.NewGame();
        }

        private void NextClicked(object sender, EventArgs e)
        {
            if(evalStep)
            {
                game.Evaluate();
                evalStep = false;
            }
            else if(game.Draw())
                evalStep = true;
        }


        //set up visuals showing no cards on table and two full decks
        public void NewGame()
        {
            this.WinLabel.Visibility = Visibility.Hidden;
            this.LoseLabel.Visibility = Visibility.Hidden;
            this.TieLabel.Visibility = Visibility.Hidden;
            evalStep = false;
            //reset deck counters
            this.countLabel1.Content = "26";
            this.countLabel2.Content = "26";
            deckCount1 = deckCount2 = 26;
            //remove cards from the table
            if(risk1 != null)
                while(risk1.Count > 0)
                    this.CardGrid.Children.Remove(risk1.Pop());
            if(risk2 != null)
                while(risk2.Count > 0)
                    this.CardGrid.Children.Remove(risk2.Pop());
            
            risk1 = new Stack<Image>();
            risk2 = new Stack<Image>();
        }

        //shows card leaving appropriate deck and being placed on table
        //if card not supplied, the new card is face down
        public void AddCard(int player, Card card)
        {
            string source = GetCardString(card);
            AddCard(player, source);
        }
        public void AddCard(int player)
        {
            string source = "/DeacomWarVisual;component/Card_Back.png";
            AddCard(player, source);
        }
        private void AddCard(int player, string source)
        {
            Image newCard = new Image();

            this.CardGrid.Children.Add(newCard);
            var uriSource = new Uri(source, UriKind.Relative);
            newCard.Source = new BitmapImage(uriSource);
            newCard.Height = this.deckImage2.Height;
            newCard.Width = this.deckImage2.Width;

            Thickness startPos = new Thickness(0), endPos = new Thickness(0);
            if (player == 1)
            {
                newCard.HorizontalAlignment = deckImage1.HorizontalAlignment;
                newCard.VerticalAlignment = deckImage1.VerticalAlignment;
                //calculate starting and ending position (margins) of the card
                startPos = this.deckImage1.Margin;
                endPos = new Thickness(360,(234 + risk1.Count * 17),0,0);
                //keep track of placed cards
                risk1.Push(newCard);
                deckCount1--;
                //show number of remaining cards in the deck
                this.countLabel1.Content = deckCount1.ToString();
            }
            if (player == 2)
            {
                newCard.HorizontalAlignment = deckImage2.HorizontalAlignment;
                newCard.VerticalAlignment = deckImage2.VerticalAlignment;
                //rotate the card image to face the "other player"
                newCard.RenderTransformOrigin = new Point(0.5, 0.5);
                newCard.RenderTransform = new RotateTransform(180);
                //calculate starting and ending position (margins) of the card
                startPos = this.deckImage2.Margin;
                endPos = new Thickness(260, 234 - risk2.Count * 17,0,0);
                //keep track of placed cards
                risk2.Push(newCard);
                deckCount2--;
                //show number of remaining cards in the deck
                this.countLabel2.Content = deckCount2.ToString();
            }


            //create a thickness animation to animate the margin changes
            ThicknessAnimation anim = new ThicknessAnimation();
            anim.From = startPos;
            anim.To = endPos;
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.25));

            //associate the animation with a storyboard
            story = new Storyboard();
            story.Children.Add(anim);
            //associate the animation with the new card image
            Storyboard.SetTarget(anim, newCard);
            //associate the animation with the margin property of Image
            Storyboard.SetTargetProperty(anim, new PropertyPath(Image.MarginProperty));
       
            story.Begin(this);
        }

        //returns the resource path to the desired card
        private string GetCardString(Card card)
        {
            //"/DeacomWarVisual;component/ace_of_spades2.png"
            string suit = card.suit.ToString();
            string face = "";

            if(card.val < 11)
                face = card.val.ToString();
            else
            {
                switch(card.val)
                {
                    case 11:
                        face = "jack";
                        break;
                    case 12:
                        face = "queen";
                        break;
                    case 13:
                        face = "king";
                        break;
                    case 14:
                        face = "ace";
                        break;
                }
            }
            return String.Format("/DeacomWarVisual;component/{0}_of_{1}.png", face, suit);
        }


        //removes all cards from the given risk stack and moves them to the given deck
        //risk and deck refer to the side of the table the cards are taken from
        //	and the deck the cards are placed in, respectively
        public void RemoveCards(int risk, int deck)
        {
            Queue<Image> animQueue = new Queue<Image>();
            Thickness startPos = new Thickness(0); //start position for animation
            //set end position for animations
            Thickness endPos = this.deckImage1.Margin;
            
            if (deck == 2)
                endPos = this.deckImage2.Margin;

            if (risk == 1)
                while (risk1.Count > 0)
                    animQueue.Enqueue(risk1.Pop());
            else
                while (risk2.Count > 0)
                    animQueue.Enqueue(risk2.Pop());

            //move (animate) cards from the table to the correct deck
            while(animQueue.Count > 0)
            {
                Image currentCard = animQueue.Dequeue();
                startPos = currentCard.Margin;
                
                //create a thickness animation to animate the margin changes
                ThicknessAnimation anim = new ThicknessAnimation();
                anim.From = startPos;
                anim.To = endPos;
                anim.Duration = new Duration(TimeSpan.FromSeconds(0.25));

                //associate the animation with a storyboard
                story = new Storyboard();
                story.Children.Add(anim);
                //associate the animation with the current card image
                Storyboard.SetTarget(anim, currentCard);
                //associate the animation with the margin property of Image
                Storyboard.SetTargetProperty(anim, new PropertyPath(Image.MarginProperty));

                story.Completed += (sender, e) =>
                    {
                        //remove the image associated with the removed card
                        this.CardGrid.Children.Remove(currentCard);
                        //update the deck counter labels
                        if(deck == 1)
                        {
                            deckCount1++;
                            this.countLabel1.Content = deckCount1.ToString();
                        }
                        else
                        {
                            deckCount2++;
                            this.countLabel2.Content = deckCount2.ToString();
                        }
                    };

                story.Begin(this);
            }

        }


        //outputs "player [player] wins" or "Tie" depending on argument
        public void Win(int player)
        {
            if (player == 1)
            {
                this.WinLabel.Visibility = Visibility.Visible;
            }
            else if (player == 2)
            {
                this.LoseLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.TieLabel.Visibility = Visibility.Visible;
            }
        }
    }
}
