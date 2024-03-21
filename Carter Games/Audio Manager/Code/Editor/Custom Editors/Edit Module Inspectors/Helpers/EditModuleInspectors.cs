/*
 * Copyright (c) 2024 Carter Games
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class to determine the editor scripts for edit modules.
    /// </summary>
    public static class EditModuleInspectors
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// A lookup of all the editors for the edit modules that have custom editors for.
        /// </summary>
        public static readonly Dictionary<string, EditModuleInspectorBase> Inspectors =
            new Dictionary<string, EditModuleInspectorBase>()
            {
                { "CarterGames.Assets.AudioManager.VolumeEdit", new VolumeEditModuleInspector() },
                { "CarterGames.Assets.AudioManager.PitchEdit", new PitchEditModuleInspector() },
                { "CarterGames.Assets.AudioManager.MixerEdit", new MixerGroupEditModuleInspector() },
                { "CarterGames.Assets.AudioManager.DelayEdit", new DelayEditModuleInspector() },
                { "CarterGames.Assets.AudioManager.GlobalVarianceEdit", new GlobalVarianceEditModuleInspector() },
                { "CarterGames.Assets.AudioManager.DynamicStartTimeEdit", new DynamicStartTimeEditModuleInspector() },
                { "CarterGames.Assets.AudioManager.LoopEdit", new LoopEditModuleInspector() },
            };
    }
}