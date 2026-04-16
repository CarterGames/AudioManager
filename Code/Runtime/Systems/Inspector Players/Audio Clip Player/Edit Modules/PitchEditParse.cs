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

using CarterGames.Shared.AudioManager.Serializiation;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the parsing of the pitch edit from the inspector audio clip player.
    /// </summary>
    public class PitchEditParse : IInspectorPlayerParse
    {
        /// <summary>
        /// Parses the module from the inspector setup when called.
        /// </summary>
        /// <param name="data">The data to parse with.</param>
        /// <returns>The setup module</returns>
        public IEditModule Parse(SerializableDictionary<string, string> data)
        {
            PitchEdit pitchModule = null;

            switch (int.Parse(data["moduleMode"]))
            {
                case 0:

                    pitchModule = new PitchEdit(float.Parse(data["normalEditValue"]));

                    break;
                case 1:

                    var min = float.Parse(data["rangeEditValue"].Split(',')[0].Replace("[", string.Empty));
                    var max = float.Parse(data["rangeEditValue"].Split(',')[1].Replace("]", string.Empty));

                    pitchModule = new PitchEdit(min, max);

                    break;
                case 2:

                    var starting = float.Parse(data["varianceEditValue"].Split(',')[0].Replace("[", string.Empty));
                    var offset = float.Parse(data["varianceEditValue"].Split(',')[1]);
                    var minVariance = float.Parse(data["varianceEditValue"].Split(',')[2]);
                    var maxVariance = float.Parse(data["varianceEditValue"].Split(',')[3].Replace("]", string.Empty));

                    pitchModule = new PitchEdit(new Variance(starting, offset, minVariance, maxVariance));

                    break;
            }

            return pitchModule;
        }
    }
}