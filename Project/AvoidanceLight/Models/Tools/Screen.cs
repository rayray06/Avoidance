using AvoidanceLight.Models.Entity;

namespace AvoidanceLight.Models.Tools
{
    public class Screen
    {
        public int Xsize { get; private set; }
        public int Ysize { get; private set; }
        public char[,] ScreenComposition { get; private set; } = new char[0,0];
        private char[,] Title;
        private static int offset = 5;

        public Screen(int Xparam,int Yparam)
        {
            this.Xsize = Xparam;
            this.Ysize = Yparam;
            this.ScreenComposition = new char[Ysize,Xsize];
            this.Title = Screen.GetTitle();
            InitScreen();
            InitScreenBorder();
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
        private void InitScreenBorder()
        {
            try
            {
                Console.CursorVisible = false;
                SetScreenSize();
                Console.Clear();

                for(int j = 0; j < this.Xsize+2;j++)
                {
                    Console.SetCursorPosition(j,0);
                    Console.Write('&');
                }

                for(int i = 1; i < this.Ysize+1;i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write('!');

                    Console.SetCursorPosition(this.Xsize+1, i);
                    Console.Write('!'); 
                }

                for(int j = 0; j < this.Xsize+2;j++)
                {
                    Console.SetCursorPosition(j, this.Ysize+1);
                    Console.Write('&');
                }

                for(int j = 0; j < Title.GetLength(1); j++)
                {
                    for(int i = 0; i < Title.GetLength(0); i++)
                    {
                        Console.SetCursorPosition(j, this.Ysize+2+i);
                        Console.Write(this.Title[i,j]);
                    }
                }
            }
            catch(IOException)
            {

            }

        }

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

        public void Display()
        {

            try
            {
                Console.CursorVisible = false;
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

            InitScreen();
        }

        private void SetScreenSize()
        {
            int X = Math.Max(this.Xsize+2,Title.GetLength(1)) + Screen.offset;
            int Y = Screen.offset + this.Ysize+2+Title.GetLength(0);
            Console.SetWindowSize( X , Y );
        }
        
    }
}