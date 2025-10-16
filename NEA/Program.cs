using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype_2
{
    internal class Program
    {
        // maze size
        static int height = 10;
        static int width = 10;

        static Cell[,] maze = new Cell[width, height];

        //items on floor
        static string[,] items = new string[width, height];

        //player state
        static int playerX = 0;
        static int playerY = 0;
        static List<string> collected = new List<string>();
        

        static Random rnd = new Random();

        static int target = 0;

        // takes in array of direction tuples and shuffles
        static void Shuffle((int dx, int dy)[] dirs)    
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
        static void GenerateMaze(int x, int y)
        {
            //start cell is marked as visited
            maze[x, y].visited = true;

            // create an array of 4 direction tuples
            (int dx, int dy)[] directions = new (int dx, int dy)[4];

            // fill each slot in the array
            directions[0] = (0, -1);  // up
            directions[1] = (1, 0);   // right
            directions[2] = (0, 1);   // down
            directions[3] = (-1, 0);  // left

            // shuffle the directions
            Shuffle(directions);

            // go through each possible direction
            for (int i = 0; i < directions.Length; i++)
            {
                int dx = directions[i].dx;
                int dy = directions[i].dy;

                int nx = x + dx;
                int ny = y + dy;

                // check new position is valid (inside maze and not visited)
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !maze[nx, ny].visited)
                {
                    // remove wall between current cell and new cell
                    RemoveWall(maze[x, y], maze[nx, ny], dx, dy);

                    // recursively gen maze from the new cell
                    GenerateMaze(nx, ny);
                }
            }
        }
        static void RemoveWall(Cell current, Cell next, int dx, int dy)
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
        static void PrintMaze()
        {
            Console.WriteLine("Target: " + target);
            Console.WriteLine("Collected: " + string.Join("", collected));
            if (collected.Count > 0 && IsExpressionValid(collected))
            {
                double val;
                if (TryEvaluate(collected, out val))
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
                    if (maze[x, y].TopWall == true)
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
                    if (x == playerX && y == playerY)
                    {
                        cell = " P ";
                    }
                    // show an item if one is here
                    else if (items[x, y] != null)
                    {
                        cell = " " + items[x, y] + " ";
                    }

                    // print with or without left wall
                    if (maze[x, y].LeftWall == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(cell);

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

        static bool IsExpressionValid(List<string> expr)
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

        static bool IsNumber(string s)
        {
            //is a number if its longer than 0
            //and its first character is a digit (0-9)
            if (s.Length > 0 && char.IsDigit(s[0]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool IsOperator(string s)
        {

            if (s == "+")
            {
                return true;
            }
            else if (s == "-")
            {
                return true;
            }
            else if (s == "*")
            {
                return true;
            }
            else if (s == "/")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool TryEvaluate(List<string> expr, out double result) //takes in expression and returns true if evaluation succedded and also the result
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

        static List<string> BuildSolvableExpression()
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

        static void PlaceExpressionInMaze(List<string> expr)
        {
            int placed = 0;
            int attempts = 0;

            //repeat until all tokens are placed or too many attempts
            while (placed < expr.Count && attempts < 1000)
            {
                attempts++;

                //picks random spot
                int x = rnd.Next(width);
                int y = rnd.Next(height);

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
                    //places
                    items[x, y] = expr[placed];
                    placed++;
                }
            }
        }

        static void PlaceDistractors(int extraCount)
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
                int x = rnd.Next(width);
                int y = rnd.Next(height);

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
                    //place random distractor from pool
                    int index = rnd.Next(pool.Length);
                    items[x, y] = pool[index];
                    placed++;
                }
            }
        }

        // Main
        static void Main()
        {
            Console.CursorVisible = false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    maze[i, j] = new Cell();
                }
            }

            GenerateMaze(0, 0);

            List<string> solutionExpr = BuildSolvableExpression();                        // unsure where you print the target and how much you have collected so far

            double val;
            TryEvaluate(solutionExpr, out val);
            target = (int)Math.Round(val); // guaranteed >= 0

            PlaceExpressionInMaze(solutionExpr);                                                  // there is a bug where it says current result is incomplete/invalid ATM
            PlaceDistractors(14);
            Console.Clear();
            PrintMaze();
            while (true)
            {
                //Console.Clear();
                //PrintMaze();

                ConsoleKey key = Console.ReadKey(true).Key;

                int newX = playerX;
                int newY = playerY;

                //movement controls
                if (key == ConsoleKey.W && maze[playerX, playerY].TopWall == false)
                {
                    newY = newY - 1;
                }
                else if (key == ConsoleKey.S && maze[playerX, playerY].BottomWall == false)
                {
                    newY = newY + 1;
                }
                else if (key == ConsoleKey.A && maze[playerX, playerY].LeftWall == false)
                {
                    newX = newX - 1;
                }
                else if (key == ConsoleKey.D && maze[playerX, playerY].RightWall == false)
                {
                    newX = newX + 1;
                }
                int tempX = playerX; int tempY = playerY;
                
                //move if new position is in maze boundaries
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)          // what about walls?
                {
                    playerX = newX;
                    playerY = newY;
                }
                char movedFrom;
                if (items[tempX, tempY] != null)
                {
                    movedFrom = items[tempX, tempY].ToCharArray()[0];
                }
                else
                {
                    movedFrom = ' ';
                }
                    Console.CursorLeft = tempX * 4 + 2;
                Console.CursorTop = tempY*2+4;
                Console.Write(movedFrom);

                Console.CursorLeft = newX * 4+2;
                Console.CursorTop = newY * 2+4;
                Console.Write("P");
                

                //pick up item with space
                if (key == ConsoleKey.Spacebar && items[playerX, playerY] != null)
                {
                    collected.Add(items[playerX, playerY]);
                    items[playerX, playerY] = null;
                }

                // checks if collected expression matches target (press ENTER)
                if (key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.WriteLine("Target: " + target);
                    Console.WriteLine("Your expression: " + string.Join("", collected));

                    bool ok = false;

                    if (IsExpressionValid(collected))
                    {
                        double res;
                        bool success = TryEvaluate(collected, out res);

                        if (success)
                        {
                            if (Math.Abs(res - target) < 0.0001)
                            {
                                ok = true;
                            }
                        }
                    }

                    if (ok)
                    {
                        Console.WriteLine("Correct! You matched the target.");
                    }
                    else
                    {
                        Console.WriteLine("Not equal to target.");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    break;
                }
            }
            Main();
        }
    }
}
