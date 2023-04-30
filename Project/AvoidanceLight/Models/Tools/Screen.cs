using AvoidanceLight.Models.Entity;

namespace AvoidanceLight.Models.Tools
{
    /// <summary>
    /// Screen class representing the screen
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// The horizontal size of the screen
        /// </summary>
        public int Xsize { get; private set; }
        /// <summary>
        /// The vertical size of the screen
        /// </summary>
        public int Ysize { get; private set; }
        /// <summary>
        /// The screen composition
        /// </summary>
        public char[,] ScreenComposition { get; private set; } = new char[0,0];

        /// <summary>
        /// The base constructor for the screen
        /// </summary>
        /// <param name="Xparam">The Horizontal size of the screen</param>
        /// <param name="Yparam">The vertical size of the screen</param>
        public Screen(int Xparam,int Yparam)
        {
            this.Xsize = Xparam;
            this.Ysize = Yparam;
            this.ScreenComposition = new char[Ysize,Xsize];
            InitScreen();
        }

        /// <summary>
        /// Fill the screen of empty character
        /// </summary>
        public void InitScreen()
        {
            for(int i = 0; i < this.Ysize;i++)
            {
                for(int j = 0; j < this.Xsize;j++)
                {
                    this.ScreenComposition[i,j] = ' ';
                }
            }
            

        }
        /// <summary>
        /// Add an object to the screen
        /// </summary>
        /// <param name="Object">The entity to display on the screen</param>
        public void AddObject(BaseEntity Object)
        {
            for(int i = 0; (i + Object.YPosition) < this.Ysize && i < Object.Ysize ;i++)
            {
                for(int j = 0; (j + Object.XPosition) < this.Xsize && j < Object.Xsize;j++)
                {
                    if((i + Object.YPosition) >= 0 && (j + Object.XPosition) >= 0)
                    {
                        this.ScreenComposition[(i + Object.YPosition),(j + Object.XPosition)] = Object.Sprite[i,j];
                    }
                }
            }
        }

        /// <summary>
        /// Display the screen in the console
        /// </summary>
        public void Display()
        {
            try
            {
                // We hide the cursor
                Console.CursorVisible = false;
                // We fill each cursor with the console position
                for(int i = 0; i < this.Ysize;i++)
                {
                    for(int j = 0; j < this.Xsize;j++)
                    {
                        Console.SetCursorPosition(j+1, i+1);

                        Console.Write(this.ScreenComposition[i,j]);
                    }
                }
            }
            catch(IOException)
            {

            }
            // We reinitialise the screen 
            InitScreen();
        }
        
    }
}