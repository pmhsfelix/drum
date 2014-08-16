using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drum
{
    public class RouteNotFoundException : Exception
    {
        public MethodInfo ActionMethod { get; set; }
        
        public RouteNotFoundException(MethodInfo actionMethod)
        {
            ActionMethod = actionMethod;
        }
    }
}
