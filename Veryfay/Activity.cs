using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay
{
    public interface Activity { }
    public interface Activity<TTarget> : Activity { }
    public interface Container
    {
        Activity[] Activities { get; }
    }

    public sealed class Create<TTarget> : Activity<TTarget> { }
    public sealed class Read<TTarget> : Activity<TTarget> { }
    public sealed class Update<TTarget> : Activity<TTarget> { }
    public sealed class Patch<TTarget> : Activity<TTarget> { }
    public sealed class Delete<TTarget> : Activity<TTarget> { }

    public sealed class CRUD<TTarget> : Activity<TTarget>, Container
    {
        private Activity<TTarget>[] activities =
            new Activity<TTarget>[] { new Create<TTarget>(), new Read<TTarget>(), new Update<TTarget>(), new Delete<TTarget>() };

        public Activity[] Activities
        {
            get { return activities; }
        }
    }
    public sealed class CRUDP<TTarget> : Activity<TTarget>, Container
    {
        private Activity<TTarget>[] activities =
            new Activity<TTarget>[] { new CRUD<TTarget>(), new Patch<TTarget>() };

        public Activity[] Activities
        {
            get { return activities; }
        }
    }
}
