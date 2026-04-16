/*
 * Audio Manager (3.x)
 * Copyright (c) Carter Games
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the
 * GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version. 
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
 *
 * You should have received a copy of the GNU General Public License along with this program.
 * If not, see <https://www.gnu.org/licenses/>. 
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