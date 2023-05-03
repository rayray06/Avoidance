using System.Threading.Tasks;
using AvoidanceNEAT.Models.AINetwork.Layers;
using AvoidanceNEAT.Models.AINetwork.Neurons;

namespace AvoidanceNEAT.Models.AINetwork
{
    /// <summary>
    /// Class defining the neural network processing
    /// </summary>
    public class NeuralNetwork
    {
        /// <summary>
        /// List corresponding to the input neurons
        /// </summary>
        public InputLayer inputLayer { get; protected set; }
        
        /// <summary>
        /// List containing each hidden layers containing each one their number of neurons
        /// </summary>
        public List<Layer> hiddenLayers { get; protected set; } = new List<Layer>();
        
        /// <summary>
        /// List containing each output Neuron 
        /// </summary>
        public Layer outputLayer { get; protected set; }

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

            inputLayer = new InputLayer(inputNeuronCount);
            Layer previousLayer = inputLayer;

            // for each hidden layer define the neuron with the number of weigths corresponding at the previous layer size
            foreach (int nbNeuron in hiddenLayersConfig)
            {
                Layer newLayer = new Layer(previousLayer,nbNeuron,minweights,maxweights);
                hiddenLayers.Add(newLayer);
                previousLayer = newLayer;

            }

            outputLayer = new Layer(previousLayer, outputNeuronCount, minweights, maxweights);
            
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

                hiddenLayers[i].Inherit(firstParent.hiddenLayers[i], secondParent.hiddenLayers[i]);
            }

            outputLayer.Inherit(firstParent.outputLayer, secondParent.outputLayer);
            
        }

        /// <summary>
        /// Mutation function to mutate the neural wetwork
        /// </summary>
        /// <param name="MutationChance">The mutation chances between 0-100</param>
        /// <param name="MutationMax">The absolute value of change in the Mutation value</param>
        public void Mutate(float MutationChance,float MutationMax)
        {

            // We iterate through each hidden layer making each neuron mutate
            foreach (Layer layer in hiddenLayers)
            {
                layer.Mutate(MutationChance, MutationMax);
            }

            // We iterate through the ouput layer making each neuron mutate
            outputLayer.Mutate(MutationChance, MutationMax);
        }

        /// <summary>
        /// Main running method from the neural network to process 
        /// </summary>
        /// <param name="inputValues">the value input values of the current process </param>
        /// <returns>The index of the index of the max output neuron</returns>
        public int Run(double[] inputValues)
        {
            // We set the current input value in the correspondings neuron
            inputLayer.loadInput(inputValues);

           foreach(Layer layer in hiddenLayers)
            {
                layer.Run();
            }



            // for the output layer calculate the new output value
            outputLayer.Run();
            
            // We retrieve the max output 
            var (_, maxIndex) = outputLayer.Neurons.Select((x, i) => (x.output, i)).Max();
    
            return maxIndex;
        }
    }

}