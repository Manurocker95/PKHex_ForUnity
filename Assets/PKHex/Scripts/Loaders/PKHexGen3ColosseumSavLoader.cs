// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Colosseum.
    /// </summary>
    public class PKHexGen3ColosseumSavLoader : PKHexSavLoader<SAV3Colosseum>
    {
        public static SAV3Colosseum LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV3Colosseum>(_sav);
        }
    }
}
#endif
