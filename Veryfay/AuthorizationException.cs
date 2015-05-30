using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string msg) : base(msg) { }
    }
}
