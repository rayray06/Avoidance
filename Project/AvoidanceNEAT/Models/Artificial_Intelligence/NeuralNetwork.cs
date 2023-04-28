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
        /// Base Constructor of the Neural Network
        /// </summary>
        /// <param name="inputNeuronCount">The number of input neuron</param>
        /// <param name="hiddenLayersConfig">A list of number Neuron </param>
        /// <param name="outputNeuronCount">The number of output neuron</param>
        /// <param name="minweights">the minimum weigth</param>
        /// <param name="maxweights">the maxmum weigth</param>
        public NeuralNetwork(int inputNeuronCount,int[] hiddenLayersConfig,int outputNeuronCount,double minweights,double maxweights)
        {
            // initialize the input layer
            for(int i = 0; i < inputNeuronCount; i++)
            {
                inputNeuron.Add(new BaseNeuron());
            }
            int previouscount = inputNeuronCount;

            // for each hidden layer define the neuron with the number of weigths corresponding at the previous layer size
            for (int i = 0; i < hiddenLayersConfig.Count(); i++)
            {
                hiddenLayers.Add(new List<BaseNeuron>());
                for(int j = 0; j < hiddenLayersConfig[i] ;j++ )
                {
                    hiddenLayers[i].Add(new BaseNeuron(previouscount, minweights,maxweights));
                }
                previouscount = hiddenLayersConfig[i];

            }

            // initilize the output layer
            for(int i = 0; i < outputNeuronCount; i++)
            {
                outputNeuron.Add(new BaseNeuron(previouscount, minweights,maxweights));
            }
        }

        /// <summary>
        /// Inheritance function from two parent neural network 
        /// </summary>
        /// <param name="firstParent">The first parent to inherit from</param>
        /// <param name="secondParent">The second parent to inherit from</param>
        public void Inherit(NeuralNetwork firstParent,NeuralNetwork secondParent)
        {
            // We iterate through each hidden layer making each neuron inherit from each parent 
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

            // We iterate through the output layer to pass each neuron through the Inherit process
            for(int j = 0; j < outputNeuron.Count();j++)
            {
                BaseNeuron currentNeuron = outputNeuron[j];
                BaseNeuron firstParentNeuron = firstParent.outputNeuron[j];
                BaseNeuron secondParentNeuron = secondParent.outputNeuron[j];

                currentNeuron.Inherit(firstParentNeuron,secondParentNeuron);
            }
        }

        /// <summary>
        /// Mutation function to mutate the neural wetwork
        /// </summary>
        /// <param name="MutationChance">The mutation chances between 0-100</param>
        /// <param name="MutationMax">The absolute value of change in the Mutation value</param>
        public void Mutate(float MutationChance,float MutationMax)
        {

            // We iterate through each hidden layer making each neuron mutate
            for (int i = 0; i < hiddenLayers.Count();i++)
            {
                for(int j = 0; j < hiddenLayers[i].Count();j++)
                {
                    BaseNeuron currentNeuron = hiddenLayers[i][j];

                    currentNeuron.Mutate(MutationChance,MutationMax);
                }
            }

            // We iterate through the ouput layer making each neuron mutate
            for (int j = 0; j < outputNeuron.Count();j++)
            {
                BaseNeuron currentNeuron = outputNeuron[j];

                currentNeuron.Mutate(MutationChance,MutationMax);
            }
        }

        /// <summary>
        /// Main running method from the neural network to process 
        /// </summary>
        /// <param name="inputValues">the value input values of the current process </param>
        /// <returns>The index of the index of the max output neuron</returns>
        public int Run(double[] inputValues)
        {
            // We set the current input value in the correspondings neuron
            for(int i = 0; i< inputValues.Count();i++)
            {
                inputNeuron[i].output = inputValues[i];
            }

            // We set the input layer as previous layer
            List<BaseNeuron> previousLayer = inputNeuron;

            List<Task> currentThreads;
            double[] previousLayerOutputs;

            // for each hidden layer calcutate the new output value
            for (int i = 0; i < hiddenLayers.Count();i++)
            {
                 
                currentThreads = new List<Task>();
                previousLayerOutputs = new double[previousLayer.Count()];

                for(int j = 0; j < previousLayer.Count(); j++)
                {
                    previousLayerOutputs[j] = previousLayer[j].output;
                }

                for(int j = 0;j < hiddenLayers[i].Count();j++)
                {
                    BaseNeuron currNeuron = hiddenLayers[i][j];
                    currentThreads.Add(Task.Run(() => {currNeuron.Calculate(previousLayerOutputs);}));
                }

                previousLayer = hiddenLayers[i];
                Task.WaitAll(currentThreads.ToArray());

            }


            currentThreads = new List<Task>();
            previousLayerOutputs = new double[previousLayer.Count()];

            for (int j = 0; j < previousLayer.Count(); j++)
            {
                previousLayerOutputs[j] = previousLayer[j].output;
            }
            
            // for the output layer calculate the new output value
            for(int j = 0;j < outputNeuron.Count();j++)
            {
                BaseNeuron currentNeuron = outputNeuron[j];
                currentThreads.Add(Task.Run(() => {currentNeuron.Calculate(previousLayerOutputs);}));
            }

            Task.WaitAll(currentThreads.ToArray());
            
            // We retrieve the max output 
            var (_, maxIndex) = outputNeuron.Select((x, i) => (x.output, i)).Max();
    
            return maxIndex;
        }
    }

}