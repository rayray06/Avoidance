using AvoidanceNEAT.Models.AINetwork.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvoidanceNEAT.Models.AINetwork
{
    /// <summary>
    /// 
    /// </summary>
    public class InputNeuron:BaseNeuron
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public InputNeuron():base()
        {

        }

        /// <summary>
        /// Set the inputs of the layer
        /// </summary>
        /// <param name="input">Input values</param>
        public void setInput(double input)
        {
            output = input;
        }
    }
}
