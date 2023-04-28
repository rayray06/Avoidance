using AvoidanceNEAT.Models.Entity;


namespace AvoidanceNEAT.Models.Tools
{
    public class Environement
    {
        private static Environement? _instance;
        public static Environement GetInstance(int XScreenSize,int YScreenSize,int PopulationSize)
        {
            if (Environement._instance == null)
            {
                Environement._instance = new Environement(XScreenSize,YScreenSize,PopulationSize);
            }
            return Environement._instance;
        }
        
        public List<Playground> listPlayground {get; private set;}
        private int populationSize;
        private char[,] Title;
        private Random rdn = new Random();
        
        private static int offset = 5;

        public Environement(int XScreenSize,int YScreenSize,int PopulationSize)
        {
            populationSize = PopulationSize;
            listPlayground = new List<Playground>();
            this.Title = GetTitle();
            this.InitScreenBorder(XScreenSize, YScreenSize);
            for(int i = 0; i < PopulationSize;i++)
            {
                listPlayground.Add(new Playground(XScreenSize,YScreenSize));
            }
            
        }

        private void NewPopulation(Ero firstParent,Ero secondParent)
        {
            listPlayground[0].Reset(firstParent);
            listPlayground[1].Reset(secondParent);
            for(int i = 2; i < populationSize;i++)
            {
                listPlayground[i].Reset(firstParent,secondParent);
            }
            
        }

        public void Run()
        {
            long currentStep = 0;
            Screen? MainScreen = null;

            while(true)
            {
                currentStep++;
                MainScreen?.Display();

                if(listPlayground.All(p => !(p.currEro.alive)))
                {
                    List<Playground> Survivors = listPlayground.OrderByDescending(e => e.currEro.score).Take(2).ToList();
                    NewPopulation(Survivors[0].currEro,Survivors[1].currEro);
                    currentStep = 0;
                }


                Task[] MovementDecision = new Task[listPlayground.Count()];
                for(int i = 0; i<listPlayground.Count(); i++)
                {
                    Playground currPlayground = listPlayground[i];
                    MovementDecision[i] = Task.Run(() => currPlayground.NewFrame());
                }

                Task.WaitAll(MovementDecision);

                if(listPlayground.Where(p => p.currEro.alive).Count() > 0)
                {
                    MainScreen = listPlayground.Where(p => p.currEro.alive).OrderByDescending(p => p.currEro.score).First().MainScreen;
                }

                Thread.Sleep(30);
            }
        }

        private static char[,] GetTitle()
        {
            string[] lines = File.ReadAllLines("Assets\\Title.txt");
            int maxCharCount = 0;
            int lineNumber = lines.Count();
            foreach(string line in lines)
            {
                maxCharCount = Math.Max(maxCharCount,line.Count());
            }

            char[,] Title = new char[lineNumber,maxCharCount];

            for(int j = 0; j < lineNumber;j++)
            {
                string line = lines[j];

                for(int i = 0; i < maxCharCount;i++)
                {
                    if(i < line.Count())
                    {
                        Title[j,i] = line[i];
                    }
                    else
                    {
                        Title[j,i] = ' ';
                    }
                }
            }
            return Title;
        }

        private void InitScreenBorder(int XSize,int YSize)
        {
            try
            {
                Console.CursorVisible = false;
                SetScreenSize(XSize,YSize);
                Console.Clear();

                for(int j = 0; j < XSize+2;j++)
                {
                    Console.SetCursorPosition(j,0);
                    Console.Write('&');
                }

                for(int i = 1; i < YSize+1;i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write('!');

                    Console.SetCursorPosition(XSize+1, i);
                    Console.Write('!'); 
                }

                for(int j = 0; j < XSize+2;j++)
                {
                    Console.SetCursorPosition(j, YSize+1);
                    Console.Write('&');
                }

                for(int j = 0; j < Title.GetLength(1); j++)
                {
                    for(int i = 0; i < Title.GetLength(0); i++)
                    {
                        Console.SetCursorPosition(j, YSize+2+i);
                        Console.Write(this.Title[i,j]);
                    }
                }
            }
            catch(IOException)
            {

            }

        }

        private void SetScreenSize(int XSize,int YSize)
        {
            int X = Math.Max(XSize+2,Title.GetLength(1)) + Environement.offset;
            int Y = Environement.offset + YSize+2+Title.GetLength(0);
            Console.SetWindowSize( X , Y );
        }

    }
}