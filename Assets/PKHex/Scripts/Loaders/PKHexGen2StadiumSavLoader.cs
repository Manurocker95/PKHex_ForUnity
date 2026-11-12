// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Stadium 2.
    /// </summary>
    public class PKHexGen2StadiumSavLoader : PKHexSavLoader<SAV2Stadium>
    {
        public static SAV2Stadium LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV2Stadium>(_sav);
        }
    }
}
#endif
