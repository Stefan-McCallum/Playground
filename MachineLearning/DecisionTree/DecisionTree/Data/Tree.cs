using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;

namespace DecisionTree.Data
{
    public class Tree
    {
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

        public static Output ProcessInstance(Tree tree, Instance i)
        {
            if (tree.Leaf != null)
            {
                return tree.Leaf;
            }

            return ProcessInstance(tree.TreeForInstance(i), i);
        }

        private Tree TreeForFeature(Feature feature)
        {
            var branch = Branches.FirstOrDefault(i => i.Key.Axis == feature.Axis && i.Key.Value == feature.Value);

            if (branch.Equals(default(KeyValuePair<Feature, Tree>)))
            {
                return null;
            }

            return branch.Value;
        }

        private Tree TreeForInstance(Instance instance)
        {
            var tree = instance.Features.Select(TreeForFeature).FirstOrDefault(f => f != null);

            return tree;
        }
    }
}
