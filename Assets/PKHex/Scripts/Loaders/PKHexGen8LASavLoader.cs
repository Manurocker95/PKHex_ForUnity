using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen8LASavLoader : PKHexSavLoader<SAV8LA>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? 0 : 0; // TODO look for offset here
            }
        }

        public static SAV8LA LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV8LA savefile = new PKHeX.Core.SAV8LA(bytess);

            return savefile;
        }

        public override SAV8LA LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV8LA savefile = new PKHeX.Core.SAV8LA(bytess);

            return savefile;
        }
    }
}
