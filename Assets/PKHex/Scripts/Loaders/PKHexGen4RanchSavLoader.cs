// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for My Pokémon Ranch.
    /// </summary>
    public class PKHexGen4RanchSavLoader : PKHexSavLoader<SAV4Ranch>
    {
        public static SAV4Ranch LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4Ranch>(_sav);
        }
    }
}
#endif
