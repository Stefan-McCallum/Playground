using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DecisionTree.Data;

namespace SampleDataParsers
{
    public enum PatientAge
    {
        Young = 1,
        PrePresbyopic = 2,
        Presbypic = 3
    }

    public enum SpectacleRx
    {
        Myope = 1,
        Hypermetrope = 2
    }

    public enum Astigmatic
    {
        No = 1,
        Yes = 2
    }

    public enum TearRateProduction
    {
        Reduced = 1,
        Normal = 2
    }

    public enum LenseType
    {
        Hard = 1,
        Soft = 2,
        None = 3
    }

    public class Lenses : IParser
    {
        public DecisionTreeSet Parse(string file)
        {
            var set = new DecisionTreeSet();

            set.Instances = new List<Instance>();

            using (var stream = new StreamReader(file))
            {
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    set.Instances.Add(ParseLine(line));
                }
            }

            return set;
        }

        private Instance ParseLine(string line)
        {
            var splits = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            var age = Enum.Parse(typeof (PatientAge), splits[1]);
            var rx = Enum.Parse(typeof (SpectacleRx), splits[2]);
            var astigmatic = Enum.Parse(typeof (Astigmatic), splits[3]);
            var tearRate = Enum.Parse(typeof (TearRateProduction), splits[4]);
            var needsContacts = Enum.Parse(typeof (LenseType), splits[5]);

            return new Instance
                   {
                       Output = new Output(needsContacts.ToString(), "contact type"),
                       Features = new List<Feature>
                                  {
                                      new Feature(age.ToString(), "age"),
                                      new Feature(rx.ToString(), "rx"),
                                      new Feature(astigmatic.ToString(), "astigmatic"),
                                      new Feature(tearRate.ToString(), "tearRate")
                                  }
                   };
        }
    }
}
