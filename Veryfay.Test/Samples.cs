using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay.Test
{
    public class SomeClass { }
    public class SomeOtherClass { }
    public class OtherSomeOtherClass { }

    public class PrincipalClass 
    {
        public PrincipalClass(string username)
        {
            this.Username = username;
        }

        public string Username { get; private set; } 
    }
    public class OtherPrincipalClass
    {
        public OtherPrincipalClass(string username)
        {
            this.Username = username;
        }

        public string Username { get; private set; } 
    }

    public sealed class Admin : Role<PrincipalClass, int>
    {
        private static readonly Lazy<Admin> instance = new Lazy<Admin>(() => new Admin());

        public static Admin Instance { get { return instance.Value; } }

        private Admin() { }

        public bool Contains(PrincipalClass principal, int extraInfo = default(int))
        {
            return principal.Username == "admin";
        }
    }

    public sealed class Supervisor : Role<PrincipalClass, string>
    {
        private static readonly Lazy<Supervisor> instance = new Lazy<Supervisor>(() => new Supervisor());

        public static Supervisor Instance { get { return instance.Value; } }

        private Supervisor() { }

        public bool Contains(PrincipalClass principal, string extraInfo = default(string))
        {
            return principal.Username == "supervisor" || 
                principal.Username == "supervisor-commiter";
        }
    }

    public sealed class Commiter : Role<PrincipalClass, string>
    {
        private static readonly Lazy<Commiter> instance = new Lazy<Commiter>(() => new Commiter());

        public static Commiter Instance { get { return instance.Value; } }

        private Commiter() { }
        
        public bool Contains(PrincipalClass principal, string extraInfo = default(string))
        {
            return principal.Username == "commiter" ||
                principal.Username == "supervisor-commiter";
        }
    }

    public sealed class Contributor : Role<OtherPrincipalClass, int>
    {
        private static readonly Lazy<Contributor> instance = new Lazy<Contributor>(() => new Contributor());

        public static Contributor Instance { get { return instance.Value; } }

        private Contributor() { }

        public bool Contains(OtherPrincipalClass principal, int extraInfo = default(int))
        {
            return principal.Username == "contributor" ||
                principal.Username == "contributor-reader";
        }
    }

    public sealed class Reviewer : Role<PrincipalClass, int>
    {
        private static readonly Lazy<Reviewer> instance = new Lazy<Reviewer>(() => new Reviewer());

        public static Reviewer Instance { get { return instance.Value; } }

        private Reviewer() { }

        public bool Contains(PrincipalClass principal, int extraInfo = default(int))
        {
            return principal.Username == "contributor" ||
                principal.Username == "commiter";
        }
    }

    public sealed class Reader : Role<OtherPrincipalClass, Tuple<int, string>>
    {
        private static readonly Lazy<Reader> instance = new Lazy<Reader>(() => new Reader());

        public static Reader Instance { get { return instance.Value; } }

        private Reader() { }

        public bool Contains(OtherPrincipalClass principal, Tuple<int, string> extraInfo = default(Tuple<int, string>))
        {
            return principal.Username == "reader" &&
                extraInfo.Item1 == 1234;
        }
    } 
}
