using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DecisionTree.Data
{
    [DataContract]
    public class Feature
    {
        [DataMember]
        public string Value { get; set; }

        [DataMember]
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
