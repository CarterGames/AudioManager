using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles coroutine running for the music management system.
    /// </summary>
    public sealed class MusicRoutineHandler : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly Dictionary<string, Coroutine> Routines = new Dictionary<string, Coroutine>();
        private static MusicRoutineHandler routineHandler;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The routine handler for the music tracks.
        /// </summary>
        private static MusicRoutineHandler RoutineHandler
        {
            get
            {
                if (routineHandler != null) return routineHandler;
                routineHandler = DoNotDestroyHandler.MusicRoutineHandler;
                return routineHandler;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs when scenes are loading to initialize the event subscription for this class.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            MusicManager.TransitionCompletedCtx.Add(OnMusicTransitionCompletedWithCtx);
        }
        
        
        /// <summary>
        /// Runs a coroutines when called.
        /// </summary>
        /// <param name="id">The id to assign to the routine.</param>
        /// <param name="coroutine">The routine to run.</param>
        public static void RunRoutine(string id, IEnumerator coroutine)
        {
            if (Routines.ContainsKey(id))
            {
                if (Routines[id] != null)
                {
                    RoutineHandler.StopCoroutine(Routines[id]);
                }
                
                Routines[id] = RoutineHandler.StartCoroutine(coroutine);
                return;
            }
            
            Routines.Add(id, RoutineHandler.StartCoroutine(coroutine));
        }


        /// <summary>
        /// Stops the routine of the entered id if its not null.
        /// </summary>
        /// <param name="id">The id to stop.</param>
        public static void StopRoutine(string id)
        {
            if (Routines.ContainsKey(id))
            {
                if (Routines[id] != null)
                {
                    RoutineHandler.StopCoroutine(Routines[id]);
                }
            }
            
            Routines.Remove(id);
        }

        
        /// <summary>
        /// Runs when any music transition 
        /// </summary>
        /// <param name="id">The id to use.</param>
        /// <param name="transition">The transition that was used.</param>
        private static void OnMusicTransitionCompletedWithCtx(string id, IMusicTransition transition)
        {
            if (!Routines.ContainsKey(id)) return;
            
            RoutineHandler.StopCoroutine(Routines[id]);
            Routines.Remove(id);
        }
    }
}