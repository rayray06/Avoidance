using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvoidanceNEAT.Models.AINetwork.Layers
{
    /// <summary>
    /// Layer composed of the input neuron
    /// </summary>
    public class InputLayer:Layer
    {
        /// <summary>
        /// Base constructor for an input layer
        /// </summary>
        /// <param name="NeuronCount">The number of neuron in the layer</param>
        public InputLayer(int NeuronCount) : base() {

            for (int i = 0; i < NeuronCount; i++)
            {

                Neurons.Add(new InputNeuron());
            }


        }

        /// <summary>
        /// Load the inputs in the layer
        /// </summary>
        /// <param name="inputs">The imputs values to load</param>
        public void loadInput(double[] inputs)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                ((InputNeuron)Neurons[i]).setInput(inputs[i]);
            }
        }
    }
}
