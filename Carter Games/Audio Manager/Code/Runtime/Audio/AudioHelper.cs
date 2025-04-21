namespace CarterGames.Assets.AudioManager
{
    public static class AudioHelper
    {
        public static float GetTotalDuration(this AudioSourceInstance audioInstance, AudioData audioData)
        {
            var time = 0f;
            
            // Adjusts for dynamic start time (- Time).
            /* ────────────────────────────────────────────────────────────────────────────────────────────────────── */
            if (audioInstance.EditParams.TryGetValue<bool>("dynamicTime", out var useDynamicTime))
            {
                time += audioData.value.length - (useDynamicTime ? audioData.dynamicStartTime.time : 0);
            }
            else
            {
                time += audioData.value.length;
            }

            
            // Adjusts for looping x times (+ Time).
            /* ────────────────────────────────────────────────────────────────────────────────────────────────────── */
            if (audioInstance.EditParams.TryGetValue<LoopEdit>("loop", out var loop))
            {
                if (audioInstance.EditParams.TryGetValue<DelayEdit>("delay", out var delayModule))
                {
                    if (loop.ShouldLoopWithDelays)
                    {
                        time += delayModule.Delay;
                    }
                    else
                    {
                        time -= delayModule.Delay;
                    }
                }
            }
            // Adjusts for delay (+ Time) if not adjusted by loop edits.
            /* ────────────────────────────────────────────────────────────────────────────────────────────────────── */
            else if (audioInstance.EditParams.TryGetValue<DelayEdit>("delay", out var delayModule))
            {
                time += delayModule.Delay;
            }

            return time;
        }
    }
}