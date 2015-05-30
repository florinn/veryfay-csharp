using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    public class AuthorizationEngine
    {
        private ActivityRegistry activityRegistry = new ActivityRegistry();

        public PermissionSet Register(Activity activity, params Activity[] moreActivities)
        {
            var ps = new PermissionSet(this);
            activityRegistry.Add(activity, ps);
            foreach (var a in moreActivities)
                activityRegistry.Add(a, ps);
            return ps;
        }

        public ActivityAuthorization this[Activity activity]
        {
            get
            {
                return new ActivityAuthorization(activity, activityRegistry);
            }
        }
    }
}
