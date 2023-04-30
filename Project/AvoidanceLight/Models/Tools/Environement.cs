using AvoidanceLight.Models.Entity;


namespace AvoidanceLight.Models.Tools
{
    /// <summary>
    /// Running environment of the program
    /// </summary>
    public class Environement
    {
        /// <summary>
        /// Instance corresponding to the environment
        /// </summary>
        private static Environement? _instance;
        /// <summary>
        /// Retrieve the instance or initialize if instance not define
        /// </summary>
        /// <param name="XScreenSize">The horizontal size of the screen of the environment</param>
        /// <param name="YScreenSize">The vertical size of the screen of the environment</param>
        /// <param name="PopulationSize">The population size of the environnement</param>
        /// <returns>the instance of the environement</returns>
        public static Environement GetInstance(int XScreenSize,int YScreenSize,int PopulationSize)
        {
            if (Environement._instance == null)
            {
                Environement._instance = new Environement(XScreenSize,YScreenSize,PopulationSize);
            }
            return Environement._instance;
        }
        
        /// <summary>
        /// the playgrounds list
        /// </summary>
        public List<Playground> listPlayground {get; private set;}

        /// <summary>
        /// The population size
        /// </summary>
        private int populationSize;
        /// <summary>
        /// The title of the game to display below the screen
        /// </summary>
        private char[,] Title;
        /// <summary>
        /// Random object use througout the environment
        /// </summary>
        private Random rdn = new Random();
        /// <summary>
        /// The offset between the border of the screen 
        /// </summary>
        private static int offset = 5;

        /// <summary>
        /// Base constructor of the environement
        /// </summary>
        /// <param name="XScreenSize">The horizontal size of the screen of the environment</param>
        /// <param name="YScreenSize">The vertical size of the screen of the environment</param>
        /// <param name="PopulationSize">The population size of the environnement</param>
        public Environement(int XScreenSize,int YScreenSize,int PopulationSize)
        {
            populationSize = PopulationSize;

            listPlayground = new List<Playground>();

            this.Title = GetTitle();
            this.InitScreenBorder(XScreenSize, YScreenSize);

            // For each population create a playground
            for(int i = 0; i < PopulationSize;i++)
            {
                listPlayground.Add(new Playground(XScreenSize,YScreenSize));
            }
            
        }

        /// <summary>
        /// Create a new population from two parent
        /// </summary>
        /// <param name="firstParent">The first parent</param>
        /// <param name="secondParent">The second parent</param>
        private void NewPopulation(Ero firstParent,Ero secondParent)
        {
            // Set the first two plaground to the parents
            listPlayground[0].Reset(firstParent);
            listPlayground[1].Reset(secondParent);

            // for each remaining popuplation spot create a one new playground from the parents 
            for(int i = 2; i < populationSize;i++)
            {
                listPlayground[i].Reset(firstParent,secondParent);
            }
            
        }

        /// <summary>
        /// RUn the environnement 
        /// </summary>
        public void Run()
        {
            // Keep track of the number of frame survived 
            long currentStep = 0;
            Screen? MainScreen = null;


            while(true)
            {
                // We iterate and display the first screen state
                currentStep++;
                MainScreen?.Display();

                // If every hero died we reset the population from the two best hero of the population
                if(listPlayground.All(p => !(p.currEro.alive)))
                {
                    List<Playground> Survivors = listPlayground.OrderByDescending(e => e.currEro.score).Take(2).ToList();
                    NewPopulation(Survivors[0].currEro,Survivors[1].currEro);
                    currentStep = 0;
                }

                // We get the next frame of each playground
                Task[] MovementDecision = new Task[listPlayground.Count()];
                for(int i = 0; i<listPlayground.Count(); i++)
                {
                    Playground currPlayground = listPlayground[i];
                    MovementDecision[i] = Task.Run(() => currPlayground.NewFrame());
                }

                Task.WaitAll(MovementDecision);

                // We display the best hero on the main screen 
                if(listPlayground.Where(p => p.currEro.alive).Count() > 0)
                {
                    MainScreen = listPlayground.Where(p => p.currEro.alive).OrderByDescending(p => p.currEro.score).First().MainScreen;
                }

                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Get the title from the title file
        /// </summary>
        /// <returns>the character aray corresponding to the Title</returns>
        private static char[,] GetTitle()
        {
            // Retrieve the the Title from the Title 
            string[] lines = File.ReadAllLines("Assets\\Title.txt");

            // the max number of character for each line
            int maxCharCount = 0;
            
            // the number of line
            int lineNumber = lines.Count();

            // We retrieve the max number of character for a line
            foreach(string line in lines)
            {
                maxCharCount = Math.Max(maxCharCount,line.Count());
            }

            char[,] Title = new char[lineNumber,maxCharCount];

            // Fill the array with the character of the fill
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

        /// <summary>
        /// Initialise the screen border which doesn't change at each frame
        /// </summary>
        /// <param name="XSize">The horizontal screen size</param>
        /// <param name="YSize">The vertical screen size</param>
        private void InitScreenBorder(int XSize,int YSize)
        {
            try
            {
                // We hide the cursor
                Console.CursorVisible = false;
                // We set the console size
                SetScreenSize(XSize,YSize);
                // We clean the console
                Console.Clear();

                // The top border
                for(int j = 0; j < XSize+2;j++)
                {
                    Console.SetCursorPosition(j,0);
                    Console.Write('&');
                }

                // The sides border
                for(int i = 1; i < YSize+1;i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write('!');

                    Console.SetCursorPosition(XSize+1, i);
                    Console.Write('!'); 
                }

                // The bottom layer
                for(int j = 0; j < XSize+2;j++)
                {
                    Console.SetCursorPosition(j, YSize+1);
                    Console.Write('&');
                }

                // The Title
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

        /// <summary>
        /// Set the console size
        /// </summary>
        /// <param name="XSize">The horizontal side of the screen </param>
        /// <param name="YSize">The Vertical size of the screen </param>
        private void SetScreenSize(int XSize,int YSize)
        {
            int X = Math.Max(XSize+2,Title.GetLength(1)) + Environement.offset;
            int Y = Environement.offset + YSize+2+Title.GetLength(0);
            Console.SetWindowSize( X , Y );
        }

    }
}