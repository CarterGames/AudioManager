using System;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    [Serializable]
    public class LibraryMetaData
    {
        [SerializeField] private string category;
        [SerializeField] private string[] tags;


        public string Category => category;
        public string[] Tags => tags;
    }
}