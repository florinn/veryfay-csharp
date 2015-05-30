using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    internal static class PermissionVerifier
    {
        internal static string Verify<TPrincipal, TExtraInfo>(
            PermissionSet[] activityPermissions, TPrincipal principal, TExtraInfo extraInfo)
        {
            StringBuilder resMsg = new StringBuilder();
            var activityPermissionsCuratedRoleSets = CurateRoleSets(activityPermissions, principal, extraInfo);

            foreach (var ap in activityPermissionsCuratedRoleSets)
            {
                var denyRoleSets = ap.RoleSets.Where(x =>
                    x.GetType().GetGenericTypeDefinition() == typeof(DenyRoleSet<TPrincipal, TExtraInfo>).GetGenericTypeDefinition());

                foreach (dynamic denyRoleSet in denyRoleSets)
                {
                    var msg = typeof(TExtraInfo) == typeof(Nothing) ? denyRoleSet.GetMsg(principal) : denyRoleSet.GetMsg(principal, extraInfo);
                    if (typeof(TExtraInfo) == typeof(Nothing) ? denyRoleSet.Check(principal) : denyRoleSet.Check(principal, extraInfo))
                    {
                        resMsg.AppendLine(string.Format("### DENY SET => TRUE:\n {0}", msg));
                        throw new AuthorizationException(resMsg.ToString());
                    }
                    else
                    {
                        resMsg.AppendLine(string.Format("--- DENY SET => FALSE:\n {0}", msg));
                    }
                }
            }

            foreach (var ap in activityPermissionsCuratedRoleSets)
            {
                var allowRoleSets = ap.RoleSets.Where(x =>
                    x.GetType().GetGenericTypeDefinition() == typeof(AllowRoleSet<TPrincipal, TExtraInfo>).GetGenericTypeDefinition());

                foreach (dynamic allowRoleSet in allowRoleSets)
                {
                    var msg = typeof(TExtraInfo) == typeof(Nothing) ? allowRoleSet.GetMsg(principal) : allowRoleSet.GetMsg(principal, extraInfo);
                    if (typeof(TExtraInfo) == typeof(Nothing) ? allowRoleSet.Check(principal) : allowRoleSet.Check(principal, extraInfo))
                    {
                        resMsg.AppendLine(string.Format("### ALLOW SET => TRUE:\n {0}", msg));
                        return resMsg.ToString();
                    }
                    else
                    {
                        resMsg.AppendLine(string.Format("--- ALLOW SET => FALSE:\n {0}", msg));
                    }
                }
            }

            throw new AuthorizationException(string.IsNullOrEmpty(resMsg.ToString()) ? "NO MATCHING ROLE SET FOUND" : resMsg.ToString());
        }

        private static PermissionSet[] CurateRoleSets<TPrincipal, TExtraInfo>(
            PermissionSet[] activityPermissions, TPrincipal principal, TExtraInfo extraInfo)
        {
            var result = from ap in activityPermissions
                         select new PermissionSet(ap.And)
                         {
                             RoleSets = (from rs in ap.RoleSets
                                         where rs.PrincipalType == typeof(TPrincipal) &&
                                         (typeof(TExtraInfo) == typeof(Nothing) || rs.ExtraInfoType == typeof(TExtraInfo))
                                         select rs).ToList()
                         };
            return result.ToArray();
        }
    }
}
