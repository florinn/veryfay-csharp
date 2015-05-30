using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veryfay
{
    public interface Role<TPrincipal, TExtraInfo>
    {
        bool Contains(TPrincipal principal, TExtraInfo extraInfo = default(TExtraInfo));
    }
}
