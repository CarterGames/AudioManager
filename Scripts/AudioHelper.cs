using System.Collections;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Helper
 *      Holds a static method for the AudioArgs, used in many Audio Manager methods.
 *			
 *  Written by:
 *      Jonathan Carter
 *
 *  Published By:
 *      Carter Games
 *      E: hello@carter.games
 *      W: https://www.carter.games
 *		
 *  Version: 2.5.6
*	Last Updated: 09/02/2022 (d/m/y)								
 * 
 */

namespace CarterGames.Assets.AudioManager
{
    public static class AudioHelper
    {
        /// <summary>
        /// Passes through the arguments for the audio source...
        /// Static | Can be accessed anywhere for use...
        /// </summary>
        /// <param name="values">Parings to pass through...</param>
        /// <returns>Hashtable | Setup with the values you have entered...</returns>
        public static Hashtable AudioArgs(params object[] values)
        {
            var _table = new Hashtable(values.Length / 2);

            if (!(values.Length % 2).Equals(0)) return null;

            var i = 0;

            while (i < values.Length - 1)
            {
                _table.Add(values[i], values[i + 1]);
                i += 2;
            }

            return _table;
        }
    }
}