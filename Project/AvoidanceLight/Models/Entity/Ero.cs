using AvoidanceLight.Models.AINetwork;
using AvoidanceLight.Models.Tools;

// Hero sprite
// O
//-|-
// ^

namespace AvoidanceLight.Models.Entity
{
    /// <summary>
    /// Class describing the hero behavior
    /// </summary>
    public class Ero: BaseEntity
    {
        /// <summary>
        /// The folder containing the Ero skins part
        /// </summary>
        private static readonly string EroSpriteLocation = "Assets\\EroSkin\\";

        /// <summary>
        /// the score of the given Hero
        /// </summary>
        public double score { get; private set; } = 0;
        /// <summary>
        /// Wheteher the hero is alive or not 
        /// </summary>
        public bool alive { get; private set; } = true;

        /// <summary>
        /// The minimum starting weight value for the hero brain
        /// </summary>
        private static double minweights = -10;
        /// <summary>
        /// The maximum starting weight value for the hero brain 
        /// </summary>
        private static double maxweights = 10;

        /// <summary>
        /// The mutation value maximum
        /// </summary>
        private static readonly float MutationMax = 5;
        /// <summary>
        /// The mutation chances 
        /// </summary>
        private static readonly float MutationChance = 15;
        /// <summary>
        /// The brain of the hero
        /// </summary>
        public NeuralNetwork brain { get; private set; }
        /// <summary>
        /// The Environement Where the Ero is located
        /// </summary>
        public readonly Playground eroPlayground;

        /// <summary>
        /// The path of the Neural wetwork template
        /// </summary>
        private static string EroNNTemplate = "Assets\\EroBrainTemplate.txt";

        
        /// <summary>
        /// Get the Input neuron number from the template
        /// </summary>
        /// <returns>The number of input neuron</returns>
        private static int GetInputTemplate()
        {
            string[] lines = File.ReadAllLines(Ero.EroNNTemplate);
            return Convert.ToInt32(lines[0]);
        }

        /// <summary>
        /// Get the output neuron number from the template
        /// </summary>
        /// <returns>The number of output neuron</returns>
        private static int GetOutputTemplate()
        {
            string[] lines = File.ReadAllLines(Ero.EroNNTemplate);
            return Convert.ToInt32(lines[lines.Count()-1]);
        }

        /// <summary>
        /// Get the number of neuron in each hidden layer neuron number from the template
        /// </summary>
        /// <returns>The number of input neuron</returns>
        private static int[] GetHiddenLayerTemplate()
        {
             
            string[] lines = File.ReadAllLines(Ero.EroNNTemplate);
            int[] hiddenLayerConfig;

            // if hidden layer then fill the array with the values else return a empty arrray
            if ((lines.Length - 2) > 0)
            {
                hiddenLayerConfig = new int[lines.Length - 2];
                for (int i = 1; i < lines.Length - 1; i++)
                {
                    hiddenLayerConfig[i - 1] = Convert.ToInt32(lines[i]);
                }
            }
            else
            {
                hiddenLayerConfig = new int[0];
            }

            return hiddenLayerConfig;
        }

        /// <summary>
        /// Base constructor for the hero
        /// </summary>
        /// <param name="XStartPos">The horizontal starting position of the hero</param>
        /// <param name="YStartPos">The vertical starting position of the hero</param>
        /// <param name="env">The environment of the hero</param>
        /// <param name="firstParent">First parent from which the hero inherit</param>
        /// <param name="secondParent">Second parent from which the parent inherit</param>
        public Ero(int XStartPos,int YStartPos,Playground env,Ero? firstParent = null, Ero? secondParent = null):
        base(XStartPos,YStartPos,Ero.SetSprite(),1,0)
        {   
            // Initializing the hero brain 
            this.brain = new NeuralNetwork( GetInputTemplate(),GetHiddenLayerTemplate(),GetOutputTemplate(),minweights,maxweights);
            this.eroPlayground = env;

            // If the two parent are definied then Inherit and Mutate 
            if(firstParent != null && secondParent != null)
            {
                this.brain.Inherit(firstParent.brain,secondParent.brain);
                this.brain.Mutate(Ero.MutationChance,Ero.MutationMax);
            }
        }

        /// <summary>
        /// Copy constructor for Hero
        /// </summary>
        /// <param name="XStartPos">Starting horizontal position of the new ero</param>
        /// <param name="YStartPos">Starting vertical position of the new ero</param>
        /// <param name="ero">Ero to copy stat from</param>
        public Ero(int XStartPos,int YStartPos,Ero ero):
        base(XStartPos,YStartPos,Ero.SetSprite(),1,0)
        {
            this.brain = ero.brain;
            this.eroPlayground = ero.eroPlayground;
        }

        /// <summary>
        /// Function to retrieve a random body part from the the hero
        /// </summary>
        /// <param name="BodyPart">The name of the body part to retrieve</param>
        /// <returns>A character corresponding to the selected body part</returns>
        private static char GetBodyPart(string BodyPart)
        {
            Random rdn = new Random();
            string line = File.ReadAllLines(Ero.EroSpriteLocation+BodyPart)[0];
            int charNum = line.Length;

            return line[rdn.Next(0,charNum)];
        }

        /// <summary>
        /// Retrieve a randomly selcted sprite for the current Hero
        /// </summary>
        /// <returns></returns>
        private static char[,] SetSprite()
        {
            char[,] NewSprite = new char[3,3];
            
            for(int i = 0; i < 2;i++)
            {
                for(int j = 0;j < 2;j++)
                {
                    NewSprite[i,j] = ' ';
                }
            }

            NewSprite[0,1] = GetBodyPart("Head");
            NewSprite[1,1] = GetBodyPart("Torso");
            NewSprite[1,0] = GetBodyPart("Arm");
            NewSprite[1,2] = GetBodyPart("Arm");
            NewSprite[2,1] = GetBodyPart("Leg");

            return NewSprite;   
        }

        /// <summary>
        /// Get information of the first object or wall directly above the given point of the user
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="j">The input step</param>
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

        /// <summary>
        /// Get information of the first object or wall directly below the given point of the user
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="j">The input step</param>
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

        /// <summary>
        /// Get information of the first object or wall directly left the given point of the user
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="j">The input step</param>
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
        /// <summary>
        /// Get information of the first object or wall directly right the given point of the user
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="j">The input step</param>
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

        /// <summary>
        /// Get information of the first object or wall on the upper left the given point of the user
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="iWhileLoop">The horieontal iterator of the verification</param>
        /// <param name="jWhileLoop">The vertical iterator of the verification</param>
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

        /// <summary>
        /// Get information of the first object or wall on the upper right the given point of the user
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="iWhileLoop">The horieontal iterator of the verification</param>
        /// <param name="jWhileLoop">The vertical iterator of the verification</param>
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

        /// <summary>
        /// Get information of the first object or wall on the lower left the given point of the hero
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="iWhileLoop">The horizontal iterator of the verification</param>
        /// <param name="jWhileLoop">The vertical iterator of the verification</param>
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

        /// <summary>
        /// Get information of the first object or wall on the lower left the given point of the hero
        /// </summary>
        /// <param name="currentThreads">The thread to which add the information retrieval</param>
        /// <param name="inputList">The input list where we need to add the result</param>
        /// <param name="view">The screen corresponding to the hero view</param>
        /// <param name="Start">The start point of the for the input</param>
        /// <param name="iWhileLoop">The horizontal iterator of the verification</param>
        /// <param name="jWhileLoop">The vertical iterator of the verification</param>
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

        /// <summary>
        /// Function processing the next move of the hero 
        /// </summary>
        public void MakeAMove()
        {
            // If the hero is dead don't move
            if(!this.alive)
            {
                return;
            }

            // Retrieve the number of input and create a array
            double[] inputList = new double[GetInputTemplate()];

            // We retrieve the screen
            Screen view = eroPlayground.MainScreen;

            List<Task> currentThreads = new List<Task>();


            //We look above the hero 
            int Start = 0;
            for(int j = 0; j < Xsize;j++)
            {
                GetUpperView(currentThreads,inputList,view,Start,j);
            }

            //We lokk below the hero
            Start += 2*Xsize;
            for(int j = 0; j < Xsize;j++)
            {
                GetDownView(currentThreads,inputList,view,Start,j);
            }

            //We look left to the hero
            Start += 2*Xsize;
            for(int j = 0; j < Ysize;j++)
            {
                GetLeftView(currentThreads,inputList,view,Start,j);
            }


            //We look right to the hero
            Start += 2*Ysize;
            for(int j = 0; j < Ysize;j++)
            {
                GetRightView(currentThreads,inputList,view,Start,j);
            }

            //We look on the upper left of the hero
            Start += 2*Ysize;
            int iWhileLoop = XPosition-1;
            int jWhileLoop = YPosition-1;
            GetUpperLeftView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //We look on the upper right of the hero
            Start += 2;
            iWhileLoop = XPosition+Xsize;
            jWhileLoop = YPosition-1;
            GetUpperRightView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //We look on the lower left of the hero
            Start += 2;
            iWhileLoop = XPosition-1;
            jWhileLoop = YPosition+Ysize;
            GetDownLeftView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //We look on the lower right of the hero
            Start += 2;
            iWhileLoop = XPosition+Xsize;
            jWhileLoop = YPosition+Ysize;
            GetDownRightView(currentThreads,inputList,view,Start,iWhileLoop,jWhileLoop);

            //We insert position of the hero
            Start += 2;
            inputList[Start] = YPosition;
            inputList[Start+1] = XPosition;

            // We wait for the process to end
            Task.WaitAll(currentThreads.ToArray());

            // We let the brain choose an action from the inputs
            int choice = brain.Run(inputList);

            // We set the movement according to the decision
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

        /// <summary>
        /// Function to exceute the next move of the hero
        /// </summary>
        public override void Move()
        {
            if(this.alive)
            {
                // If the hero is alive add a point to the fitness function
                score += 1;
                if(XMovement == 0 && YMovement == 0)
                {
                    // We the user don't move gain additional point
                    score += 0.001;
                }
                base.Move();
                this.XMovement = 0;
                this.YMovement = 0;
            }
        }

        /// <summary>
        /// Function the kill the hero
        /// </summary>
        public void Kill()
        {
            this.alive = false;
        }
    }
}