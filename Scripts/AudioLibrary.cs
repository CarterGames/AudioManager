/*
 * 
 *  Audio Manager
 *							  
 *	Audio Library
 *      Used to store the clips in the scriptable object Audio Manager Files.
 *			
 *  Written by:
 *      Jonathan Carter
 *
 *  Published By:
 *      Carter Games
 *      E: hello@carter.games
 *      W: https://www.carter.games
 *		
 *  Version: 2.5.8
 *	Last Updated: 18/06/2022 (d/m/y)							
 * 
 */

using System;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Used the store the key/pair values for each clip found in the audio manager scan.
    /// </summary>
    [Serializable]
    public class AudioLibrary
    {
        public string key;
        public AudioClip value;

        public AudioLibrary(string _key, AudioClip _value)
        {
            key = _key;
            value = _value;
        }
    }
}