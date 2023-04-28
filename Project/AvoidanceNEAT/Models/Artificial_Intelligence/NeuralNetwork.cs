using System.Threading.Tasks;

namespace AvoidanceLight.Models.AINetwork
{
    /// <summary>
    /// Class defining the neural network processing
    /// </summary>
    public class NeuralNetwork
    {
        /// <summary>
        /// List corresponding to the input neurons
        /// </summary>
        public List<BaseNeuron> inputNeuron { get; protected set; } = new List<BaseNeuron>();
        
        /// <summary>
        /// List containing each hidden layers containing each one their number of neurons
        /// </summary>
        public List<List<BaseNeuron>> hiddenLayers { get; protected set; } = new List<List<BaseNeuron>>();
        
        /// <summary>
        /// List containing each output Neuron 
        /// </summary>
        public List<BaseNeuron> outputNeuron { get; protected set; } = new List<BaseNeuron>();
        
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="inputNeuronCount">The number of input neuron</param>
        /// <param name="hiddenLayersConfig">A list of Neuron </param>
        /// <param name="outputNeuronCount"></param>
        /// <param name="minweights"></param>
        /// <param name="maxweights"></param>
        public NeuralNetwork(int inputNeuronCount,int[] hiddenLayersConfig,int outputNeuronCount,double minweights = -10,double maxweights = 10)
        {
            for(int i = 0; i < inputNeuronCount; i++)
            {
                inputNeuron.Add(new BaseNeuron());
            }

            for(int i = 0; i < hiddenLayersConfig.Count(); i++)
            {
                hiddenLayers.Add(new List<BaseNeuron>());
                for(int j = 0; j < hiddenLayersConfig[i] ;j++ )
                {
                    if(0 != i)
                    {
                        hiddenLayers[i].Add(new BaseNeuron(hiddenLayersConfig[i-1],minweights,maxweights));
                    }
                    else
                    {
                        hiddenLayers[i].Add(new BaseNeuron(inputNeuronCount,minweights,maxweights));
                    }
                }

            }

            for(int i = 0; i < outputNeuronCount; i++)
            {
                if(hiddenLayersConfig.Count() > 0)
                {
                    outputNeuron.Add(new BaseNeuron(hiddenLayersConfig[hiddenLayersConfig.Count()-1],minweights,maxweights));
                }
                else
                {
                    outputNeuron.Add(new BaseNeuron(inputNeuronCount,minweights,maxweights));
                }
            }
        }

        public void Inherit(NeuralNetwork firstParent,NeuralNetwork secondParent)
        {
            for(int i = 0; i < hiddenLayers.Count();i++)
            {
                for(int j = 0; j < hiddenLayers[i].Count();j++)
                {
                    BaseNeuron currentNeuron = hiddenLayers[i][j];
                    BaseNeuron firstParentNeuron = firstParent.hiddenLayers[i][j];
                    BaseNeuron secondParentNeuron = secondParent.hiddenLayers[i][j];

                    currentNeuron.Inherit(firstParentNeuron,secondParentNeuron);
                }
            }

            for(int j = 0; j < outputNeuron.Count();j++)
            {
                BaseNeuron currentNeuron = outputNeuron[j];
                BaseNeuron firstParentNeuron = firstParent.outputNeuron[j];
                BaseNeuron secondParentNeuron = secondParent.outputNeuron[j];

                currentNeuron.Inherit(firstParentNeuron,secondParentNeuron);
            }
        }

        public void Mutate(float MutationChance,float MutationMax)
        {
            for(int i = 0; i < hiddenLayers.Count();i++)
            {
                for(int j = 0; j < hiddenLayers[i].Count();j++)
                {
                    BaseNeuron currentNeuron = hiddenLayers[i][j];

                    currentNeuron.Mutate(MutationChance,MutationMax);
                }
            }

            for(int j = 0; j < outputNeuron.Count();j++)
            {
                BaseNeuron currentNeuron = outputNeuron[j];

                currentNeuron.Mutate(MutationChance,MutationMax);
            }
        }

        public int Run(double[] inputValues)
        {
            for(int i = 0; i< inputValues.Count();i++)
            {
                inputNeuron[i].output = inputValues[i];
            }

            for(int i = 0; i < hiddenLayers.Count();i++)
            {
                double[] CurrentStepInputLoop;
                List<Task> currentThreadsLoop = new List<Task>();
                if(i == 0)
                {
                    
                    CurrentStepInputLoop = new double[inputNeuron.Count()];
                    for(int j = 0; j < inputNeuron.Count(); j++)
                    {
                        CurrentStepInputLoop[j] = inputNeuron[j].output;
                    }
                }
                else
                {
                    CurrentStepInputLoop = new double[hiddenLayers[i-1].Count()];
                    for(int j = 0; j < hiddenLayers[i-1].Count(); j++)
                    {
                        CurrentStepInputLoop[j] = hiddenLayers[i-1][j].output;
                    }
                }

                for(int j = 0;j < hiddenLayers[i].Count();j++)
                {
                    BaseNeuron currNeuron = hiddenLayers[i][j];
                    currentThreadsLoop.Add(Task.Run(() => {currNeuron.Calculate(CurrentStepInputLoop);}));
                }

                Task.WaitAll(currentThreadsLoop.ToArray());
            }

            List<Task> currentThreads = new List<Task>();
            double[] CurrentStepInput = new double[hiddenLayers[hiddenLayers.Count()-1].Count()];
            for(int j = 0; j < hiddenLayers[hiddenLayers.Count()-1].Count(); j++)
            {
                CurrentStepInput[j] = hiddenLayers[hiddenLayers.Count()-1][j].output;
            }
            
            for(int j = 0;j < outputNeuron.Count();j++)
            {
                BaseNeuron currentNeuron = outputNeuron[j];
                currentThreads.Add(Task.Run(() => {currentNeuron.Calculate(CurrentStepInput);}));
            }

            Task.WaitAll(currentThreads.ToArray());
            
            var (_, maxIndex) = outputNeuron.Select((x, i) => (x.output, i)).Max();
    
            return maxIndex;
        }
    }

}