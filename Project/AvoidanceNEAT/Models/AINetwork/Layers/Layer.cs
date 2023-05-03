using AvoidanceNEAT.Models.AINetwork.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvoidanceNEAT.Models.AINetwork.Layers
{
    /// <summary>
    /// Class describing a layer of neuron 
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Neurons of the layer
        /// </summary>
        public List<BaseNeuron> Neurons { get; protected set; } = new List<BaseNeuron>();

        /// <summary>
        /// Empty constructor
        /// </summary>
        protected Layer()
        {

        }

        /// <summary>
        /// Basic constructor for the neuron layer
        /// </summary>
        /// <param name="previousLayer">The previous layer </param>
        /// <param name="neuronCount">The number of neuron in the current layer</param>
        /// <param name="maxweights">Maximum Weight value </param>
        /// <param name="minweights">Minimum Weigth value </param>
        public Layer(Layer previousLayer, int neuronCount, double minWeight, double maxWeigth)
        {
            // initialize the layer
            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new BaseNeuron(previousLayer, maxWeigth, minWeight));
            }
        }

        /// <summary>
        /// Inherit value from two parent layer
        /// </summary>
        /// <param name="firstParent">Equivalent layer from the first parent</param>
        /// <param name="secondParent">Equivalent layer from the second parent</param>
        public void Inherit(Layer FirstParent, Layer SecondParent)
        {
            // We iterate through the output layer to pass each neuron through the Inherit process
            for (int j = 0; j < Neurons.Count(); j++)
            {
                BaseNeuron currentNeuron = Neurons[j];
                BaseNeuron firstParentNeuron = FirstParent.Neurons[j];
                BaseNeuron secondParentNeuron = SecondParent.Neurons[j];

                currentNeuron.Inherit(firstParentNeuron, secondParentNeuron);
            }
        }

        /// <summary>
        /// Mutate every neuron of the layer
        /// </summary>
        /// <param name="MutationChance">The mutation chances between 0-100</param>
        /// <param name="MutationMax">The absolute value of change in the Mutation value</param>
        public void Mutate(float MutationChance, float MutationMax)
        {
            foreach (BaseNeuron currentNeuron in Neurons)
            {
                currentNeuron.Mutate(MutationChance, MutationMax);
            }

        }

        /// <summary>
        /// Calculate the neuron value for the all layer
        /// </summary>
        public void Run()
        {

            var result = Parallel.ForEach(Neurons, currentNeuron =>
            {
                currentNeuron.Calculate();
            });

        }

        /// <summary>
        /// Return every ouput of the layer as a list of double
        /// </summary>
        /// <returns>The output list</returns>
        public List<double> ToOutputList()
        {
            List<double> outputList = new List<double>();
            for(int i = 0; i < Neurons.Count; i++)
            {
                outputList.Add(Neurons[i].output);
            }
            return outputList;
        }


    }
}
