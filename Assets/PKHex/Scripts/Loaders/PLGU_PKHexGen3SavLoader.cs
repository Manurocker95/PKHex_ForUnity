using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen3SavLoader : PLGU_PKHexSavLoader<SAV3>
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

        public static SAV3 LoadSaveFileFromPath(string path)
        {
            byte[] bytess = File.ReadAllBytes(path);
            PKHeX.Core.SaveFile sav = null;
            SAV3 savefile = null;
            var other = FileUtil.GetSupportedFile(path, sav);
            if (other is SaveFile s)
            {
                s.Metadata.SetExtraInfo(path);
                sav = s;
            }

            if (sav != null && sav is SAV3 s3)
            {
                if (sav is SAV3FRLG)
                {
                    savefile = s3.ForceLoad(GameVersion.FRLG);
                }
                else
                {
                    savefile = s3.ForceLoad(GameVersion.RSE);
                }
            }

            //SAV3 savefile = new PKHeX.Core.SAV3(bytess, GameVersion.RSE);

            return savefile;
        }

        public override SAV3 LoadSaveFile(string path)
        {
            byte[] bytess = File.ReadAllBytes(path);
            PKHeX.Core.SaveFile sav = null;
            SAV3 savefile = null;
            var other = FileUtil.GetSupportedFile(path, sav);
            if (other is SaveFile s)
            {
                s.Metadata.SetExtraInfo(path);
                sav = s;
            }

            if (sav != null && sav is SAV3 s3)
            {
                if (sav is SAV3FRLG)
                {
                    savefile = s3.ForceLoad(GameVersion.FRLG);
                }
                else
                {
                    savefile = s3.ForceLoad(GameVersion.RSE);
                }
            }
            return savefile;
        }        
    }
}
