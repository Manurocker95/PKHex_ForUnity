using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGenDPSavLoader : PLGU_PKHexSavLoader<SAV4DP>
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

        public static SAV4DP LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV4DP savefile = new SAV4DP(bytess);

            return savefile;
        }

        public override SAV4DP LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV4DP savefile = new SAV4DP(bytess);

            return savefile;
        }        
    }
}
