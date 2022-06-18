using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    public class AudioPool : MonoBehaviour
    {
        private static List<AudioSource> memberObjects;
        private static HashSet<AudioSource> unavailableObjects;
        private static GameObject prefab;
        private static Transform parent;

        private static AudioPool instance;
        public static bool ExistsInScene => instance != null;
        public static bool IsInitialised => memberObjects != null && unavailableObjects != null;
        public static bool AnyActiveWithClip(string clipName) => unavailableObjects.Any(t => t.clip.name.Equals(clipName));


        private void OnEnable()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);

            DontDestroyOnLoad(this);
        }
        
        
        public static void Initialise(GameObject prefabObj, int initialCount)
        {
            prefab = prefabObj;
            parent = instance.transform;

            memberObjects = new List<AudioSource>();
            unavailableObjects = new HashSet<AudioSource>();

            for (var i = 0; i < initialCount; i++)
            {
                memberObjects.Add(Instantiate(prefab, parent).GetComponentInChildren<AudioSource>(true));
                memberObjects[i].gameObject.SetActive(false);
            }
        }


        private static AudioSource Create()
        {
            var newMember = Instantiate(prefab, parent).GetComponentInChildren<AudioSource>(true);
            memberObjects.Add(newMember);
            return newMember;
        }
        

        public static AudioSource Assign()
        {
            foreach (var t in memberObjects)
            {
                if (unavailableObjects.Contains(t)) continue;
                unavailableObjects.Add(t);
                return t;
            }

            var newMember = Create();
            unavailableObjects.Add(newMember);
            return newMember;
        }


        public static void Return(AudioSource member)
        {
            unavailableObjects.Remove(member);
        }
    }
}