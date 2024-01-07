namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Point> snake;
        private Point food;
        private int direction;
        private System.Windows.Forms.Timer gameTimer;
        private bool gameStarted;

        private const int GridSize = 20;
        private const int TimerInterval = 75; // Reduced timer interval for quicker food generation

        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();

            InitializeGame();

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Tick += UpdateGame;
            gameTimer.Interval = TimerInterval;
            gameStarted = false;

            this.Text = "Snake Game";
            this.ClientSize = new Size(400, 400);
            this.BackColor = Color.Black;
            this.Paint += DrawGame;
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true;
        }

        private void InitializeGame()
        {
            snake = new List<Point> { new Point(5, 5) };
            food = GenerateFoodLocation();
            direction = 0;
            gameStarted = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int newDirection = direction;

            switch (e.KeyCode)
            {
                case Keys.D: newDirection = 0; break; // right
                case Keys.S: newDirection = 1; break; // down
                case Keys.A: newDirection = 2; break; // left
                case Keys.W: newDirection = 3; break; // up
            }

            // Ensure the snake cannot turn 180 degrees into itself
            if (Math.Abs(newDirection - direction) != 2)
            {
                direction = newDirection;
            }

            // Start the game only when the user presses a key
            if (!gameStarted)
            {
                gameStarted = true;
                gameTimer.Start();
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            Point head = snake.First();
            Point newHead = new Point(head.X, head.Y);

            switch (direction)
            {
                case 0: newHead.X++; break; // right
                case 1: newHead.Y++; break; // down
                case 2: newHead.X--; break; // left
                case 3: newHead.Y--; break; // up
            }

            snake.Insert(0, newHead);

            if (newHead.Equals(food))
            {
                food = GenerateFoodLocation();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            if (CheckCollisions(newHead))
            {
                GameOver();
            }

            this.Invalidate();
        }

        private bool CheckCollisions(Point head)
        {
            if (head.X < 0 || head.X >= this.ClientSize.Width / GridSize || head.Y < 0 || head.Y >= this.ClientSize.Height / GridSize)
            {
                return true; // Wall collision
            }

            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[i].Equals(head))
                {
                    return true; // Self-collision
                }
            }

            return false;
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over!");
            InitializeGame();
            gameTimer.Start();
        }

        private void DrawGame(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw snake
            for (int i = 0; i < snake.Count; i++)
            {
                Brush snakeBrush = (i == 0) ? Brushes.Green : Brushes.LimeGreen;
                g.FillRectangle(snakeBrush, snake[i].X * GridSize, snake[i].Y * GridSize, GridSize - 1, GridSize - 1);
            }

            // Draw food as a red square
            g.FillRectangle(Brushes.Red, food.X * GridSize, food.Y * GridSize, GridSize, GridSize);
        }

        private Point GenerateFoodLocation()
        {
            int x, y;

            // Ensure food does not spawn on the snake
            do
            {
                x = random.Next(0, this.ClientSize.Width / GridSize);
                y = random.Next(0, this.ClientSize.Height / GridSize);
            } while (snake.Any(point => point.X == x && point.Y == y));

            return new Point(x, y);
        }
    }
}