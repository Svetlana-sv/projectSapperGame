using System;
using System.Reflection;
using Xamarin.Forms;


namespace SapperGame
{
    enum TileStatus
    {
        Hidden,
        Flagged,
        Exposed
    }

    class Tile : Frame
    {
        TileStatus tileStatus = TileStatus.Hidden;
        readonly Label label;
        readonly Image flagImage, mineImage;
        readonly static ImageSource flagImageSource;
        readonly static ImageSource mineImageSource;
        bool doNotFireEvent;

        public event EventHandler<TileStatus> TileStatusChanged;

        static Tile()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            flagImageSource = ImageSource.FromUri(new Uri("https://png.pngtree.com/png-vector/20190114/ourlarge/pngtree-vector-flag-icon-png-image_313056.jpg"));
            mineImageSource = ImageSource.FromUri(new Uri("https://static.vecteezy.com/system/resources/previews/002/148/475/non_2x/bomb-icon-design-free-vector.jpg"));
        }

        public Tile(int row, int col)
        {
            Random random = new Random();

            this.Row = row;
            this.Col = col;
            
            this.BackgroundColor = Color.WhiteSmoke;
            //this.BorderColor = Color.Black;
            this.BorderColor = Color.FromRgb(random.Next(255), random.Next(255), random.Next(255));
            this.Padding = 2;

            label = new Label {
                Text = " ",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.RoyalBlue,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            flagImage = new Image {
                Source = flagImageSource,

            };

            mineImage = new Image {
                Source = mineImageSource,
            };

            TapGestureRecognizer singleTap = new TapGestureRecognizer {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            this.GestureRecognizers.Add(singleTap);

            TapGestureRecognizer doubleTap = new TapGestureRecognizer {
                NumberOfTapsRequired = 2
            };
            doubleTap.Tapped += OnDoubleTap;
            this.GestureRecognizers.Add(doubleTap);
        }

        public int Row { private set; get; }

        public int Col { private set; get; }

        public bool IsMine { set; get; }

        public int SurroundingMineCount { set; get; }

        public TileStatus Status {
            set {
                if (tileStatus != value) {
                    tileStatus = value;

                    switch (tileStatus) {
                        case TileStatus.Hidden:
                            this.Content = null;
                            break;

                        case TileStatus.Flagged:
                            this.Content = flagImage;
                            break;

                        case TileStatus.Exposed:
                            if (this.IsMine) {
                                this.Content = mineImage;
                            } else {
                                this.Content = label;
                                label.Text =
                                        (this.SurroundingMineCount > 0) ?
                                            this.SurroundingMineCount.ToString() : " ";
                            }
                            break;
                    }

                    if (!doNotFireEvent && TileStatusChanged != null) {
                        TileStatusChanged(this, tileStatus);
                    }
                }
            }
            get {
                return tileStatus;
            }
        }

        public void Initialize()
        {
            doNotFireEvent = true;
            this.Status = TileStatus.Hidden;
            this.IsMine = false;
            this.SurroundingMineCount = 0;
            doNotFireEvent = false;
        }

        void OnSingleTap(object sender, object args)
        {
            switch (this.Status) {
            case TileStatus.Hidden:
                this.Status = TileStatus.Flagged;
                break;

            case TileStatus.Flagged:
                this.Status = TileStatus.Hidden;
                break;

            case TileStatus.Exposed:
                    // Do nothing
                break;
            }
        }

        void OnDoubleTap (object sender, object args)
        {
            this.Status = TileStatus.Exposed;
        }

        
    }
}
