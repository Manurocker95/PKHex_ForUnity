using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Stadium (international).
    /// </summary>
    public class PKHexGen1StadiumSavLoader : PKHexSavLoader<SAV1Stadium>
    {
        public static SAV1Stadium LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV1Stadium>(_sav);
        }
    }
}
