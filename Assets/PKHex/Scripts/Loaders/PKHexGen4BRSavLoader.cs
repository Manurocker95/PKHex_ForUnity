// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Battle Revolution.
    /// </summary>
    public class PKHexGen4BRSavLoader : PKHexSavLoader<SAV4BR>
    {
        public static SAV4BR LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4BR>(_sav);
        }
    }
}
#endif
