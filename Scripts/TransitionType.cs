/*
 * 
 *  Audio Manager
 *							  
 *	Transition Type
 *      the types of the transitions that can be used in the music player
 *			
 *  Written by:
 *      Jonathan Carter
 *      E: jonathan@carter.games
 *      W: https://jonathan.carter.games
 *		
 *  Version: 2.5.6
*	Last Updated: 09/02/2022 (d/m/y)										
 * 
 */

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Used in the Music Player to define which transition to use.
    /// </summary>
    public enum TransitionType
    {
        None,
        FadeIn,
        FadeOut,
        Fade,
        CrossFade,
    }
}