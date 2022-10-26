using System.Collections.Generic;
using System.IO;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper for managing directories for the asset inspectors.
    /// </summary>
    public static class DirectorySelectHelper
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static List<string> _allDirectories = new List<string>();
        private static List<string> _directoriesCache = new List<string>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Converts an int into a directory.
        /// </summary>
        /// <param name="value">The index to convert.</param>
        /// <returns>The directory matching the index.</returns>
        public static string ConvertIntToDir(int value) => GetAllDirectories()[value];

        
        /// <summary>
        /// Converts an int into a directory.
        /// </summary>
        /// <param name="value">The index to convert.</param>
        /// <param name="options">The directories to search through.</param>
        /// <returns>The directory matching the index.</returns>
        public static string ConvertIntToDir(int value, List<string> options) => options[value];
        
        
        /// <summary>
        /// Converts a string to an index.
        /// </summary>
        /// <param name="value">The index to convert.</param>
        /// <returns>The index matching the directory.</returns>
        public static int ConvertStringToIndex(string value) => GetAllDirectories().IndexOf(value);

        
        /// <summary>
        /// Converts a string to an index.
        /// </summary>
        /// <param name="value">The index to convert.</param>
        /// <param name="options">The directories to search through.</param>
        /// <returns>The index matching the directory.</returns>
        public static int ConvertStringToIndex(string value, List<string> options) => options.IndexOf(value);
        
        
        /// <summary>
        /// Calls to refresh the directories currently saved.
        /// </summary>
        public static void RefreshAllDirectories()
        {
            _allDirectories = new List<string>();
            _allDirectories.Add("");
            _allDirectories.Add("Assets");
            _allDirectories.AddRange(Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories));

            for (var i = 0; i < _allDirectories.Count; i++)
                _allDirectories[i] = _allDirectories[i].Replace(@"\", "/");
        }
        
        
        /// <summary>
        /// Gets all the directories currently in the project.
        /// </summary>
        /// <returns>A collection of all the current directories in the project.</returns>
        public static List<string> GetAllDirectories()
        {
            if (_allDirectories.Count > 0) return _allDirectories;

            _allDirectories = new List<string>();
            _allDirectories.Add("");
            _allDirectories.Add("Assets");
            _allDirectories.AddRange(Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories));

            for (var i = 0; i < _allDirectories.Count; i++)
                _allDirectories[i] = _allDirectories[i].Replace(@"\", "/");

            return _allDirectories;
        }
        
        
        /// <summary>
        /// Gets all the directories currently in the project.
        /// </summary>
        /// <param name="shouldUpdate">Should the cache of directories update?</param>
        /// <returns>A collection of all the current directories in the project.</returns>
        public static List<string> GetDirectoriesFromBase(bool shouldUpdate = false)
        {
            if (!shouldUpdate)
            {
                if (_directoriesCache.Count > 0)
                    return _directoriesCache;
            }
            
            _directoriesCache = new List<string>();
            _directoriesCache.Add("");
            _directoriesCache.Add(AudioManagerEditorUtil.Settings.baseAudioScanPath);
            _directoriesCache.AddRange(Directory.GetDirectories(AudioManagerEditorUtil.Settings.baseAudioScanPath, "*", SearchOption.AllDirectories));

            for (var i = 0; i < _directoriesCache.Count; i++)
                _directoriesCache[i] = _directoriesCache[i].Replace(@"\", "/");

            return _directoriesCache;
        }
    }
}