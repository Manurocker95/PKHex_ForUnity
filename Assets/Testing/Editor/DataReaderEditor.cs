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

            myTarget.PokemonIndex = EditorGUILayout.IntSlider("Pok�mon Index", myTarget.PokemonIndex, 1, 898);
            myTarget.Language = (LanguageID)EditorGUILayout.EnumPopup("Language", myTarget.Language);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Pok�mon Data"))
            {
                myTarget.GenerateMonster();
            }
        }
    }
}
#endif