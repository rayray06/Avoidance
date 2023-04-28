using AvoidanceNEAT.Models.Entity;

namespace AvoidanceNEAT.Models.Tools
{
    public class Screen
    {
        public int Xsize { get; private set; }
        public int Ysize { get; private set; }
        public char[,] ScreenComposition { get; private set; } = new char[0,0];

        public Screen(int Xparam,int Yparam)
        {
            this.Xsize = Xparam;
            this.Ysize = Yparam;
            this.ScreenComposition = new char[Ysize,Xsize];
            InitScreen();
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
        
    }
}