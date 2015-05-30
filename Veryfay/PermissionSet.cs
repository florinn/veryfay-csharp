using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veryfay
{
    public class PermissionSet
    {
        internal List<RoleSet> RoleSets { get; set; }

        public AuthorizationEngine And { get; private set; }

        public PermissionSet(AuthorizationEngine ae)
        {
            this.RoleSets = new List<RoleSet>();
            this.And = ae;
        }

        public PermissionSet Allow<TPrincipal, TExtraInfo>(Role<TPrincipal, TExtraInfo> role, params Role<TPrincipal, TExtraInfo>[] moreRoles)
        {
            var roles = new Role<TPrincipal, TExtraInfo>[] { role }.Concat(moreRoles).ToArray();
            RoleSets.Add(new AllowRoleSet<TPrincipal, TExtraInfo>(roles));
            return this;
        }

        public PermissionSet Deny<TPrincipal, TExtraInfo>(Role<TPrincipal, TExtraInfo> role, params Role<TPrincipal, TExtraInfo>[] moreRoles)
        {
            var roles = new Role<TPrincipal, TExtraInfo>[] { role }.Concat(moreRoles).ToArray();
            RoleSets.Add(new DenyRoleSet<TPrincipal, TExtraInfo>(roles));
            return this;
        }
    }
}
