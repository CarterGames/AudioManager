using System;
using CarterGames.Assets.Shared.PerProject;

namespace CarterGames.Assets.Shared.Common.Editor
{
    public static class CommonPerUserSettings
    {
        private static readonly string AutoValidationAutoCheckId = $"{UniqueId}_CarterGames_AudioManager_Editor_AutoVersionCheck";
        
        

        /// <summary>
        /// The unique if for the assets settings to be per project...
        /// </summary>
        public static string UniqueId =>
            (string) PerUserSettingsEditor.GetOrCreateValue<string>(PerUserSettingsUuidHandler.Uuid,
                PerUserSettingType.PlayerPref, Guid.NewGuid().ToString());


        /// <summary>
        /// Has the scanner been initialized.
        /// </summary>
        public static bool VersionValidationAutoCheckOnLoad
        {
            get => (bool) PerUserSettingsEditor.GetOrCreateValue<bool>(AutoValidationAutoCheckId, PerUserSettingType.EditorPref);
            set => PerUserSettingsEditor.SetValue<bool>(AutoValidationAutoCheckId, PerUserSettingType.EditorPref, value);
        }
    }
}