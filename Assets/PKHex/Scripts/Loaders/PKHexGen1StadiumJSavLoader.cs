using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Stadium (Japanese, Pocket Monsters Stadium).
    /// </summary>
    public class PKHexGen1StadiumJSavLoader : PKHexSavLoader<SAV1StadiumJ>
    {
        public static SAV1StadiumJ LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV1StadiumJ>(_sav);
        }
    }
}
