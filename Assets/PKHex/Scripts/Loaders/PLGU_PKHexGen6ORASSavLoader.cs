using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen6ORASSavLoader : PLGU_PKHexSavLoader<SAV6AO>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Misc.Badges : 0;
            }
        }

        public override uint Money
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Misc.Money : 0;
            }
        }

        public static SAV6AO LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV6AO savefile = new SAV6AO(bytess);

            return savefile;
        }

        public override SAV6AO LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
