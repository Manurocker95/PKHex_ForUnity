using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PKHexForUnity
{
    /// <summary>
    /// Loads the exported PKHeX data from <c>Resources</c> (synchronously or with coroutines). No PKHeX.Core dependency.
    /// </summary>
    /// <remarks>
    /// Loaded assets are cached by path until <see cref="Unload"/> or <see cref="UnloadAll"/>.
    /// Async usage from a MonoBehaviour:
    /// <code>
    /// yield return StartCoroutine(PKHexDatabase.LoadGameAsync(9, "SV", OnGameLoaded));
    /// </code>
    /// </remarks>
    public static class PKHexDatabase
    {
        private static readonly Dictionary<string, Object> m_cache = new Dictionary<string, Object>();

        public static PKHexDataIndex LoadIndex()
        {
            return Load<PKHexDataIndex>(PKHexDataPaths.GetIndexResourcePath());
        }

        public static IEnumerator LoadIndexAsync(Action<PKHexDataIndex> _onLoaded)
        {
            return LoadAsync<PKHexDataIndex>(PKHexDataPaths.GetIndexResourcePath(), _onLoaded);
        }

        public static PKHexGameData LoadGame(int _generation, string _gameKey)
        {
            return Load<PKHexGameData>(PKHexDataPaths.GetGameResourcePath(_generation, _gameKey));
        }

        public static IEnumerator LoadGameAsync(int _generation, string _gameKey, Action<PKHexGameData> _onLoaded)
        {
            return LoadAsync<PKHexGameData>(PKHexDataPaths.GetGameResourcePath(_generation, _gameKey), _onLoaded);
        }

        /// <param name="_languageFolder">Folder name such as "English" (see <see cref="PKHexDataPaths.LANGUAGE_FOLDERS"/>).</param>
        public static PKHexStringTable LoadStrings(int _generation, string _languageFolder)
        {
            return Load<PKHexStringTable>(PKHexDataPaths.GetStringsResourcePath(_generation, _languageFolder));
        }

        public static IEnumerator LoadStringsAsync(int _generation, string _languageFolder, Action<PKHexStringTable> _onLoaded)
        {
            return LoadAsync<PKHexStringTable>(PKHexDataPaths.GetStringsResourcePath(_generation, _languageFolder), _onLoaded);
        }

        /// <param name="_fileName">Text file name without extension, e.g. "Species" or "Pokedex_SV".</param>
        public static TextAsset LoadText(int _generation, string _languageFolder, string _fileName)
        {
            return Load<TextAsset>(PKHexDataPaths.GetTextResourcePath(_generation, _languageFolder, _fileName));
        }

        public static IEnumerator LoadTextAsync(int _generation, string _languageFolder, string _fileName, Action<TextAsset> _onLoaded)
        {
            return LoadAsync<TextAsset>(PKHexDataPaths.GetTextResourcePath(_generation, _languageFolder, _fileName), _onLoaded);
        }

        /// <summary> Removes one asset from the cache and unloads it from memory. </summary>
        public static void Unload(Object _asset)
        {
            if (_asset == null)
            {
                return;
            }
            string key = null;
            foreach (KeyValuePair<string, Object> pair in m_cache)
            {
                if (pair.Value == _asset)
                {
                    key = pair.Key;
                    break;
                }
            }
            if (key != null)
            {
                m_cache.Remove(key);
            }
            Resources.UnloadAsset(_asset);
        }

        /// <summary> Clears the cache and unloads every cached asset. </summary>
        public static void UnloadAll()
        {
            foreach (KeyValuePair<string, Object> pair in m_cache)
            {
                if (pair.Value != null)
                {
                    Resources.UnloadAsset(pair.Value);
                }
            }
            m_cache.Clear();
        }

        private static T Load<T>(string _path) where T : Object
        {
            T cached = GetCached<T>(_path);
            if (cached != null)
            {
                return cached;
            }
            T asset = Resources.Load(_path, typeof(T)) as T;
            if (asset == null)
            {
                Debug.LogError("[PKHex] Data not found in Resources: " + _path + " (" + typeof(T).Name + ")");
                return null;
            }
            m_cache[_path] = asset;
            return asset;
        }

        private static IEnumerator LoadAsync<T>(string _path, Action<T> _onLoaded) where T : Object
        {
            T cached = GetCached<T>(_path);
            if (cached == null)
            {
                ResourceRequest request = Resources.LoadAsync(_path, typeof(T));
                yield return request;
                cached = request.asset as T;
                if (cached == null)
                {
                    Debug.LogError("[PKHex] Data not found in Resources: " + _path + " (" + typeof(T).Name + ")");
                }
                else
                {
                    m_cache[_path] = cached;
                }
            }
            if (_onLoaded != null)
            {
                _onLoaded(cached);
            }
        }

        private static T GetCached<T>(string _path) where T : Object
        {
            Object cached;
            if (m_cache.TryGetValue(_path, out cached))
            {
                if (cached != null)
                {
                    return cached as T;
                }
                m_cache.Remove(_path); // Unloaded or destroyed outside this class.
            }
            return null;
        }
    }
}
