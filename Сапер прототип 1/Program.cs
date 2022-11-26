namespace Сапер_прототип_1
{

    class Ruler
    {
        int width;
        int height;
        float minePercent;
        Random random = new Random();
        bool[,] IsMine;
        bool[,] IsVisible;
        bool[,] HaveMineN;
        int[,] Neighbors;
        int mineAmmount;
        int winAmm;
        int currentAmm = 0;
        bool HaveIlose = false;
        public Ruler(int Width, int Height, float MinePercent)
        {
            width = Width;
            height = Height;
            minePercent = MinePercent;
            IsMine = new bool[Width, Height];
            IsVisible = new bool[Width, Height];
            HaveMineN = new bool[Width, Height];
            Neighbors = new int[Width, Height];
            mineAmmount = (int)MathF.Round(width * height * minePercent / 100);
            winAmm = Width * Height - mineAmmount;
        }
        int X = 10;
        int Y = 10;
        public void Start()
        {
            Randomize();
            CalculateN();
        }
        public void Update()
        {
            Control();
            Fill();
            Render();
            WinScore();
            if (currentAmm == winAmm)
            {
                Console.Clear();
                Console.WriteLine("you win");
                while (true) ;
            }
        }
        void WinScore()
        {
            currentAmm = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (IsVisible[x, y]) currentAmm++;
                }
            }
        }
        void Render()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Console.SetCursorPosition(x, y);
                    if (HaveMineN[x, y]&& IsVisible[x, y]) Console.Write(Neighbors[x, y]);
                    if (IsVisible[x, y] && !HaveMineN[x, y]) Console.Write('.');
                    if (IsVisible[x, y] && HaveMineN[x, y]) Console.Write(Neighbors[x, y]);

                    if (HaveIlose)
                    {
                        Console.Clear();
                        Console.WriteLine("you lose");
                    }
                    if (!IsVisible[x, y]) Console.Write('\u2588');
                    
                }
            }
            Console.SetCursorPosition(X, Y);
            Console.ForegroundColor = ConsoleColor.White;
        }
        void Randomize()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    IsMine[x, y] = false;
                }
            }
            for (int t = 0; t < mineAmmount; t++)
            {
                int x = random.Next(1, width) - 1;
                int y = random.Next(1, height) - 1;
                if (IsMine[x, y])
                {
                    t--;
                }
                else
                {
                    IsMine[x, y] = true;
                }
            }
        }
        void CalculateN()
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    int Neighbors = 0;
                    for(int a = 0; a < 8; a++)
                    {
                        float A = (float)a * MathF.PI / 4;
                        int X = x + (int)MathF.Round(MathF.Cos(A));
                        int Y = y + (int)MathF.Round(MathF.Sin(A));
                        X = Distance(X, width, 0);
                        Y = Distance(Y, height, 0);
                        if(IsMine[X, Y])
                        {
                            Neighbors++;
                            HaveMineN[x, y] = true;
                        }
                    }
                    this.Neighbors[x, y] = Neighbors;
                }
            }
        }
        void Fill()
        {
            for(int t = 0;t < 50; t++) //я хотел сделать сравнение текущей и будущей сцены но оно криво работало
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        bool Visible = false;
                        if (!HaveMineN[x, y])
                        {
                            for (int a = 0; a < 8; a++)
                            {
                                float A = (float)a * MathF.PI / 4;
                                int X = Distance(x + (int)MathF.Round(MathF.Cos(A)), width, 0);
                                int Y = Distance(y + (int)MathF.Round(MathF.Sin(A)), height, 0);
                                if (IsVisible[X, Y])
                                {
                                    Visible = true;
                                }
                            }
                        }
                        if (!IsVisible[x, y] && Visible && !IsMine[x, y])
                        {
                            IsVisible[x, y] = true;
                        }
                    }
                }
            }
        }
        void Control()
        {
            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.UpArrow) Y--;
            if (keyInfo.Key == ConsoleKey.DownArrow) Y++;
            if (keyInfo.Key == ConsoleKey.LeftArrow) X--;
            if (keyInfo.Key == ConsoleKey.RightArrow) X++;
            X = Distance(X, width, 0);
            Y = Distance(Y, height, 0);
            if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                IsVisible[X, Y] = true;
                if(IsVisible[X, Y] && IsMine[X, Y])
                {
                    HaveIlose = true;
                }
            }
        }
        int Distance(int Current, int Max,int Min)
        {
            if (Current > Max - 1) return Current - Max;
            else if (Current < Min) return Current + Max;
            else return Current;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите Ширину(не рекомендую меньше 16)");
            int Width = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите Высоту");
            int Height = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите процент мин");
            int Percent = Convert.ToInt32(Console.ReadLine());
            Console.SetWindowSize(Width, Height);
            Console.SetBufferSize(Width, Height + 1);
            
            Ruler ruler = new Ruler(Width, Height, Percent);
            ruler.Start();
            while (true)
            {
                ruler.Update();
            }
        }
    }
    /*
     * думаем об рекурсивной функции
     * есть клетка
     * мы её обновили
     * надо вызвать обновление соседей(идея не работает ведь оно вызовет сканирование изначальной клетки и будет бесконечный цикл, хотя есть идеи решения)
     * сканируем сколько мин по соседству
     * пишем их число
     * стопаем рекурсию
     * да идет вызов обновления текущей клетки
     * потом она вызывает обнову у соседей
     * если у клетки сосед мина стопаем
     * 
     * кусок ***** можно использовать клетточный автомат
     */
}