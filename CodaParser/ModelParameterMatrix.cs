using System.Collections.Generic;

namespace SharpJags.CodaParser
{
    public class ModelParameterMatrix : IModelParameter
    {
        public readonly List<ModelParameterVector> Vectors;

        public ModelParameterMatrix()
        {
            Vectors = new List<ModelParameterVector>();
        }

        public ModelParameterVector this[int key]
        {
            get
            {
                return Vectors[key];
            }
        }
    }
}
