using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen2StadiumSavLoader : PKHexSavLoader<SAV2Stadium>
    {
        public override string PlayerName
        {
            get
            {
                return m_saveFile != null ? m_saveFile.OT : base.PlayerName;
            }
        }

        public static SAV2Stadium LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV2Stadium savefile = new PKHeX.Core.SAV2Stadium(bytess);

            return savefile;
        }

        public override SAV2Stadium LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV2Stadium savefile = new PKHeX.Core.SAV2Stadium(bytess);

            return savefile;
        }
    }
}
