using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the object pooling for the audio currently in use.
    /// </summary>
    public class AudioPool : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static List<AudioSource> _memberObjects;
        private static HashSet<AudioSource> _unavailableObjects;
        private static GameObject _prefab;
        private static Transform _parent;
        private static AudioPool _instance;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets whether or not the audio pool is in the scene already.
        /// </summary>
        public static bool ExistsInScene => _instance != null;
        
        
        /// <summary>
        /// Gets whether or not the pool system has initialised.
        /// </summary>
        public static bool IsInitialised => _memberObjects != null && _unavailableObjects != null;
        

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(this);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initialises the pool for use.
        /// </summary>
        /// <param name="prefabObj">The prefab for the pool.</param>
        /// <param name="initialCount">The starting count of the pool.</param>
        public static void Initialise(GameObject prefabObj, int initialCount)
        {
            _prefab = prefabObj;
            _parent = _instance.transform;

            _memberObjects = new List<AudioSource>();
            _unavailableObjects = new HashSet<AudioSource>();

            for (var i = 0; i < initialCount; i++)
            {
                _memberObjects.Add(Instantiate(_prefab, _parent).GetComponentInChildren<AudioSource>(true));
                _memberObjects[i].gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Gets if there is any clip active of the name entered.
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public static bool AnyActiveWithClip(string clipName)
        {
            return _unavailableObjects.Any(t => t.clip.name.Equals(clipName));
        }


        /// <summary>
        /// Creates an new element in the pool.
        /// </summary>
        /// <returns>The audio source of the new element.</returns>
        private static AudioSource Create()
        {
            var newMember = Instantiate(_prefab, _parent).GetComponentInChildren<AudioSource>(true);
            _memberObjects.Add(newMember);
            return newMember;
        }
        

        /// <summary>
        /// Assigns an element of the pool for use.
        /// </summary>
        /// <returns>The audio source of the assigned element.</returns>
        public static AudioSource Assign()
        {
            foreach (var t in _memberObjects)
            {
                if (_unavailableObjects.Contains(t)) continue;
                _unavailableObjects.Add(t);
                return t;
            }

            var newMember = Create();
            _unavailableObjects.Add(newMember);
            return newMember;
        }


        /// <summary>
        /// Returns an element that was in use to the pool.
        /// </summary>
        /// <param name="member">The source to return.</param>
        public static void Return(AudioSource member)
        {
            _unavailableObjects.Remove(member);
        }
    }
}