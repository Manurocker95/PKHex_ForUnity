using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen2SavLoader : PLGU_PKHexSavLoader<SAV2>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Badges : 0;
            }
        }

        public override uint Coins
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Coin : 0;
            }
        }

        public static SAV2 LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV2 savefile = new PKHeX.Core.SAV2(bytess, GameVersion.GSC);

            return savefile;
        }

        public override SAV2 LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV2 savefile = new PKHeX.Core.SAV2(bytess, GameVersion.GSC);

            return savefile;
        }        
    }
}
