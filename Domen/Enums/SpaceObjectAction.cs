using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Enums
{
    public enum SpaceObjectAction
    {
        None,
        Approach,
        AlignTo,
        WarpTo,

        Orbit,
        OpenCargo,
        Dock,
        Jump,
        ActivateGate,

        KeepAtRange,
        LockTarget,
        UnLockTarget
    }
}
