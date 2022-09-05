
namespace AvoidanceLight.Models.AINetwork
{

    public class BaseNeuron
    {

        protected List<double> weigths { get; set; } = new List<double>();
        protected Random rdn = new Random();

        public double output { get; set; }
        
        public BaseNeuron(int NeuronCount,double maxweights,double minweights)
        {
            for(int i = 0; i < NeuronCount; i++)
            {
                this.weigths.Add(minweights+(this.rdn.NextDouble()*(maxweights-minweights)));
            }
        }

        public BaseNeuron(List<int> weightList)
        {
            for(int i = 0; i < weightList.Count(); i++)
            {
                this.weigths.Add(weightList[i]);
            }
        }

        public BaseNeuron()
        {

        }

        public void Inherit(BaseNeuron firstParent,BaseNeuron secondParent)
        {
            for(int i = 0; i < weigths.Count(); i++ )
            {
                double chance = rdn.NextDouble();
                weigths[i] = (chance < 0.5)?firstParent.weigths[i]:secondParent.weigths[i];
            }
        }

        public void Mutate(float MutationChance,float MutationMax)
        {
            for(int i = 0; i < weigths.Count(); i++ )
            {
                double chance = rdn.NextDouble()*100;
                if(chance < MutationChance)
                {
                    double value = (rdn.NextDouble()*(MutationMax*2))-MutationMax;
                    weigths[i] = weigths[i] + value; 
                }
            }


        }

        public void Calculate(double[] input)
        {
            this.output = 0;
            for(int i = 0; i<input.Count() ; i++)
            {
                this.output += input[i] * this.weigths[i];
            }
        }
    }

}