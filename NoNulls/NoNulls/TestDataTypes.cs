using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace NoNulls
{
    public class User
    {
        public virtual School School { get; set; }
    }

    public class School
    {
        public virtual District District { get; set; }
    }

    public class District
    {
        public virtual Street Street { get; set; }
    }

    public class Street
    {
        public String Name { get; set; }
    }
}


    


