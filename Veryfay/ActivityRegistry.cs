using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    internal class ActivityRegistry
    {
        private Dictionary<Type, PermissionSet[]> registeredPermissions = new Dictionary<Type, PermissionSet[]>();

        internal void Add(Activity activity, PermissionSet ps)
        {
            if (activity is Container)
                foreach (var a in (activity as Container).Activities)
                    this.Add(a, ps);
            else
                this.AddActivityPermissions(activity, ps);
        }

        private void AddActivityPermissions(Activity activity, PermissionSet ps)
        {
            PermissionSet[] activityPermissionList = new PermissionSet[0];
            var key = activity.GetType();
            if (registeredPermissions.ContainsKey(key))
                activityPermissionList = registeredPermissions[key];
            registeredPermissions[key] = activityPermissionList.Concat(new PermissionSet[] { ps }).ToArray();
        }

        internal PermissionSet[] Get(Activity activity)
        {
            PermissionSet[] permissionSets;
            if (registeredPermissions.TryGetValue(activity.GetType(), out permissionSets))
                return permissionSets;
            else
            {
                string msg = string.Format("no registered activity of type {0}", activity);
                throw new KeyNotFoundException(msg);
            }
        }
    }
}
