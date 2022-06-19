using System;
using Xamarin.Forms;

namespace SapperGame
{
    class Board : AbsoluteLayout
    {
   
        const int COLS = 5;
        const int ROWS = 5;         
        public static int MINES = 2;
        
        Tile[,] tiles = new Tile[ROWS, COLS];
        int flaggedTileCount;
        bool isGameInProgress;              // на первое нажатие
        bool isGameInitialized;             // на двойной клик
        bool isGameEnded;

        public event EventHandler GameStarted;
        public event EventHandler<bool> GameEnded;
       

        public Board()
        {
            for (int row = 0; row < ROWS; row++)
                for (int col = 0; col < COLS; col++)
                {
                    Tile tile = new Tile(row, col);
                    tile.TileStatusChanged += OnTileStatusChanged;
                    this.Children.Add(tile);
                    tiles[row, col] = tile;
                }

            SizeChanged += (sender, args) =>
                {
                    double tileWidth = this.Width / COLS;
                    double tileHeight = this.Height / ROWS;

                    foreach (Tile tile in tiles)
                    {
                        Rectangle bounds = new Rectangle(tile.Col * tileWidth,
                                                         tile.Row * tileHeight,
                                                         tileWidth, tileHeight);
                        AbsoluteLayout.SetLayoutBounds(tile, bounds);
                    }
                };

            NewGameInitialize();
        }

        public void NewGameInitialize()
        {
            // обновление tile
            foreach (Tile tile in tiles)
                tile.Initialize();

            isGameInProgress = false;
            isGameInitialized = false;
            isGameEnded = false;
            this.FlaggedTileCount = 0;
        }

        public int FlaggedTileCount
        {
            set
            {
                if (flaggedTileCount != value)
                {
                    flaggedTileCount = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return flaggedTileCount;
            }
        }

        public int MineCount
        { 
            get
            {
                return MINES;
            }
        }

        
        // вызывается после первого двойного нажатия
        void DefineNewBoard(int tappedRow, int tappedCol)
        {
            
            Random random = new Random();
            int mineCount = 0;

            while (mineCount < MINES)
            {
                // рандомная колонка и ряд
                int row = random.Next(ROWS);
                int col = random.Next(COLS);

                // пропуск если уже мина
                if (tiles[row, col].IsMine)
                {
                    continue;
                }

                // избагать tappedRow & Col
                if (row >= tappedRow - 1 &&
                    row <= tappedRow + 1 &&
                    col >= tappedCol - 1 &&
                    col <= tappedCol + 1)
                {
                    continue;
                }

                // мина
                tiles[row, col].IsMine = true;

                // подсчет количества мин вокруг
                CycleThroughNeighbors(row, col,
                    (neighborRow, neighborCol) =>
                    {
                        ++tiles[neighborRow, neighborCol].SurroundingMineCount;
                    });

                mineCount++;
            }
        }

        void CycleThroughNeighbors(int row, int col, Action<int, int> callback)
        {
            int minRow = Math.Max(0, row - 1);
            int maxRow = Math.Min(ROWS - 1, row + 1);
            int minCol = Math.Max(0, col - 1);
            int maxCol = Math.Min(COLS - 1, col + 1);

            for (int neighborRow = minRow; neighborRow <= maxRow; neighborRow++)
                for (int neighborCol = minCol; neighborCol <= maxCol; neighborCol++)
                {
                    if (neighborRow != row || neighborCol != col)
                        callback(neighborRow, neighborCol);
                }
        }

        void OnTileStatusChanged(object sender, TileStatus tileStatus)
        {
            if (isGameEnded)
                return;

            // после касания первой плитки игра продолжается
            if (!isGameInProgress)
            {
                isGameInProgress = true;

                // Активировать событие GameStarted
                if (GameStarted != null)
                {
                    GameStarted(this, EventArgs.Empty);
                }
            }

            // Обновите количество "flagged" перед проверкой 
            int flaggedCount = 0;

            foreach (Tile tile in tiles)
                if (tile.Status == TileStatus.Flagged)
                    flaggedCount++;

            this.FlaggedTileCount = flaggedCount;

            // Получите плитку, статус которой изменился
            Tile changedTile = (Tile)sender;

            // Если он открыт, требуются некоторые действия
            if (tileStatus == TileStatus.Exposed)
            {
                if (!isGameInitialized)
                {
                    DefineNewBoard(changedTile.Row, changedTile.Col);
                    isGameInitialized = true;
                }

                if (changedTile.IsMine)
                {
                    isGameInProgress = false;
                    isGameEnded = true;

                    // Запустите событие GameEnded
                    if (GameEnded != null)
                    {
                        GameEnded(this, false);
                    }
                    return;
                }

                // Автоматическое раскрытие для нулевых окружающих мин
                if (changedTile.SurroundingMineCount == 0)
                {
                    CycleThroughNeighbors(changedTile.Row, changedTile.Col,
                        (neighborRow, neighborCol) =>
                        {
                            // Открыть все соседнии ячейки
                            tiles[neighborRow, neighborCol].Status = TileStatus.Exposed;
                        });
                }
            }

            // проверка на победу
            bool hasWon = true;

            foreach (Tile til in tiles)
            {
                if (til.IsMine && til.Status != TileStatus.Flagged)
                    hasWon = false;

                if (!til.IsMine && til.Status != TileStatus.Exposed)
                    hasWon = false;
            }

            // если победил
            if (hasWon)
            {
                isGameInProgress = false;
                isGameEnded = true;

                // Запустите событие GameEnded
                if (GameEnded != null)
                {
                    GameEnded(this, true);
                }
                return;
            }
        }
    }
}
