using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen8ShSwSavLoader : PKHexSavLoader<SAV8SWSH>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Badges : 0;
            }
        }

        public static SAV8SWSH LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV8SWSH savefile = new PKHeX.Core.SAV8SWSH(bytess);

            return savefile;
        }

        public override SAV8SWSH LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV8SWSH savefile = new PKHeX.Core.SAV8SWSH(bytess);

            return savefile;
        }
    }
}
