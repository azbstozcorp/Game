namespace CoreModule.Terrain {
    public enum TerrainType : byte {
        TT_UNDEFINED = 0,

        TT_AIR,
        TT_DIRT,
        TT_GRASSTOP,
        TT_GRASSMID,
        TT_GRASSBOT,
        TT_FLOWER,
        TT_STONE,

        TT_COUNT // Always keep last
    }
}
