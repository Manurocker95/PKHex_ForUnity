// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Black 2 and White 2.
    /// </summary>
    public class PKHexGen5BW2SavLoader : PKHexSavLoader<SAV5B2W2>
    {
        public static SAV5B2W2 LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV5B2W2>(_sav);
        }
    }
}
#endif
