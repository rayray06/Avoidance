
namespace AvoidanceLight.Models.AINetwork
{
    /// <summary>
    /// Class defining the basic working of a neuron
    /// </summary>
    public class BaseNeuron
    {
        /// <summary>
        /// The weigths list where each weigth correspond to the next neuron 
        /// </summary>
        protected List<double> weigths { get; set; } = new List<double>();
        /// <summary>
        /// Random object used throughout the object
        /// </summary>
        protected Random rdn = new Random();

        /// <summary>
        /// The output value of the neuron
        /// </summary>
        public double output { get; set; }
        
        /// <summary>
        /// Base Constructor of the neuron
        /// </summary>
        /// <param name="NeuronCount">Number of previous neuron</param>
        /// <param name="maxweights">Maximum Weight value </param>
        /// <param name="minweights">Minimum Weigth value </param>
        public BaseNeuron(int NeuronCount,double maxweights,double minweights)
        {
            // For each neuron we create give a random weigth
            for(int i = 0; i < NeuronCount; i++)
            {
                this.weigths.Add(minweights+(this.rdn.NextDouble()*(maxweights-minweights)));
            }
        }

        /// <summary>
        /// Constructonr used to copy a weigth list to the new neuron
        /// </summary>
        /// <param name="weightList">the weigth list to copy</param>
        public BaseNeuron(List<int> weightList)
        {
            // We copy each weigth and set them at the same position
            for(int i = 0; i < weightList.Count(); i++)
            {
                this.weigths.Add(weightList[i]);
            }
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public BaseNeuron()
        {

        }

        /// <summary>
        /// Inheritance how the behavior from two define parent
        /// </summary>
        /// <param name="firstParent">Equivalenet neuron from the first parent</param>
        /// <param name="secondParent">Equivalent neuron from the second parent</param>
        public void Inherit(BaseNeuron firstParent,BaseNeuron secondParent)
        {
            // For each weigth we chose randomly between the two parent values
            for(int i = 0; i < weigths.Count(); i++ )
            {
                double chance = rdn.NextDouble();
                weigths[i] = (chance < 0.5)?firstParent.weigths[i]:secondParent.weigths[i];
            }
        }

        /// <summary>
        /// Apply random changes to the neuron weigths
        /// </summary>
        /// <param name="MutationChance">The mutation chances between 0-100</param>
        /// <param name="MutationMax">The absolute value of change in the Mutation value</param>
        public void Mutate(float MutationChance,float MutationMax)
        {
            // for each weigth process the mutation
            for(int i = 0; i < weigths.Count(); i++ )
            {
                double chance = rdn.NextDouble()*100;
                // if mutation randomly modify the weigth value
                if(chance < MutationChance)
                {
                    double value = (rdn.NextDouble()*(MutationMax*2))-MutationMax;
                    weigths[i] = weigths[i] + value; 
                }
            }


        }

        /// <summary>
        /// Calculate the neuron value
        /// </summary>
        /// <param name="input">The list of input corresponding to the previous neuron</param>
        public void Calculate(double[] input)
        {
            this.output = 0;
            // for each neuron we add to the output the input multiplied by the weigth
            for(int i = 0; i<input.Count() ; i++)
            {
                this.output += input[i] * this.weigths[i];
            }
        }
    }

}