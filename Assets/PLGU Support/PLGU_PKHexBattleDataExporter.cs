using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexBattleDataExporter : MonoBehaviour
    {
        [SerializeField] protected TMPro.TMP_Text m_text;

        protected bool m_loaded;
        protected string m_pkmPath;
        protected PKHeX.Core.PKM m_pkm;

        [Header("Pokémon Data - Serialized for Debug" ),Space]
        [SerializeField] protected PLGU_PKHexPokemonData m_data;

        // Start is called before the first frame update
        void Start()
        {
            m_loaded = false;
            m_text.text = "No PKM is loaded";
        }

        public void ImportFile()
        {
            // Open file with filter
            var extensions = new[] 
            {
                new ExtensionFilter("Geneneration files", "pk1", "pk2", "pk3", "pk4", "pk5", "pk6", "pk7", "pk8" ),
                new ExtensionFilter("PKM", "pkm" ),               
                new ExtensionFilter("All Files", "*" ),
            };

            var paths = StandaloneFileBrowser.OpenFilePanel("Select PKM file", "", extensions, false);

            if (paths.Count > 0)
            {
                m_pkmPath = paths[0].Name;
                if (!string.IsNullOrEmpty(m_pkmPath))
                {
                    var kvp = PLGU_PKHexUtils.LoadPKMFromPath(m_pkmPath);
                    m_loaded = kvp.Key;
                    if (m_loaded)
                    {
                        m_pkm = kvp.Value;
                        m_text.text = "Loaded file " + System.IO.Path.GetFileName(m_pkmPath) + " with Pokémon " + m_pkm.Nickname;
                    }
                    else
                    {
                        m_pkm = null;
                        m_text.text = "Error. The PKM couldn't be loaded.";
                    }
                }
                else if (!m_loaded)
                {
                    m_text.text = "No PKM is loaded";
                }
            }
        }

        public void ImportFromPLGUFile()
        {
            // Open file with filter
            var extensions = new[]
            {
                new ExtensionFilter("PLGU Data", "PLGU_PKHexData" ),
                new ExtensionFilter("All Files", "*" ),
            };

            var paths = StandaloneFileBrowser.OpenFilePanel("Select PLGU_PKHexData file", "", extensions, false);

            if (paths.Count > 0)
            {
                m_pkmPath = paths[0].Name;
                if (!string.IsNullOrEmpty(m_pkmPath))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(m_pkmPath);
                    
                    string json = System.Text.UTF8Encoding.UTF8.GetString(bytes);
                    m_data = JsonUtility.FromJson<PLGU_PKHexPokemonData>(json);
          
                    m_loaded = m_data != null;
                    if (m_loaded)
                    {
                        m_pkm = null;
                        m_text.text = "Loaded file " + System.IO.Path.GetFileName(m_pkmPath) + " with Pokémon " + m_data.m_nickname;
                    }
                    else
                    {
                        m_pkm = null;
                        m_text.text = "Error. The PKM couldn't be loaded.";
                    }
                }
                else if (!m_loaded)
                {
                    m_text.text = "No PKM is loaded";
                }
            }
        }


        public void GeneratePLGUData()
        {
            if (m_loaded)
            {
                var item = StandaloneFileBrowser.SaveFilePanel("Select where to save the PLGU Pokémon Data", "", $"{m_pkm.Nickname}", "PLGU_PKHexData");
                if (item != null)
                {
                    string path = item.Name;
                    if (string.IsNullOrEmpty(path))
                    {
                        try
                        {
                            m_data = PLGU_PKHexUtils.ConvertPKMToBattleData(m_pkm);
                            string json = JsonUtility.ToJson(m_data);
                            byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(json);
                            System.IO.File.WriteAllBytes(path, bytes);
                        }
                        catch (System.Exception e)
                        {
                            m_text.text = "No PKM is loaded because: " + e.Message + ": " + e.StackTrace;
                        }
                    }
                }    
            }
            else
            {
                m_text.text = "No PKM is loaded. Load one to export it for PLGU.";
            }
        }
    }

}