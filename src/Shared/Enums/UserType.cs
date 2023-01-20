using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Shared.Enums
{
    public enum UserType
    {
        [Description(@"Gestionaire")]
        Gestionaire,

        [Description(@"Salarie")]
        Salarie
    }
}
