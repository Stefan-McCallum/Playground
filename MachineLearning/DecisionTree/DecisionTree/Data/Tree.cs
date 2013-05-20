using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;

namespace DecisionTree.Data
{
    public class Tree
    {
        public String Axis { get; set; }

        public Output Leaf { get; set; }

        public Dictionary<Feature, Tree> Branches { get; set; }

        public void DisplayTree(int tab = 0)
        {
            Action tabWriter = () => Enumerable.Range(0, tab).ForEach(i => Console.Write("\t"));                
            
            if (Branches != null)
            {
                foreach (var feature in Branches)
                {                    
                    tabWriter();

                    Console.WriteLine(feature.Key);

                    feature.Value.DisplayTree(tab + 1);
                }
            }
            else
            {
                tabWriter();                
                Console.WriteLine(Leaf);
            }
        }

        public Output ProcessInstance(Instance i)
        {
            return null;
        }

        private Tree TreeForFeature(Feature feature)
        {
            var branch = Branches.FirstOrDefault(i => i.Key.Axis == feature.Axis);

            return branch.Value;            
        }

        private Tree TreeForInstance(Instance instance)
        {
            foreach (var item in Branches.Keys)
            {
                if (instance.Features.Any(feature => item.Axis == feature.Axis))
                {
                    return Branches[item];
                }
            }

            return null;
        }
    }
}
