// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Ruby, Sapphire, Emerald, FireRed and LeafGreen.
    /// </summary>
    public class PKHexGen3SavLoader : PKHexSavLoader<SAV3>
    {
        public static SAV3 LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV3>(_sav);
        }
    }
}
#endif
