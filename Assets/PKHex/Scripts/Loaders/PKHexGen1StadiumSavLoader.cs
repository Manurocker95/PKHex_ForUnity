using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen1StadiumSavLoader : PKHexSavLoader<SAV1Stadium>
    {
        public override string PlayerName
        {
            get
            {
                return m_saveFile != null ? m_saveFile.OT : base.PlayerName;
            }
        }

        public static SAV1Stadium LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV1Stadium savefile = new PKHeX.Core.SAV1Stadium(bytess);

            return savefile;
        }

        public override SAV1Stadium LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV1Stadium savefile = new PKHeX.Core.SAV1Stadium(bytess);

            return savefile;
        }
    }
}
