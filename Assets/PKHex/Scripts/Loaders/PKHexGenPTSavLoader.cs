// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Platinum.
    /// </summary>
    public class PKHexGenPTSavLoader : PKHexSavLoader<SAV4Pt>
    {
        public static SAV4Pt LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4Pt>(_sav);
        }
    }
}
#endif
