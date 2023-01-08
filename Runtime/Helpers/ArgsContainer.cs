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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A class to hold the audio args setup.
    /// </summary>
    public static class ArgsContainer
    {
        /// <summary>
        /// Passes through the arguments for the audio source.
        /// Is Static & can be accessed anywhere for use.
        /// </summary>
        /// <param name="values">Parings to pass through.</param>
        /// <returns>Setup with the values you have entered.</returns>
        public static Hashtable AudioArgs(params object[] values)
        {
            var table = new Hashtable(values.Length / 2);

            if (!(values.Length % 2).Equals(0)) return null;

            var i = 0;

            while (i < values.Length - 1)
            {
                table.Add(values[i], values[i + 1]);
                i += 2;
            }

            return table;
        }
    }
}