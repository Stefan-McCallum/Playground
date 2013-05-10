using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NoNulls
{
    [TestFixture]
    internal class Tests
    {
        [Test]
        public void NeverNullTest()
        {
            var actions = new List<Func<int, string>>
                          {
                              i =>
                                  {
                                      NonNullNoProxyButNewObjects(i);
                                      return "NonNullWithoutProxyNewingObjects";
                                  },
                              i =>
                                  {
                                      NonNullNoProxy(i);
                                      return "NonNullNoProxy";
                                  },
                              i =>
                                  {
                                      TestNonNullWithProxy(i);
                                      return "NonNullWithProxy";
                                  },

                          };

            foreach (var action in actions)
            {                
                for (int i = 1; i < 5000; i += 100)
                {                    
                    var start = DateTime.Now;

                    var label = action(i);

                    var totalMs = (DateTime.Now - start).TotalMilliseconds;
                    
                    Console.WriteLine(i + " " + label + " " + totalMs);
                }
            }
        }

        private void NullWithProxy(int amount)
        {
            var user = new User();

            var s = "na";
            for (int i = 0; i < amount; i++)
            {
                s += user.NeverNull().School.District.Street.Name.Final() ?? "na";
            }

            Console.WriteLine(s.FirstOrDefault());
        }

        private void TestNonNullWithProxy(int amount)
        {
            var student = new User
            {
                School = new School
                {
                    District = new District
                    {
                        Street = new Street
                        {
                            Name = "Elm"
                        }
                    }
                }
            };

            var s = "na";
            for (int i = 0; i < amount; i++)
            {
                s += student.NeverNull().School.District.Street.Name.Final();
            }

            Console.WriteLine(s.FirstOrDefault());
        }

        private void NonNullNoProxy(int amount)
        {
            var student = new User
                    {
                        School = new School
                                 {
                                     District = new District
                                                {
                                                    Street = new Street
                                                             {
                                                                 Name = "Elm"
                                                             }
                                                }
                                 }
                    };

            var s = "na";
            for (int i = 0; i < amount; i++)
            {
                if (student.School != null)
                {
                    if (student.School.District != null)
                    {
                        if (student.School.District.Street != null)
                        {
                            s += student.School.District.Street.Name;
                        }
                    }
                }
            }

            Console.WriteLine(s.FirstOrDefault());
        }

        private void NonNullNoProxyButNewObjects(int amount)
        {          
            var s = "na";
            for (int i = 0; i < amount; i++)
            {
                var student = new User
                {
                    School = new School
                    {
                        District = new District
                        {
                            Street = new Street
                            {
                                Name = "Elm"
                            }
                        }
                    }
                };

                if (student.School != null)
                {
                    if (student.School.District != null)
                    {
                        if (student.School.District.Street != null)
                        {
                            s += student.School.District.Street.Name;
                        }
                    }
                }
            }

            Console.WriteLine(s.FirstOrDefault());
        }
    }
}
