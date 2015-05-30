using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veryfay
{
    public class ActivityAuthorization
    {
        private Activity activity;
        private ActivityRegistry activityRegistry;

        internal ActivityAuthorization(Activity activity, ActivityRegistry activityRegistry)
        {
            this.activity = activity;
            this.activityRegistry = activityRegistry;
        }

        public IsAllowingResult IsAllowing<TPrincipal>(TPrincipal principal)
        {
            return this.IsAllowing(principal, Nothing.AtAll);
        }

        public IsAllowingResult IsAllowing<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo)
        {
            return this.Authorize(principal, extraInfo);
        }

        public string Verify<TPrincipal>(TPrincipal principal)
        {
            return this.Verify(principal, Nothing.AtAll);
        }

        public string Verify<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo)
        {
            var result = this.Authorize(principal, extraInfo);

            if (result.IsSuccess)
                return result.Details;
            else
                throw new AuthorizationException(result.Details);
        }

        private IsAllowingResult Authorize<TPrincipal, TExtraInfo>(TPrincipal principal, TExtraInfo extraInfo)
        {
            PermissionSet[] activityPermissions;
            try
            {
                activityPermissions = activityRegistry.Get(activity);
            }
            catch (KeyNotFoundException e)
            {
                return new IsAllowingResult(false, e.Message);
            }

            string details;
            try
            {
                details = PermissionVerifier.Verify(activityPermissions, principal, extraInfo);
            }
            catch (AuthorizationException e)
            {
                return new IsAllowingResult(false, e.Message);
            }

            return new IsAllowingResult(true, details);
        }
    }

    public struct IsAllowingResult
    {
        private bool isAllowing;
        private string details;

        public IsAllowingResult(bool isAllowing, string details)
        {
            this.isAllowing = isAllowing;
            this.details = details;
        }

        public bool IsFailure { get { return !this.isAllowing; } }
        public bool IsSuccess { get { return this.isAllowing; } }
        public string Details { get { return this.details; } }
    }
}
