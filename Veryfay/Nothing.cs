using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    public sealed class Nothing
    {
        private Nothing() { }
        private readonly static Nothing atAll = new Nothing();
        public static Nothing AtAll { get { return atAll; } }
    }
}
