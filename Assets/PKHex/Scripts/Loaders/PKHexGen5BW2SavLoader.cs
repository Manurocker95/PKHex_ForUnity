using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen5BW2SavLoader : PKHexSavLoader<SAV5B2W2>
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

        public static SAV5B2W2 LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV5B2W2 savefile = new SAV5B2W2(bytess);

            return savefile;
        }

        public override SAV5B2W2 LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
