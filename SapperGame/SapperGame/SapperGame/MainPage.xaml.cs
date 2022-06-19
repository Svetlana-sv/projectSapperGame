using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SapperGame
{
    public partial class MainPage : ContentPage
    {

        const string timeFormat = @"%m\:ss";

        bool isGameInProgress;
        DateTime gameStartTime;

        String level;
        

        public MainPage()
        {
            InitializeComponent(); 

            board.GameStarted += (sender, args) =>
            {
                isGameInProgress = true;
                
                gameStartTime = DateTime.Now;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    timeLabel.Text = (DateTime.Now - gameStartTime).ToString(timeFormat);
                    return isGameInProgress;
                });
            };

            board.GameEnded += (sender, hasWon) =>
            {
                isGameInProgress = false;

                if (hasWon)
                {
                    DisplayWonAnimation();
                }
                else
                {
                    DisplayLostAnimation();
                }
            };

            PrepareForNewGame();
        }

        void PrepareForNewGame()
        {
            board.NewGameInitialize();

            congratulationsText.IsVisible = false;
            consolationText.IsVisible = false;
            playAgainButton.IsVisible = false;
            playAgainButton.IsEnabled = false;

            timeLabel.Text = new TimeSpan().ToString(timeFormat);
            isGameInProgress = false;
        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                level = picker.Items[selectedIndex];
                switch (level)
                {
                    case("Легкий"):
                        Board.MINES = 2;
                        break;
                    case ("Средний"):
                        Board.MINES = 4;
                        break;
                    case ("Сложный"):
                        Board.MINES = 9;
                        break;
                    default:
                        break;
                }
                lbMineCount.Text = " бомб из " + Board.MINES;
                PrepareForNewGame();
            }  
        }

        void OnMainContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double width = contentView.Width;
            double height = contentView.Height;

            bool isLandscape = width > height;

            if (isLandscape)
            {
                mainGrid.RowDefinitions[0].Height = 0;
                mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

                mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

                Grid.SetRow(textStack, 1);
                Grid.SetColumn(textStack, 0);
            }
            else 
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
                mainGrid.RowDefinitions[1].Height = new GridLength(5, GridUnitType.Star);

                mainGrid.ColumnDefinitions[0].Width = 0;
                mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

                Grid.SetRow(textStack, 0);
                Grid.SetColumn(textStack, 1);
            }
        }

        
        void OnBoardContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double width = contentView.Width;
            double height = contentView.Height;
            double dimension = Math.Min(width, height);
            double horzPadding = (width - dimension) / 2;
            double vertPadding = (height - dimension) / 2;
            contentView.Padding = new Thickness(horzPadding, vertPadding);
        }

        async void DisplayWonAnimation()
        {
            congratulationsText.Scale = 3;
            congratulationsText.IsVisible = true;

            await Task.Delay(1000);
            await DisplayPlayAgainButton();
        }

        async void DisplayLostAnimation()
        {
            consolationText.Scale = 3;
            consolationText.IsVisible = true;


            await Task.Delay(1000);
            await DisplayPlayAgainButton();
        }

        async Task DisplayPlayAgainButton()
        {
            playAgainButton.Scale = 3;
            playAgainButton.IsVisible = true;
            playAgainButton.IsEnabled = true;
        }

        void OnplayAgainButtonClicked(object sender, object EventArgs)
        {
            PrepareForNewGame();
        }

        
    }
}