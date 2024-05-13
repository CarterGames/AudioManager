using System;
using System.IO;
using CarterGames.Assets.AudioManager.Logging;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles generating the Track.? struct class entries.
    /// </summary>
    public sealed class TrackClassGenerator : StructGeneratorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override string LinePrefix => "        public const string";
        public override string ClassPath => UtilEditor.GetPathOfFile("playlist", "Utility/Generated Classes/Playlist.cs");
        public override string ClassName => "Playlist";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override void Generate()
        {
            using (var file = new StreamWriter(ClassPath))
            {
                StructHandler.WriteHeader(file, ClassName);

                var data = AssetAccessor.GetAsset<AudioLibrary>().MusicTrackLookup;

                if (data == null)
                {
                    AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
                }
                else
                {
                    if (data.Count <= 0)
                    {
                        AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
                    }
                    else
                    {
                        foreach (var trackKeyPair in data)
                        {
                            try
                            {
                                StructHandler.WriteLine(file, LinePrefix, trackKeyPair.Value.ListKey, $"\"{trackKeyPair.Key}\"");
                            }
#pragma warning disable
                            catch (Exception e)
                            {
                                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorElementFailed));
                            }
#pragma warning restore
                        }
                    }
                }

                StructHandler.WriteFooter(file);
            }
        }
    }
}