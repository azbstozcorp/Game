using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Terrain {
    public enum TerrainType : int {
        TT_UNDEFINED = 0,

        TT_AIR,
        TT_DIRT,

        TT_COUNT // Always keep last
    }
}
