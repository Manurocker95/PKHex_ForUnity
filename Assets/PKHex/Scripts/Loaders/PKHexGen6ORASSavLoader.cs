// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Omega Ruby and Alpha Sapphire.
    /// </summary>
    public class PKHexGen6ORASSavLoader : PKHexSavLoader<SAV6AO>
    {
        public static SAV6AO LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV6AO>(_sav);
        }
    }
}
#endif
