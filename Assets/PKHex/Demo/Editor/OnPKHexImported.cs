using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PKHexForUnity
{
    public class OnPKHexImported : AssetPostprocessor
    {
        public const bool CHECK_DEFINE_ON_IMPORT = true;

        protected static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (CHECK_DEFINE_ON_IMPORT && !IsPKHexForUnityAdded)
            {
                bool needToCheck = false;
                foreach (string importedAsset in importedAssets)
                {
                    if (importedAsset.Contains("OnPKHexImported.cs"))
                    {
                        needToCheck = true;
                        break;
                    }
                }

                if (needToCheck)
                {
                    TryAddCoreDefineSymbols();
                }
            }
        }

        public static string[] GetRequiredDefineSymbols()
        {
            return new string[]
            {
                    "PKHEX_FOR_UNITY"
            };
        }

        public static bool NeedToAddDefine(string[] requiredDefines, List<string> defines)
        {
            foreach (var d in requiredDefines)
            {
                if (!defines.Contains(d))
                {
                    return true;
                }
            }

            return false;
        }

#if PKHEX_FOR_UNITY
        public static bool IsPKHexForUnityAdded => true;
#else
        public bool IsPKHexForUnityAdded => false;
#endif


        [MenuItem("PKHex4Unity/Add Default DefineSymbols")]
        public static void TryAddCoreDefineSymbols()
        {
            string[] dfs = GetRequiredDefineSymbols();

            var currentTarget = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;

            if (currentTarget == UnityEditor.BuildTargetGroup.Unknown)
            {
                return;
            }

            var definesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget).Trim();
            var defines = new List<string>(definesString.Split(';'));

            if (NeedToAddDefine(dfs, defines))
            {
                if (EditorUtility.DisplayDialog("Add Define Symbols?","Do you want to add required PKHexForUnity Define Symbols?", "Add them", "Do NOT add them"))
                {
                    AddCoreDefineSymbols(dfs, defines, currentTarget);
                }
            }
        }

        public static void AddCoreDefineSymbols(string[] dfs, List<string> defines, BuildTargetGroup currentTarget)
        {
            foreach (var d in dfs)
            {
                if (!defines.Contains(d))
                {
                    defines.Add(d);
                }
            }

            string s = "";
            foreach (var d in defines)
            {
                s += d + ";";
            }

            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, s);
        }
    }
}


