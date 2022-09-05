using AvoidanceLight.Models.AINetwork;
using AvoidanceLight.Models.Tools;

// Hero sprite
// O
//-|-
// ^

namespace AvoidanceLight.Models.Entity
{
    public class Ero: BaseEntity
    {
        private static readonly char[,] EroSprite = {{' ','O',' '},{'-','|','-'},{' ','^',' '}};

        public int score { get; private set; } = 0;
        public bool alive { get; private set; } = true;

        private static readonly float MutationMax = 5;
        private static readonly float MutationChance = 15;
        public NeuralNetwork brain { get; private set; }
        public readonly Environement eroEnvironement;
        private static string EroNNTemplate = "Assets\\EroBrainTemplate.txt";

        private static int GetInputTemplate()
        {
            string[] lines = File.ReadAllLines(Ero.EroNNTemplate);
            return Convert.ToInt32(lines[0]);
        }

        private static int GetOutputTemplate()
        {
            string[] lines = File.ReadAllLines(Ero.EroNNTemplate);
            return Convert.ToInt32(lines[lines.Count()-1]);
        }

        private static int[] GetHiddenLayerTemplate()
        {
            string[] lines = File.ReadAllLines(Ero.EroNNTemplate);
            int[] hiddenLayerConfig = new int[lines.Count()-2];
            for(int i = 1; i < lines.Count()-1; i++)
            {
                hiddenLayerConfig[i-1] = Convert.ToInt32(lines[i]);
            }

            return hiddenLayerConfig;
        }

        public Ero(int XStartPos,int YStartPos,Environement env,Ero? firstParent = null, Ero? secondParent = null):
        base(XStartPos,YStartPos,Ero.EroSprite,1,0)
        {   
            this.brain = new NeuralNetwork( GetInputTemplate(),GetHiddenLayerTemplate(),GetOutputTemplate());
            this.eroEnvironement = env;
            if(firstParent != null && secondParent != null)
            {
                this.brain.Inherit(firstParent.brain,secondParent.brain);
                this.brain.Mutate(Ero.MutationChance,Ero.MutationMax);
            }
        }

        public Ero(int XStartPos,int YStartPos,Ero ero):
        base(XStartPos,YStartPos,Ero.EroSprite,1,0)
        {
            this.brain = ero.brain;
            this.eroEnvironement = ero.eroEnvironement;
        }

        private void GetUpperView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int j)
        {
            currentThreads.Add(Task.Run(() => 
            { 
                for(int i = YPosition-1; i >= 0; i--)
                {
                    if(view.ScreenComposition[i,j+XPosition] != ' ')
                    {
                        inputList[Start+2*j] = Math.Abs(i-YPosition);
                        inputList[Start+2*j+1] = 1;
                        break;
                    }
                    else if(i == 0)
                    {
                        inputList[Start+2*j] = Math.Abs(i-YPosition);
                        inputList[Start+2*j+1] = 0;
                        break;
                    }
                }
            }));
        }

        private void GetDownView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int j)
        {
            currentThreads.Add(Task.Run(() => 
            { 
                for(int i = YPosition+Ysize; i < view.Ysize; i++)
                {
                    if(view.ScreenComposition[i,j+XPosition] != ' ')
                    {
                        inputList[Start+2*j] = Math.Abs(i-(YPosition+Ysize));
                        inputList[Start+2*j+1] = 1;
                        break;
                    }
                    else if(i == (view.Ysize-1))
                    {
                        inputList[Start+2*j] = Math.Abs(i-(YPosition+Ysize));
                        inputList[Start+2*j+1] = 0;
                        break;
                    }
                }
            }));
        }


        private void GetLeftView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int j)
        {
                currentThreads.Add(Task.Run(() => 
                { 
                    for(int i = XPosition-1; i >= 0; i--)
                    {
                        if(view.ScreenComposition[j+YPosition,i] != ' ')
                        {
                            inputList[Start+2*j] =  Math.Abs(i-XPosition);
                            inputList[Start+2*j+1] = 1;
                            break;
                        }
                        else if(i == 0)
                        {
                            inputList[Start+2*j] = Math.Abs(i-XPosition);
                            inputList[Start+2*j+1] = 0;
                            break;
                        }
                    }
                }));
        }

        private void GetRightView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int j)
        {
            currentThreads.Add(Task.Run(() => 
            {
                for(int i = XPosition+Xsize; i < view.Xsize; i++)
                {
                    if(view.ScreenComposition[j+YPosition,i] != ' ')
                    {
                        inputList[Start+2*j] = Math.Abs(i-(XPosition+Xsize));
                        inputList[Start+2*j+1] = 1;
                        break;
                    }
                    else if(i == (view.Xsize-1))
                    {
                        inputList[Start+2*j] = Math.Abs(i-(XPosition+Xsize));
                        inputList[Start+2*j+1] = 0;
                        break;
                    }
                }
            }));
        }

        private void GetUpperLeftView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int iWhileLoop,int jWhileLoop)
        {
            currentThreads.Add(Task.Run(() => 
            {
                int ite = 0;
                while( iWhileLoop >= 0 && jWhileLoop >= 0)
                {
                    ite++;
                    if(view.ScreenComposition[jWhileLoop,iWhileLoop] != ' ')
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 1;
                        break;
                    }
                    else if(iWhileLoop == 0 || jWhileLoop == 0)
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 0;
                        break;
                    }

                    iWhileLoop--;
                    jWhileLoop--;
                }
            }));
        }

        private void GetUpperRightView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int iWhileLoop,int jWhileLoop)
        {
            currentThreads.Add(Task.Run(() => 
            {
                int ite = 0;
                while( iWhileLoop < view.Xsize && jWhileLoop >= 0)
                {
                    ite++;
                    if(view.ScreenComposition[jWhileLoop,iWhileLoop] != ' ')
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 1;
                        break;
                    }
                    else if(iWhileLoop == (view.Xsize-1) || jWhileLoop == 0)
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 0;
                        break;
                    }

                    iWhileLoop++;
                    jWhileLoop--;
                }
            }));
        }

        private void GetDownLeftView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int iWhileLoop,int jWhileLoop)
        {
            currentThreads.Add(Task.Run(() => 
            {            
                int ite = 0;
                while( iWhileLoop >= 0 && jWhileLoop < view.Ysize)
                {
                    ite++;
                    if(view.ScreenComposition[jWhileLoop,iWhileLoop] != ' ')
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 1;
                        break;
                    }
                    else if(iWhileLoop == 0 || jWhileLoop == (view.Ysize-1))
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 0;
                        break;
                    }

                    iWhileLoop--;
                    jWhileLoop++;
                }
            }));
        }


        private void GetDownRightView(List<Task> currentThreads,double[] inputList,Screen view,int Start,int iWhileLoop,int jWhileLoop)
        {
            currentThreads.Add(Task.Run(() => 
            {            
                int ite = 0;
                while( iWhileLoop < view.Xsize && jWhileLoop < view.Ysize)
                {
                    ite++;
                    if(view.ScreenComposition[jWhileLoop,iWhileLoop] != ' ')
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 1;
                        break;
                    }
                    else if(iWhileLoop == (view.Xsize-1) || jWhileLoop == (view.Ysize-1))
                    {
                        inputList[Start] = Math.Sqrt(2)*ite;
                        inputList[Start+1] = 0;
                        break;
                    }

                    iWhileLoop++;
                    jWhileLoop++;
                }
            }));
        }

        public void MakeAMove()
        {
            if(!this.alive)
            {
                return;
            }

            double[] inputList = new double[GetInputTemplate()];
            Screen view = eroEnvironement.MainScreen;
            List<Task> currentThreads = new List<Task>();


            //Up
            int Start = 0;
            for(int j = 0; j < Xsize;j++)
            {
                GetUpperView(currentThreads,inputList,view,Start,j);
            }

            //Down
            Start += 2*Xsize;
            for(int j = 0; j < Xsize;j++)
            {
                GetDownView(currentThreads,inputList,view,Start,j);
            }

            //Left
            Start += 2*Xsize;
            for(int j = 0; j < Ysize;j++)
            {
                GetLeftView(currentThreads,inputList,view,Start,j);
            }


            //Right
            Start += 2*Ysize;
            for(int j = 0; j < Ysize;j++)
            {
                GetRightView(currentThreads,inputList,view,Start,j);
            }

            //UpLeft
            Start += 2*Ysize;
            int iWhileLoop = XPosition-1;
            int jWhileLoop = YPosition-1;
            GetUpperLeftView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //UpRight
            Start += 2;
            iWhileLoop = XPosition+Xsize;
            jWhileLoop = YPosition-1;
            GetUpperRightView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //DownLeft
            Start += 2;
            iWhileLoop = XPosition-1;
            jWhileLoop = YPosition+Ysize;
            GetDownLeftView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //DownRight
            Start += 2;
            iWhileLoop = XPosition+Xsize;
            jWhileLoop = YPosition+Ysize;
            GetDownRightView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //EroPosition
            Start += 2;
            inputList[Start] = YPosition;
            inputList[Start+1] = XPosition;

            Task.WaitAll(currentThreads.ToArray());

            int choice = brain.Run(inputList);

            switch (choice)
            {
                case 0:
                    this.XMovement = 0;
                    this.YMovement = -1;
                    break;
                case 1:
                    this.XMovement = 0;
                    this.YMovement = 1;
                    break;
                case 2:
                    this.XMovement = -1;
                    this.YMovement = 0;
                    break;
                case 3:
                    this.XMovement = 1;
                    this.YMovement = 0;
                    break;
                case 4:
                    this.XMovement = -1;
                    this.YMovement = -1;
                    break;
                case 5:
                    this.XMovement = 1;
                    this.YMovement = 1;
                    break;
                case 6:
                    this.XMovement = -1;
                    this.YMovement = 1;
                    break;
                case 7:
                    this.XMovement = 1;
                    this.YMovement = 1;
                    break;
                case 8:
                    this.XMovement = 0;
                    this.YMovement = 0;
                    break;
            }



        }


        public override void Move()
        {
            if(this.alive)
            {
                score += 1;
                base.Move();
                this.XMovement = 0;
                this.YMovement = 0;
            }
        }

        public void Kill()
        {
            this.alive = false;
        }
    }
}