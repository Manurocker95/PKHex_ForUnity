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
