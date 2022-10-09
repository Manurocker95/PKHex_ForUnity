#if PKHEX_FOR_UNITY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PKHeX.Core;

namespace PKHexForUnity
{
    [CustomEditor(typeof(DataReader))]
    public class DataReaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DataReader myTarget = (DataReader)target;

            if (myTarget == null)
                return;

            EditorGUILayout.LabelField("PKHex For Unity ");
            myTarget.PokemonIndex = EditorGUILayout.IntSlider("Pokémon Index", myTarget.PokemonIndex, 1, 905);
            myTarget.Language = (LanguageID)EditorGUILayout.EnumPopup("Language", myTarget.Language);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Pokémon Data"))
            {
                myTarget.GenerateMonster();
            }


            if (GUILayout.Button("Show Full Dex List"))
            {
                myTarget.ShowFullDexList();
            }

            GUILayout.Space(20);

            EditorGUILayout.LabelField("PokeAPI");
            myTarget.PokeAPILoader = (PokeAPI.PokeAPILoader)EditorGUILayout.ObjectField(myTarget.PokeAPILoader, typeof(PokeAPI.PokeAPILoader), true);

            GUILayout.Space(20);
            
            if (myTarget.PokeAPILoader)
            {
                if (GUILayout.Button("Save Full Dex List From API"))
                    myTarget.PokeAPILoader.FillDexFromAPI();
            }
            else
            {
                EditorGUILayout.LabelField("Add reference for PokeAPI Loader");
            }
        }
    }
}
#endif