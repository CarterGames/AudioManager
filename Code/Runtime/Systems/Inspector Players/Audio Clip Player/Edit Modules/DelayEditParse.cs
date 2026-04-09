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

using CarterGames.Shared.AudioManager.Serializiation;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the parsing of the delay edit from the inspector audio clip player.
    /// </summary>
    public sealed class DelayEditParse : IInspectorPlayerParse
    {
        /// <summary>
        /// Parses the module from the inspector setup when called.
        /// </summary>
        /// <param name="data">The data to parse with.</param>
        /// <returns>The setup module</returns>
        public IEditModule Parse(SerializableDictionary<string, string> data)
        {
            DelayEdit delayModule = null;

            delayModule = new DelayEdit(float.Parse(data["delay"]));

            return delayModule;
        }
    }
}