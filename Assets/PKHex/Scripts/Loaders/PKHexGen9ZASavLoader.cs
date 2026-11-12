// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Legends: Z-A.
    /// </summary>
    public class PKHexGen9ZASavLoader : PKHexSavLoader<SAV9ZA>
    {
        public static SAV9ZA LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV9ZA>(_sav);
        }
    }
}
#endif
