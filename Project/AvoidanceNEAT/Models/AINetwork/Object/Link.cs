using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AvoidanceNEAT.Models.AINetwork.Neurons;

namespace AvoidanceNEAT.Models.AINetwork.Object
{
    /// <summary>
    /// Link between a weigth and the corresponding neuron
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The weigth value
        /// </summary>
        public double weight;
        /// <summary>
        /// The neuron containing the Input
        /// </summary>
        public BaseNeuron InputNeuron { get; private set; }

        /// <summary>
        /// Base constructor to create the link
        /// </summary>
        /// <param name="weigths">Weigth of the link</param>
        /// <param name="inputNeuron">The input neuron to consider</param>
        public Link(double weigths, BaseNeuron inputNeuron)
        {
            this.weight = weigths;
            InputNeuron = inputNeuron;
        }

        /// <summary>
        /// Retrieve the result of the link
        /// </summary>
        /// <returns>Link value</returns>
        public double getResult()
        {
            return weight*InputNeuron.output;
        }

        


    }
}
