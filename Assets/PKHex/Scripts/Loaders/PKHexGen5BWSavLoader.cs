// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Black and White.
    /// </summary>
    public class PKHexGen5BWSavLoader : PKHexSavLoader<SAV5BW>
    {
        public static SAV5BW LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV5BW>(_sav);
        }
    }
}
#endif
