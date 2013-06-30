using System.Collections.Generic;

namespace SharpJags.CodaParser
{
    public class ModelParameterVector : IModelParameter
    {
        public readonly List<ModelParameter> Parameters;

        public ModelParameterVector()
        {
            Parameters = new List<ModelParameter>();
        }

        public ModelParameter this[int key]
        {
            get
            {
                return Parameters[key];
            }
        }
    }
}
