using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Provides the menu items for the asset.
    /// </summary>
    public static class AudioManagerMenuItems
    {
        /// <summary>
        /// Opens the settings of the asset when called via the menu item.
        /// </summary>
        [MenuItem("Tools/Carter Games/Audio Manager/Edit Settings", priority = 0)]
        private static void OpenSettings()
        {
            SettingsService.OpenProjectSettings("Project/Carter Games/Audio Manager");
        }
    }
}