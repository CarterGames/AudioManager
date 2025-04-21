/*
 * Copyright (c) 2025 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles all the error codes the asset can throw in logs. 
    /// </summary>
    public enum AudioManagerErrorCode
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Default Case
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        None = 0,                                       // (None) No error...
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Runtime Specific Errors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        AudioDisabled = 1,                              // (Runtime) Audio is disabled via play-state setting.
        
        ClipCannotBeFound = 2,                          // (Runtime) A clip cannot be found.
        GroupCannotBeFound = 3,                         // (Runtime) A group be found.
        MixerCannotBeFound = 4,                         // (Runtime) A mixer cannot be found.
        
        TagCannotBeFound = 20,
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Editor Specific Errors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        PrefabNotValid = 100,                           // (Editor) Prefab doesn't have required classes attached.
        EditorOnlyMethod = 101,                         // (Editor) User tried to call an editor only method at runtime.
        InvalidAudioClipInspectorInput = 102,           // (Editor) When the user tries to enter an invalid value in a audio clip player inspector.
        InvalidMusicInspectorInput = 103,               // (Editor) When the user tries to enter an invalid value in a music track player inspector.
        
        StructGeneratorElementFailed = 110,             // (Editor) Struct generator had an element fail.
        StructGeneratorNoData = 111,                    // (Editor) Struct generator had no data to generate with.
        StructElementNameAlreadyExists = 112,           // (Editor) Struct generator stopped a duplicate entry from being added.
    }
}