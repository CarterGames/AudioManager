/*
 * Copyright (c) 2018-Present Carter Games
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

using UnityEngine;

namespace CarterGames.Assets.AudioManager.Demo
{
    /// <summary>
    /// A demo script for playing a clip or group from the audio manager with edit modules applied.
    /// </summary>
    public sealed class CodeAudioClipPlayWithEditsExample : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private string requestName;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public void PlayClipWithEdits()
        {
            // Clip. struct can be used instead, Example:
            // AudioManager.Play(Clip.DEMO_Click_01, new DelayEdit(.5f), new VolumeEdit(.75f));
            AudioManager.Play(requestName, new DelayEdit(.5f), new VolumeEdit(.75f));
        }


        public void PlayGroupWithEdits()
        {
            // Group. struct can be used instead, Example:
            // AudioManager.PlayGroup(Group.DEMO_ClickGroup, new DelayEdit(.5f), new VolumeEdit(.75f));
            AudioManager.PlayGroup(requestName, new DelayEdit(.5f), new VolumeEdit(.75f));
        }
    }
}