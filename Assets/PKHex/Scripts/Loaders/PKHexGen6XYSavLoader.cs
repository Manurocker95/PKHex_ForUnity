// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for X and Y.
    /// </summary>
    public class PKHexGen6XYSavLoader : PKHexSavLoader<SAV6XY>
    {
        public static SAV6XY LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV6XY>(_sav);
        }
    }
}
#endif
