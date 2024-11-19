int height;
int width;

Console.Write("Введите высоту лабиринта: ");
height = int.Parse(Console.ReadLine());
Console.Write("Введите ширину лабиринта: ");
width = int.Parse(Console.ReadLine());

mazeClass maze = new mazeClass(height, width);
maze.play();

class mazeClass
{
    private int height;
    private int width;

    private String wallSymbol;
    private List<List<String>> maze = [];
    private List<List<int>> pathToExit = [];

    private bool playerExist = false;
    private List<int> playerCoords = [0, 0];

    public mazeClass(int height, int width, String wallsymbol = "█")
    {
        this.height = height < 7 ? 7 : height;
        this.height = this.height + 1 - this.height % 2;

        this.width = width < 7 ? 7 : width;
        this.width = this.width + 1 - this.width % 2;

        this.wallSymbol = wallsymbol;

        generate();
    }

    public void print()
    {
        String output = "";

        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                bool isUsedCoordsForPrint = false;

                if (this.playerExist && this.playerCoords[0] == x && this.playerCoords[1] == y) {
                    output += "P";
                    continue;
                }

                foreach (List<int> pathCoords in this.pathToExit)
                {
                    if (pathCoords[0] == x && pathCoords[1] == y)
                    {
                        output += "*";
                        isUsedCoordsForPrint = true;
                        break;
                    }
                }

                if (!isUsedCoordsForPrint) 
                {
                    output += this.maze[y][x];
                }
            }
            output += "\n";
        }

        Console.Write(output);
    }

    private void shuffle(ref List<List<int>> movement)
    {
        List<int> order = [];
        Random random = new Random();

        while (order.Count != movement.Count)
        {
            int randomNumber = random.Next(0, movement.Count);

            if (!order.Contains(randomNumber))
            {
                order.Add(randomNumber);
            }
        }

        List<List<int>> shuffled = [];

        for (int i = 0; i < order.Count; ++i)
        {
            shuffled.Add(movement[order[i]]);
        }

        movement = shuffled;
    }

    private void generate()
    {
        generateMaze();
        generateFinish();
    }

    private void generateMaze()
    {
        for (int y = 0; y < this.height; y++)
        {
            List<String> Row = [];
            for (int x = 0; x < this.width; x++)
            {
                Row.Add(this.wallSymbol);
            }
            this.maze.Add(Row);
        }

        List<List<int>> movement = [[0, -1], [1, 0], [0, 1], [-1, 0]];

        List<List<int>> path = [[1, 1]];
        this.maze[path[0][1]][path[0][0]] = " ";

        while (path.Count != 0)
        {
            bool isMoved = false;
            foreach (List<int> move in movement)
            {
                List<int> curCoords = path.Last();
                List<int> midCoords = [curCoords[0] + move[0], curCoords[1] + move[1]];
                List<int> newCoords = [curCoords[0] + move[0] * 2, curCoords[1] + move[1] * 2];

                if (isCoordsInMaze(newCoords) &&
                    this.maze[newCoords[1]][newCoords[0]] == this.wallSymbol)
                {
                    this.maze[midCoords[1]][midCoords[0]] = " ";
                    this.maze[newCoords[1]][newCoords[0]] = " ";

                    path.Add(newCoords);

                    isMoved = true;
                    shuffle(ref movement);
                    break;
                }
            }

            if (!isMoved)
            {
                path.Remove(path.Last());
            }
        }   
    }

    private void generateFinish()
    {
        Random random = new Random();

        int x = -1, y = -1;

        if (random.Next(0, 2) == 1)
        {

            if (random.Next(0, 2) == 1)
            {
                x = 0;
            }
            else
            {
                x = this.width - 1;
            }

        }
        else
        {
            if (random.Next(0, 2) == 1)
            {
                y = 0;
            }
            else
            {
                y = this.height - 1;
            }
        }

        if (x == -1)
        {
            x = random.Next(0, this.width / 2);
            x = 2 * x + 1;
        }
        else
        {
            y = random.Next(0, this.height / 2);
            y = 2 * y + 1;

        }

        this.maze[y][x] = " ";
    }

    private bool isCoordsInMaze(List<int> coords)
    {
        return (coords[0] > 0 && coords[0] < this.width-1) && (coords[1] > 0 && coords[1] < this.height-1);
    }

    private List<int> getRandomCoordsInMaze()
    {
        Random random = new Random();

        int x = random.Next(0, this.width / 2);
        x = 2 * x + 1;

        int y = random.Next(0, this.height / 2);
        y = 2 * y + 1;

        List<int> randomCoords = [x, y];
        return randomCoords;
    }

    private void findExit(List<int> startCoords)
    {
        List<List<int>> path = [startCoords];
        List<List<String>> whereWillBe = [];

        for (int y = 0; y < this.height; y++)
        {
            List<String> row = [];
            for (int x = 0; x < this.width; x++)
            {
                row.Add(this.maze[y][x]);
            }
            whereWillBe.Add(row);
        }

        whereWillBe[path[0][1]][path[0][0]] = this.wallSymbol;
        List<List<int>> movement = [[0, -1], [1, 0], [0, 1], [-1, 0]];

        while (isCoordsInMaze(path.Last()))
        {
            bool isMoved = false;
            foreach (List<int> move in movement)
            {
                List<int> curCoords = path.Last();
                List<int> newCoords = [curCoords[0] + move[0], curCoords[1] + move[1]];

                if (whereWillBe[newCoords[1]][newCoords[0]] == " ")
                {
                    whereWillBe[newCoords[1]][newCoords[0]] = this.wallSymbol;

                    path.Add(newCoords);

                    isMoved = true;
                    break;
                }
            }

            if (!isMoved)
            {
                path.Remove(path.Last());
            }
        }
        this.pathToExit = path[1..];
    }

    public void play()
    {
        this.playerExist = true;
        this.playerCoords = getRandomCoordsInMaze();

        bool showPath = false;

        Console.Clear();
        print();
        
        Console.SetCursorPosition(playerCoords[0], playerCoords[1]);
        Console.Write("P");

        while (this.playerExist) {
            String pressedKey = Console.ReadKey(true).Key.ToString();

            Dictionary<String, List<int>> movementDict = new Dictionary<string, List<int>>
            {
                ["W"] = [0, -1],
                ["D"] = [1, 0],
                ["A"] = [-1, 0],
                ["S"] = [0, 1],
                ["UpArrow"] = [0, -1],
                ["RightArrow"] = [1, 0],
                ["LeftArrow"] = [-1, 0],
                ["DownArrow"] = [0, 1]
            };

            if (movementDict.ContainsKey(pressedKey))
            {
                List<int> newCoords = [this.playerCoords[0] + movementDict[pressedKey][0], this.playerCoords[1] + movementDict[pressedKey][1]];

                if (this.maze[newCoords[1]][newCoords[0]] != this.wallSymbol)
                {
                    Console.SetCursorPosition(this.playerCoords[0], this.playerCoords[1]);
                    Console.Write(" ");
                    Console.SetCursorPosition(newCoords[0], newCoords[1]);
                    Console.Write("P");
                    this.playerCoords = newCoords;

                    Console.CursorVisible = false;

                    if (!isCoordsInMaze(this.playerCoords))
                    {
                        this.playerExist = false;
                        continue;
                    }

                }
            } else if (pressedKey == "P")
            {
                if (showPath)
                {
                    foreach (List<int> coord in this.pathToExit)
                    {
                        if (coord[0] != this.playerCoords[0] || coord[1] != this.playerCoords[1]) 
                        {
                            Console.SetCursorPosition(coord[0], coord[1]);
                            Console.Write(" ");
                        }
                    }

                    showPath = false;
                } else
                {
                    findExit(this.playerCoords);

                    foreach (List<int> coord in this.pathToExit) 
                    {
                        Console.SetCursorPosition(coord[0], coord[1]);
                        Console.Write("*");
                    }

                    showPath= true;
                }
            }
        }

        System.Console.CursorVisible = true;
        Console.SetCursorPosition(0, this.height);
        Console.WriteLine("Поздравляю, вы вышли из лабиринта!");
        Console.WriteLine("Нажмите любую кнопку для завершения...");
        Console.ReadKey(true);
    }
}