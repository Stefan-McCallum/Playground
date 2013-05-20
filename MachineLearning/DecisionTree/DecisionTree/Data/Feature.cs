using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTree.Data
{
    public class Feature
    {
        public string Value { get; set; }
        public string Axis { get; set; }

        public Feature(string value, string axis)
        {
            Value = value;
            Axis = axis;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Axis, Value);
        }
    }
}
