using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Wizet.Audio.Dmo
{
    [Flags]
    enum DmoInputStatusFlags
    {
        None,
        DMO_INPUT_STATUSF_ACCEPT_DATA	= 0x1
    }
}
