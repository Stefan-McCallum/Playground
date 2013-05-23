using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTree.Data
{
    [Serializable]
    public class Output : Feature
    {
        public Output(string value, string @class)
            : base(value, @class)
        {
        }
    }
}
