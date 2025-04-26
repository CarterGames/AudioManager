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
    /// Implement to add a edit for the an audio source that can be used on any audio clip play call.
    /// </summary>
    public interface IEditModule
    {
        /// <summary>
        /// Gets if the edits should process when looping
        /// </summary>
        bool ProcessOnLoop { get; }
        
        
        /// <summary>
        /// Processes the edit when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        void Process(AudioSourceInstance source);
        
        
        /// <summary>
        /// Revers the edit to default when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        void Revert(AudioSourceInstance source);
    }
}