using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGenHGSSSavLoader : PKHexSavLoader<SAV4HGSS>
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

        public static SAV4HGSS LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV4HGSS savefile = new SAV4HGSS(bytess);

            return savefile;
        }

        public override SAV4HGSS LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
