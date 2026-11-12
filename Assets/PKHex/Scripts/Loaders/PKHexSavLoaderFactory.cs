using System.IO;
using PKHeX.Core;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Loads save files with automatic game detection and creates the matching loader.
    /// </summary>
    /// <remarks>
    /// Uses explicit type checks instead of MakeGenericType/dynamic so it works on Unity 5.6 (.NET 3.5) and AOT/IL2CPP.
    /// </remarks>
    public static class PKHexSavLoaderFactory
    {
        /// <summary> Loads and detects any supported save file; null (with a log) when missing or unsupported. </summary>
        public static SaveFile LoadSaveFileFromPath(string _path)
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path))
            {
                Debug.LogError("[PKHex] Save file not found: " + _path);
                return null;
            }

            SaveFile sav;
            try
            {
                sav = RuntimeSaveData.LoadSaveFile(File.ReadAllBytes(_path), _path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[PKHex] Failed to read save file " + _path + ": " + ex);
                return null;
            }

            if (sav == null)
            {
                Debug.LogError("[PKHex] Unsupported or corrupt save file: " + _path);
            }
            return sav;
        }

        /// <summary> Loads a save file and checks that it is of type <typeparamref name="T"/>. </summary>
        public static T LoadSaveFileFromPath<T>(string _path) where T : SaveFile
        {
            SaveFile sav = LoadSaveFileFromPath(_path);
            if (sav == null)
            {
                return null;
            }

            T typed = sav as T;
            if (typed == null)
            {
                Debug.LogError("[PKHex] " + _path + " is a " + sav.GetType().Name + " (" + sav.Version + "), not a " + typeof(T).Name + ".");
            }
            return typed;
        }

        /// <summary> Loads any supported save and returns a loader for it (null when unsupported). </summary>
        public static IPKHexSavLoader CreateFromPath(string _path)
        {
            SaveFile sav = LoadSaveFileFromPath(_path);
            return sav != null ? Create(sav) : null;
        }

        /// <summary> Creates the loader that matches the save type. Unknown types get a generic loader. </summary>
        public static IPKHexSavLoader Create(SaveFile _sav)
        {
            IPKHexSavLoader loader = CreateEmpty(_sav);
            loader.SetSaveFile(_sav);
            return loader;
        }

        private static IPKHexSavLoader CreateEmpty(SaveFile _sav)
        {
            // Order matters only for class hierarchies; every PKHeX save type below is sealed or a distinct branch.
            if (_sav is SAV1) { return new PKHexGen1SavLoader(); }
            if (_sav is SAV1Stadium) { return new PKHexGen1StadiumSavLoader(); }
            if (_sav is SAV1StadiumJ) { return new PKHexGen1StadiumJSavLoader(); }
            if (_sav is SAV2) { return new PKHexGen2SavLoader(); }
            if (_sav is SAV2Stadium) { return new PKHexGen2StadiumSavLoader(); }
            if (_sav is SAV3RSBox) { return new PKHexGen3BoxSavLoader(); }
            if (_sav is SAV3Colosseum) { return new PKHexGen3ColosseumSavLoader(); }
            if (_sav is SAV3XD) { return new PKHexGen3XDSavLoader(); }
            if (_sav is SAV3) { return new PKHexGen3SavLoader(); }
            if (_sav is SAV4DP) { return new PKHexGenDPSavLoader(); }
            if (_sav is SAV4Pt) { return new PKHexGenPTSavLoader(); }
            if (_sav is SAV4HGSS) { return new PKHexGenHGSSSavLoader(); }
            if (_sav is SAV4BR) { return new PKHexGen4BRSavLoader(); }
            if (_sav is SAV4Ranch) { return new PKHexGen4RanchSavLoader(); }
            if (_sav is SAV5BW) { return new PKHexGen5BWSavLoader(); }
            if (_sav is SAV5B2W2) { return new PKHexGen5BW2SavLoader(); }
            if (_sav is SAV6XY) { return new PKHexGen6XYSavLoader(); }
            if (_sav is SAV6AO) { return new PKHexGen6ORASSavLoader(); }
            if (_sav is SAV6AODemo) { return new PKHexGen6ORASDemoSavLoader(); }
            if (_sav is SAV7SM) { return new PKHexGen7SUMOSavLoader(); }
            if (_sav is SAV7USUM) { return new PKHexGen7USUMSavLoader(); }
            if (_sav is SAV7b) { return new PKHexGen8PLGPESavLoader(); }
            if (_sav is SAV8SWSH) { return new PKHexGen8ShSwSavLoader(); }
            if (_sav is SAV8BS) { return new PKHexGen8BSSavLoader(); }
            if (_sav is SAV8LA) { return new PKHexGen8LASavLoader(); }
            if (_sav is SAV9SV) { return new PKHexGen9SCVISavLoader(); }
            if (_sav is SAV9ZA) { return new PKHexGen9ZASavLoader(); }
            return new PKHexSavLoader<SaveFile>();
        }
    }
}
