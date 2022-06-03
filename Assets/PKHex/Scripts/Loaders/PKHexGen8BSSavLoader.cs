using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen8BSSavLoader : PKHexSavLoader<SAV8BS>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? 0 : 0; // TODO look for offset here
            }
        }

        public static SAV8BS LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV8BS savefile = new PKHeX.Core.SAV8BS(bytess);

            return savefile;
        }

        public override SAV8BS LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV8BS savefile = new PKHeX.Core.SAV8BS(bytess);

            return savefile;
        }
    }
}
