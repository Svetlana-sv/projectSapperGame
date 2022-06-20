using SapperGame.Interface;
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
        App app;
       
        public MainPage(App app)
        {
            InitializeComponent();
            this.app = app;
            piker.Title = App.difficulty;
            PrepareActions();
            PrepareForNewGame();
        }
        void PrepareActions()
        {
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
                
                WriteFile(hasWon);

            };
        }
        void WriteFile(bool status)
        {
            string message;
            if (status)
            {
                message = "Победа - Уровень: "+ App.difficulty + " - Время: "+timeLabel.Text;
            }
            else
            {
                message = "Поражение - Уровень: " + App.difficulty + " - Время: " + timeLabel.Text;
            }
            
            
            //DependencyService.Get<IFileService>().WriteFile(message);
            DependencyService.Get<IFileService>().CreateFile(message);
        }
        
        void PrepareForNewGame()
        {
            //app.restart();
           

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
                        Board.COLS = 4;
                        Board.ROWS = 4;
                        break;
                    case ("Средний"):
                        Board.MINES = 4;
                        Board.COLS = 5;
                        Board.ROWS = 5;
                        break;
                    case ("Сложный"):
                        Board.MINES = 9;
                        Board.COLS = 6;
                        Board.ROWS = 6;
                        break;
                    default:
                        break;
                }
                App.difficulty = level;
                lbMineCount.Text = " бомб из " + Board.MINES;

                //PrepareForNewGame();
                app.Restart();
                
               
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

            await Task.Delay(1000);
        }

        void OnplayAgainButtonClicked(object sender, object EventArgs)
        {
            PrepareForNewGame();
        }

        void ShowRating(object sender, EventArgs args)
        {
            Navigation.PushAsync(new RatingPage());
        }
    }
}