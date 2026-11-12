// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon XD: Gale of Darkness.
    /// </summary>
    public class PKHexGen3XDSavLoader : PKHexSavLoader<SAV3XD>
    {
        public static SAV3XD LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV3XD>(_sav);
        }
    }
}
#endif
