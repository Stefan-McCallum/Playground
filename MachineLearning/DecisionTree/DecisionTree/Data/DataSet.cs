using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;

namespace DecisionTree.Data
{
    public class DataSet
    {
        public List<Instance> Instances { get; set; } 

        public int NumberOfFeatures
        {
            get
            {
                var first = Instances.FirstOrDefault();
                if (first != null)
                {
                    return first.Features.Count;
                }
                return 0;
            }
        }

        public Boolean InstancesAreSameClass
        {
            get
            {
                return Instances.SelectMany(i => i.Features).DistinctBy(i => i.Axis).Count() == 1;
            }
        }

        public DataSet Split(Feature feature)
        {
            return Split(feature.Axis, feature.Value);
        }

        public DataSet Split(string axis, string value)
        {            
            return new DataSet
                   {
                       Instances = Instances.Select(i => i.Split(axis, value))
                                            .Where(i => i.Features.Any())
                                            .ToList()
                   };
        }

        public Tree BuildTree()
        {
            if (InstancesAreSameClass || Instances.Count() == 1)
            {
                return LeafTreeForRemainingFeatures();
            }

            var best = Decider.SelectBestAxis(this);

            return SplitByAxis(best);
        }

        private Tree SplitByAxis(string axis)
        {
            if (axis == null)
            {
                return null;
            }

            // split the set on each unique feature value where the feature is 
            // if of the right axis
            var splits = (from feature in UniqueFeatures().Where(a => a.Axis == axis)
                          select new {splitFeature = feature, set = Split(feature)}).ToList();

            var branches = new Dictionary<Feature, Tree>();

            // for each split, either recursively create a new tree
            // or split the final feature outputs into leaf trees
            foreach (var item in splits)
            {
                branches[item.splitFeature] = item.set.BuildTree();
            }

            return new Tree
                   {                       
                       Branches = branches
                   };
        }

        private Tree LeafTreeForRemainingFeatures()
        {
            if (Instances.Count() > 1)
            {
                var groupings = Instances.DistinctBy(i => i.Output.Value)
                                         .ToDictionary(i => i.Features.First(), j => new Tree
                                                                                     {
                                                                                         Leaf = j.Output
                                                                                     });

                if (groupings.Count() > 1)
                {
                    return new Tree
                           {
                               Branches = groupings
                           };
                }

                return new Tree
                       {
                           Leaf = groupings.First().Value.Leaf
                       };
            }
            
            return new Tree
                   {
                       Leaf = Instances.First().Output
                   };
        }

        public IEnumerable<Feature> UniqueFeatures()
        {
            return Instances.SelectMany(f => f.Features).DistinctBy(f => f.Axis + f.Value).ToList();
        } 
    }
}
