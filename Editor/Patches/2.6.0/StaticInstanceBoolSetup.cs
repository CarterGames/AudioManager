using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A patch for 2.6.0 which updates the scripting defines for the static instance when changing build platform.
    /// </summary>
    public class StaticInstanceBoolSetup : AssetPostprocessor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static bool _hasRun;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   AssetPostprocessor Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (_hasRun) return;

            AudioManagerEditorUtil.Settings.isUsingStatic = ScriptingDefineHandler.IsScriptingDefinePresent();
            _hasRun = true;
        }
    }
}