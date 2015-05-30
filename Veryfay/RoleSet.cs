using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    internal abstract class RoleSet
    {
        internal abstract Type PrincipalType { get; }
        internal abstract Type ExtraInfoType { get; }
    }

    internal abstract class RoleSet<TPrincipal, TExtraInfo> : RoleSet
    {
        internal abstract Role<TPrincipal, TExtraInfo>[] Roles { get; }

        internal override Type PrincipalType { get { return typeof(TPrincipal); } }
        internal override Type ExtraInfoType { get { return typeof(TExtraInfo); } }

        internal bool Check(TPrincipal principal, TExtraInfo extraInfo = default(TExtraInfo))
        {
            var result = true;
            Action<Role<TPrincipal, TExtraInfo>> visitor = (role) =>
                {
                    if (result && !role.Contains(principal, extraInfo))
                        result = false;
                };
            this.Traverse(this.Roles, visitor);
            return result;
        }

        internal string GetMsg(TPrincipal principal, TExtraInfo extraInfo = default(TExtraInfo))
        {
            var msg = string.Empty;
            var breakIt = false;
            Action<Role<TPrincipal, TExtraInfo>> visitor = (role) =>
            {
                if (!breakIt)
                {
                    if (role.Contains(principal, extraInfo))
                    {
                        msg += string.Format("{0} contains {1} and {2} AND\n", role, principal, extraInfo);
                    }
                    else
                    {
                        msg += string.Format("{0} DOES NOT contain {1} and {2}\n", role, principal, extraInfo);
                        breakIt = true;
                    }
                }
            };
            this.Traverse(this.Roles, visitor);
            return msg;
        }

        private void Traverse(Role<TPrincipal, TExtraInfo>[] roles, Action<Role<TPrincipal, TExtraInfo>> visitor)
        {
            foreach (var role in roles)
            {
                visitor(role);
            }
        }
    }

    internal sealed class AllowRoleSet<TPrincipal, TExtraInfo> : RoleSet<TPrincipal, TExtraInfo>
    {
        private Role<TPrincipal, TExtraInfo>[] roles;
        internal override Role<TPrincipal, TExtraInfo>[] Roles { get { return this.roles; } }

        public AllowRoleSet(Role<TPrincipal, TExtraInfo>[] roles)
        {
            this.roles = roles;
        }
    }

    internal sealed class DenyRoleSet<TPrincipal, TExtraInfo> : RoleSet<TPrincipal, TExtraInfo>
    {
        private Role<TPrincipal, TExtraInfo>[] roles;
        internal override Role<TPrincipal, TExtraInfo>[] Roles { get { return this.roles; } }

        public DenyRoleSet(Role<TPrincipal, TExtraInfo>[] roles)
        {
            this.roles = roles;
        }
    }
}
