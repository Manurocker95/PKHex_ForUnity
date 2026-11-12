// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Sun and Moon.
    /// </summary>
    public class PKHexGen7SUMOSavLoader : PKHexSavLoader<SAV7SM>
    {
        public static SAV7SM LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV7SM>(_sav);
        }
    }
}
#endif
