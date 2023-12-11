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
        public override string ClassPath => UtilEditor.GetPathOfFile("track", "Utility/Generated Classes/Track.cs");
        public override string ClassName => "Track";
        
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
                    AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
                }
                else
                {
                    if (data.Count <= 0)
                    {
                        AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
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
                                AmLog.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorElementFailed));
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