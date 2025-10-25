using Prototype_2;

namespace Prototype_2
{
    public class Cell
    {
        public bool visited = false;
        public bool TopWall = true;
        public bool BottomWall = true;
        public bool LeftWall = true;
        public bool RightWall = true;
    }
    public class Maze
    {
        private int height;
        private int width;
        private Cell[,] grid;
        private Random rnd;

        public int Height
        {
            get { return height; }
        }
        public int Width
        {
            get { return width; }
        }
        public Maze(int width, int height, Random rnd)
        {
            this.width = width;
            this.height = height;
            this.rnd = rnd;

            grid = new Cell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = new Cell();
                }
            }
        }
        public Cell GetCell(int x, int y)
        {
            return grid[x, y];
        }
        public void Generate()
        {
            GenerateFrom(0, 0);
        }
        private void GenerateFrom(int x, int y)
        {
            grid[x, y].visited = true;
            // create an array of 4 direction tuples
            (int dx, int dy)[] directions = new (int dx, int dy)[4];

            // fill each slot in the array
            directions[0] = (0, -1);  // up
            directions[1] = (1, 0);   // right
            directions[2] = (0, 1);   // down
            directions[3] = (-1, 0);  // left

            Shuffle(directions);
            // go through each possible direction
            for (int i = 0; i < directions.Length; i++)
            {
                int dx = directions[i].dx;
                int dy = directions[i].dy;

                int nx = x + dx;
                int ny = y + dy;

                //check new position is valid (inside maze and not visited)
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !grid[nx, ny].visited)
                {
                    //remove wall between current cell and new cell
                    RemoveWall(grid[x, y], grid[nx, ny], dx, dy);

                    //recursively gen maze from the new cell
                    GenerateFrom(nx, ny);
                }
            }
        }
        private void Shuffle((int dx, int dy)[] dirs)
        {

            //fisher yates shuffle

            //start from last index and go in revcerse
            for (int i = dirs.Length - 1; i > 0; i--)
            {
                // pick random index between 0 and i inclusive
                int j = rnd.Next(i + 1);

                (int dx, int dy) temp = dirs[i];
                // swap values at i and j
                dirs[i] = dirs[j];
                dirs[j] = temp;
            }
        }
        private void RemoveWall(Cell current, Cell next, int dx, int dy)
        {
            // right
            if (dx == 1)
            {
                current.RightWall = false;
                next.LeftWall = false;
            }
            // left
            else if (dx == -1)
            {
                current.LeftWall = false;
                next.RightWall = false;
            }
            // down
            else if (dy == 1)
            {
                current.BottomWall = false;
                next.TopWall = false;
            }
            // up
            else if (dy == -1)
            {
                current.TopWall = false;
                next.BottomWall = false;
            }
        }
        public void Draw(Player player, MazeItem[,] itemGrid, int target, List<string> collected, string status)
        {
            Console.WriteLine("Target: " + target);
            Console.WriteLine("Collected: " + string.Join("", collected));
            Console.WriteLine("Controls: WASD move, SPACE pick up, BACKSPACE drop last, ENTER check target");
            Console.WriteLine();
            if (!string.IsNullOrEmpty(status)) Console.WriteLine(status);
            if (collected.Count > 0 && ExpressionMaker.IsExpressionValid(collected))
            {
                double val;
                if (ExpressionMaker.TryEvaluate(collected, out val))
                {
                    Console.WriteLine("Current Result: " + val);
                }
                else
                {
                    Console.WriteLine("Current Result: (error)");
                }
            }
            else if (collected.Count > 0)
            {
                Console.WriteLine("Current Result: (incomplete/invalid)");
            }

            Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                // print the top walls for this row
                for (int x = 0; x < width; x++)
                {
                    if (grid[x, y].TopWall == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("+---");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.Write("+   ");
                        Console.ForegroundColor = ConsoleColor.Gray;

                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("+");
                Console.ForegroundColor = ConsoleColor.Gray;

                // print the left walls and the contents of each cell
                for (int x = 0; x < width; x++)
                {
                    string cell = "   ";

                    // show the player
                    if (x == player.X && y == player.Y)
                    {
                        cell = " P ";
                    }
                    // show an item if one is here
                    else if (itemGrid[x, y] != null)
                    {
                        cell = " " + itemGrid[x, y].Symbol + " ";
                    }

                    // print with or without left wall
                    if (grid[x, y].LeftWall == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        if (cell == " P ")
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(cell);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.Write(cell);
                        }

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(cell);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("|");
                Console.ForegroundColor = ConsoleColor.Gray;

            }


            for (int x = 0; x < width; x++)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.Write("+---");
                Console.ForegroundColor = ConsoleColor.Gray;

            }
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("+");
            Console.ForegroundColor = ConsoleColor.Gray;


            Console.WriteLine();
            Console.WriteLine("Controls: WASD to move, SPACE pick up, ENTER check target");
        }
    }
    public class Player
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public List<string> collected { get; private set; }

        private int fartmarker = 1;
        public Player()
        {
            X = 0;
            Y = 0;
            collected = new List<string>();
        }
        public void Reset()
        {
            X = 0;
            Y = 0;
            collected.Clear();
            MarkyQueeFartClear();
        }
        public bool TryMove(ConsoleKey key, Maze maze)
        {
            int newX = X;
            int newY = Y;
            Cell current = maze.GetCell(X, Y);

            if (key == ConsoleKey.W && !current.TopWall) newY--;
            else if (key == ConsoleKey.S && !current.BottomWall) newY++;
            else if (key == ConsoleKey.A && !current.LeftWall) newX--;
            else if (key == ConsoleKey.D && !current.RightWall) newX++;
            if (newX >= 0 && newX < maze.Width && newY >= 0 && newY < maze.Height)
            {
                X = newX;
                Y = newY;
                return true;
            }
            return false;
        }
        public void Pickup(MazeItem[,] itemGrid)
        {
            if (itemGrid[X, Y] != null)
            {
                collected.Add(itemGrid[X, Y].Symbol);
                markyqueefart(itemGrid[X, Y].Symbol);
                itemGrid[X, Y] = null;
            }
        }
        public void Drop(MazeItem[,] itemGrid)
        {
            if (collected.Count > 0 && itemGrid[X, Y] == null)
            {
                string last = collected[collected.Count - 1];
                collected.RemoveAt(collected.Count - 1);
                if (ExpressionMaker.IsNumber(last))
                {
                    itemGrid[X, Y] = new NumberItem(int.Parse(last));
                }
                else
                {
                    itemGrid[X, Y] = new OperatorItem(last);
                }
                MarkyQueeFartReset();
            }
        }
        private void markyqueefart(string item)
        {
            Console.CursorLeft = 9 + fartmarker * 2;
            Console.CursorTop = 1;
            Console.Write(item + "                                                 ");
            fartmarker++;
            MarkyQueefartEquals();
            
        }
        public void MarkyQueeFartReset()
        {
            fartmarker--;
            Console.CursorLeft = 9 + fartmarker * 2;
            Console.CursorTop = 1;
            Console.Write("                                                       ");
            MarkyQueefartEquals();
        }
        private void MarkyQueeFartClear()
        {
            fartmarker = 1;
        }
        private void MarkyQueefartEquals()
        {
            double val;
            if (ExpressionMaker.TryEvaluate(collected, out val))
            {

                Console.CursorLeft = 9 + fartmarker * 2;
                Console.CursorTop = 1;
                Console.Write("= " + val);
            }
        }
    }

    public class ExpressionMaker
    {
        public static bool IsExpressionValid(List<string> expr)
        {
            // empty list means not valid
            if (expr.Count == 0)
            {
                return false;
            }

            //expression must have odd number of tokens
            if (expr.Count % 2 == 0)
            {
                return false;
            }

            //first and last token must be numbers
            if (!IsNumber(expr[0]) || !IsNumber(expr[expr.Count - 1]))
            {
                return false;
            }

            for (int i = 0; i < expr.Count; i++)
            {
                if (i % 2 == 0) // even positions should be numbers
                {
                    if (!IsNumber(expr[i]))
                    {
                        return false;
                    }
                }
                else // odd positions should be operators
                {
                    if (!IsOperator(expr[i]))
                    {
                        return false;
                    }
                }
            }

            // expressions valid if all checks are passed
            return true;
        }
        public static bool TryEvaluate(List<string> expr, out double result) //takes in expression and returns true if evaluation succedded and also the result
        {
            // must always set result to something (rule of out)
            result = 0;

            try
            {
                //makes copy of the expression so it can be changed
                List<string> tokens = new List<string>(expr);

                for (int i = 0; i < tokens.Count; i++) //loops through looking for * or /
                {
                    if (tokens[i] == "*" || tokens[i] == "/")
                    {
                        // gets numbers on the left and right of opoerator
                        double left = double.Parse(tokens[i - 1]);
                        double right = double.Parse(tokens[i + 1]);

                        // if dividing by zero it throws an error
                        if (tokens[i] == "/" && Math.Abs(right) < 1e-12)
                        {
                            throw new DivideByZeroException();
                        }

                        //calculates result
                        double value;
                        if (tokens[i] == "*")
                        {
                            value = left * right;
                        }
                        else
                        {
                            value = left / right;
                        }

                        // puts result in left slot
                        tokens[i - 1] = value.ToString();

                        // removes operator and the right number
                        tokens.RemoveAt(i + 1);
                        tokens.RemoveAt(i);

                        // i-- so nothing is missed
                        i--;
                    }
                }

                // handles + and -
                double acc = double.Parse(tokens[0]); // start with the first number
                for (int i = 1; i < tokens.Count; i += 2)
                {
                    string op = tokens[i];
                    double num = double.Parse(tokens[i + 1]);

                    if (op == "+")
                    {
                        acc = acc + num;
                    }
                    else if (op == "-")
                    {
                        acc = acc - num;
                    }
                }

                //store final result in out variable
                result = acc;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<string> BuildSolvableExpression(Random rnd)
        {
            string[] ops = { "+", "-", "*", "/" };

            //repeat until valid expression
            while (true)
            {
                // decide length: either 3 or 5 tokens
                int len;
                if (rnd.Next(2) == 0)
                {
                    len = 3;
                }
                else
                {
                    len = 5;
                }

                List<string> expr = new List<string>();


                for (int i = 0; i < len; i++)
                {
                    if (i % 2 == 0)
                    {
                        //even index = number (1–9)
                        int number = rnd.Next(1, 10);
                        expr.Add(number.ToString());
                    }
                    else
                    {
                        //odd index = operator
                        int index = rnd.Next(ops.Length);
                        expr.Add(ops[index]);
                    }
                }

                //remove possibility of dividing by zero
                for (int i = 1; i < expr.Count; i += 2)
                {
                    if (expr[i] == "/")
                    {
                        int right = int.Parse(expr[i + 1]);
                        if (right == 0)
                        {
                            expr[i + 1] = "1"; // replace with 1
                        }
                    }
                }

                //check its valid
                if (!IsExpressionValid(expr))
                {
                    continue; // try again
                }

                //try evaluating expression
                double value;
                bool ok = TryEvaluate(expr, out value);
                if (!ok)
                {
                    continue; // try again
                }

                //only accept if its an integer
                int intVal = (int)Math.Round(value);
                if (Math.Abs(value - intVal) < 0.000000001 && intVal >= 0)
                {
                    return expr;
                }
            }
        }
        public static bool IsOperator(string s)
        {

            return s == "+" || s == "-" || s == "*" || s == "/";

        }
        public static bool IsNumber(string s)
        {
            return char.IsDigit(s[0]);
        }

    }

    public abstract class MazeItem
    {
        public abstract string Symbol { get; }
        public abstract bool IsValidForExpression();

    }

    public class NumberItem : MazeItem
    {
        private int value;
        public NumberItem(int value)
        {
            this.value = value;
        }
        public override string Symbol
        {
            get
            {
                return value.ToString();
            }
        }
        public override bool IsValidForExpression()
        {
            return true;
        }
    }

    public class OperatorItem : MazeItem
    {
        private string op;
        public OperatorItem(string op)
        {
            this.op = op;
        }
        public override string Symbol
        {
            get
            {
                return op;
            }
        }
        public override bool IsValidForExpression()
        {
            return true;
        }
    }

}

public class Game
{
    private Maze maze;
    private Player player;
    private MazeItem[,] items;
    private int target;
    private Random rnd;
    private string status;

    public Game(int width, int height)
    {
        rnd = new Random();
        maze = new Maze(width, height, rnd);
        player = new Player();
        items = new MazeItem[width, height];
        status = "";
    }
    private void SetupRound()
    {

        player.Reset();
        items = new MazeItem[maze.Width, maze.Height];

        maze = new Maze(maze.Width, maze.Height, rnd);
        maze.Generate();

        List<string> expr = ExpressionMaker.BuildSolvableExpression(rnd);
        double val;
        ExpressionMaker.TryEvaluate(expr, out val);
        target = (int)Math.Round(val);

        PlaceExpressionInMaze(expr);
        PlaceDistractors(14);
    }
    private void PlaceExpressionInMaze(List<string> expr)
    {
        int placed = 0;
        int attempts = 0;

        //repeat until all tokens are placed or too many attempts
        while (placed < expr.Count && attempts < 1000)
        {
            attempts++;

            //picks random spot
            int x = rnd.Next(maze.Width);
            int y = rnd.Next(maze.Height);

            //start pos
            if (x == 0 && y == 0)
            {
                //skips and repeats
            }
            else if (items[x, y] != null)
            {
                // already something here so skips
            }
            else
            {
                string symbol = expr[placed];

                if (ExpressionMaker.IsNumber(symbol))
                {
                    items[x, y] = new NumberItem(int.Parse(symbol));
                }
                else
                {
                    items[x, y] = new OperatorItem(symbol);
                }

                placed++;
            }
        }

    }
    private void PlaceDistractors(int extraCount)
    {
        //distractor symbol pool
        string[] pool = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "-", "*", "/" };

        int placed = 0;
        int tries = 0;

        // Keep trying until enough distractors placed or hit the tries limit
        while (placed < extraCount && tries < 2000)
        {
            tries++;

            //pick random coord
            int x = rnd.Next(maze.Width);
            int y = rnd.Next(maze.Height);

            //skip start pos
            if (x == 0 && y == 0)
            {
                // do nothing, just try again
            }
            //skip if somethings already there
            else if (items[x, y] != null)
            {
                //skip and repeat
            }
            else
            {
                // Choose a random symbol from the pool
                int index = rnd.Next(pool.Length);
                string symbol = pool[index];

                // Create the right type of MazeItem
                if (ExpressionMaker.IsNumber(symbol))
                {
                    items[x, y] = new NumberItem(int.Parse(symbol));
                }
                else
                {
                    items[x, y] = new OperatorItem(symbol);
                }

                placed++;
            }
        }
    }
    private bool CheckWin()
    {
        bool ok = false;
        if (ExpressionMaker.IsExpressionValid(player.collected))
        {
            double res;
            bool success = ExpressionMaker.TryEvaluate(player.collected, out res);
            if (success && Math.Abs(res - target) < 0.0001) ok = true;
        }
        return ok;
    }
    private bool PlayRound()
    {
        Console.Clear();
        maze.Draw(player, items, target, player.collected, status);

        while (true)
        {
            string ontopof = " ";
            (int, int) OldPlayerPos = (player.X, player.Y);

            if (items[player.X, player.Y] != null)
            {

                ontopof = items[player.X, player.Y].Symbol;
            }


            ConsoleKey key = Console.ReadKey(true).Key;
            // movement

            // pickup
            if (key == ConsoleKey.Spacebar)
            {
                player.Pickup(items);
            }


            // drop
            if (key == ConsoleKey.Backspace)
            {
                player.Drop(items);
            }




            if (key == ConsoleKey.Enter)
            {
                Console.Clear();
                Console.WriteLine("Target: " + target);
                Console.WriteLine("Your expression: " + string.Join("", player.collected));


                bool ok = CheckWin();
                Console.WriteLine(ok ? "Correct! You matched the target." : "Not equal to target.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                return ok; // return round result
            }

            if (player.TryMove(key, maze))
            {
                Console.CursorLeft = OldPlayerPos.Item1 * 4 + 2;
                Console.CursorTop = OldPlayerPos.Item2 * 2 + 6;                                 // sets previous position to console and displays what it was before you moved
                Console.Write(ontopof);

                Console.CursorLeft = player.X * 4 + 2;                               // sets current position to console and displays icon
                Console.CursorTop = player.Y * 2 + 6;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("P");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

    }
    public void GameLoop()
    {
        while (true)
        {
            SetupRound();
            bool won = PlayRound();

            Console.Clear();
            Console.WriteLine(won ? "You win this round!" : "Incorrect expression. Maze reset.");
            Console.WriteLine("Press any key for a new maze...");
            Console.ReadKey(true);
        }
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Game game = new Game(10, 10);
        game.GameLoop();

        Console.ReadKey();
    }
}