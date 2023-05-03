using AvoidanceNEAT.Models.AINetwork.Layers;
using AvoidanceNEAT.Models.AINetwork.Object;

namespace AvoidanceNEAT.Models.AINetwork.Neurons
{
    /// <summary>
    /// Class defining the basic working of a neuron
    /// </summary>
    public class BaseNeuron
    {
        /// <summary>
        /// The weigths list where each weigth correspond to the next neuron 
        /// </summary>
        protected List<Link> previousLayerLinks { get; set; } = new List<Link>();
        /// <summary>
        /// Random object used throughout the object
        /// </summary>
        protected Random rdn = new Random();

        /// <summary>
        /// The output value of the neuron
        /// </summary>
        public double output { get; protected set; }

        /// <summary>
        /// Base Constructor of the neuron
        /// </summary>
        /// <param name="PreviousLayer">The layer of inputValues</param>
        /// <param name="maxweights">Maximum Weight value </param>
        /// <param name="minweights">Minimum Weigth value </param>
        public BaseNeuron(Layer PreviousLayer, double maxweights, double minweights)
        {
            // For each neuron we create give a random weigth
            foreach (BaseNeuron neuron in PreviousLayer.Neurons)
            {

                double weigth = minweights + rdn.NextDouble() * (maxweights - minweights);
                previousLayerLinks.Add(new Link(weigth, neuron));
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
        public void Inherit(BaseNeuron firstParent, BaseNeuron secondParent)
        {
            // For each weigth we chose randomly between the two parent values
            for (int i = 0; i < previousLayerLinks.Count(); i++)
            {
                double chance = rdn.NextDouble();
                double weight = chance < 0.5 ? firstParent.previousLayerLinks[i].weight : secondParent.previousLayerLinks[i].weight;
                previousLayerLinks[i] = new Link(weight, previousLayerLinks[i].InputNeuron);
            }
        }

        /// <summary>
        /// Apply random changes to the neuron weigths
        /// </summary>
        /// <param name="MutationChance">The mutation chances between 0-100</param>
        /// <param name="MutationMax">The absolute value of change in the Mutation value</param>
        public void Mutate(float MutationChance, float MutationMax)
        {
            // for each weigth process the mutation
            for (int i = 0; i < previousLayerLinks.Count(); i++)
            {
                double chance = rdn.NextDouble() * 100;
                // if mutation randomly modify the weigth value
                if (chance < MutationChance)
                {
                    double value = rdn.NextDouble() * (MutationMax * 2) - MutationMax;
                    previousLayerLinks[i].weight = previousLayerLinks[i].weight + value;
                }
            }


        }

        /// <summary>
        /// Calculate the neuron value
        /// </summary>
        public void Calculate()
        {
            output = 0;
            // for each neuron we add to the output the input multiplied by the weigth
            foreach (Link link in previousLayerLinks)
            {
                output += link.getResult();
            }
        }
    }

}