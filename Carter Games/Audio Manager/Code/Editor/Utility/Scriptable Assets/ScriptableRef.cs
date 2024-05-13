using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles references to scriptable objects in the asset that need generating without user input etc.
    /// </summary>
    public static class ScriptableRef
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Asset Paths
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        private static readonly string AssetIndexPath = $"{AssetBasePath}/Carter Games/{AssetName}/Resources/Asset Index.asset";
        private static readonly string LibraryAssetPath = $"{AssetBasePath}/Carter Games/{AssetName}/Data/Library.asset";
        private static readonly string DataFolderPath = $"{AssetBasePath}/Carter Games/{AssetName}/Data/";
        private static readonly string SettingsAssetPath = $"{AssetBasePath}/Carter Games/{AssetName}/Data/Runtime Settings.asset";
        

        // Asset Filters
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        private static readonly string RuntimeSettingsFilter = $"t:{typeof(SettingsAssetRuntime).FullName}";
        private static readonly string AudioManagerLibraryFilter = $"t:{typeof(AudioLibrary).FullName}";
        private static readonly string AssetIndexFilter = $"t:{typeof(AssetIndex).FullName}";


        // Asset Caches
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        public static SettingsAssetRuntime settingsAssetRuntimeCache;
        public static AudioLibrary audioLibraryCache;
        public static AssetIndex assetIndexCache;
        
        
        // SerializedObject Caches
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        private static SerializedObject settingsAssetRuntimeObjectCache;
        private static SerializedObject audioLibraryObjectCache;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Helper Properties
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the path where the asset code is located.
        /// </summary>
        private static string AssetBasePath => FileEditorUtil.AssetBasePath;
        
        
        /// <summary>
        /// Gets the name of the asset stored in the file editor util class.
        /// </summary>
        private static string AssetName => FileEditorUtil.AssetName;


        /// <summary>
        /// Gets if the library file exists in the project or not.
        /// </summary>
        // public static bool HasLibraryFile => File.Exists(LibraryAssetPath);
        public static bool HasLibraryFile
        {
            get
            {
                var assets = AssetDatabase.FindAssets(AudioManagerLibraryFilter);
                return assets != null && assets.Length > 0;
            }
        }


        // Asset Properties
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The asset index for the asset.
        /// </summary>
        public static AssetIndex AssetIndex =>
            FileEditorUtil.CreateSoGetOrAssignAssetCache(ref assetIndexCache, AssetIndexFilter, AssetIndexPath, AssetName, $"{AssetName}/Asset Index.asset");
        
        
        /// <summary>
        /// The runtime settings for the asset.
        /// </summary>
        public static SettingsAssetRuntime RuntimeSettings =>
            FileEditorUtil.CreateSoGetOrAssignAssetCache(ref settingsAssetRuntimeCache, RuntimeSettingsFilter, SettingsAssetPath, AssetName, $"{AssetName}/Data/Runtime Settings.asset");
        
        
        /// <summary>
        /// The audio library for the asset.
        /// </summary>
        public static AudioLibrary Library =>
            FileEditorUtil.CreateSoGetOrAssignAssetCache(ref audioLibraryCache, AudioManagerLibraryFilter, LibraryAssetPath, AssetName, $"{AssetName}/Data/Library.asset");
        
        
        // Object Properties
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The runtime SerializedObject for the asset.
        /// </summary>
        public static SerializedObject RuntimeSettingsObject =>
            FileEditorUtil.CreateGetOrAssignSerializedObjectCache(ref settingsAssetRuntimeObjectCache, RuntimeSettings);
        
        
        /// <summary>
        /// The audio library SerializedObject for the asset.
        /// </summary>
        public static SerializedObject LibraryObject =>
            FileEditorUtil.CreateGetOrAssignSerializedObjectCache(ref audioLibraryObjectCache, Library);
       
        
        // Assets initialized Check
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public static bool HasAllAssets =>
            File.Exists(AssetIndexPath) && File.Exists(SettingsAssetPath) &&
            File.Exists(LibraryAssetPath);
        
        
        /// <summary>
        /// Tries to create any missing assets when called.
        /// </summary>
        public static void TryCreateAssets()
        {
            AssetDatabase.StartAssetEditing();
            
            if (assetIndexCache == null)
            {
                FileEditorUtil.CreateSoGetOrAssignAssetCache(
                    ref assetIndexCache, 
                    AssetIndexFilter, 
                    AssetIndexPath,
                    AssetName, $"{AssetName}/Resources/Asset Index.asset");
            }

            
            if (settingsAssetRuntimeCache == null)
            {
                FileEditorUtil.CreateSoGetOrAssignAssetCache(
                    ref settingsAssetRuntimeCache, 
                    RuntimeSettingsFilter, 
                    SettingsAssetPath, 
                    AssetName, $"{AssetName}/Data/Runtime Settings.asset");
            }
            
            
            if (audioLibraryCache == null)
            {
                FileEditorUtil.CreateSoGetOrAssignAssetCache(
                    ref audioLibraryCache, 
                    AudioManagerLibraryFilter, 
                    LibraryAssetPath, 
                    AssetName, $"{AssetName}/Data/Library.asset");
            }
            
            AssetDatabase.StopAssetEditing();
        }
    }
}