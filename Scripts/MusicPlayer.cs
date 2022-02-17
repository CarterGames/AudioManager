using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Linq;

/*
 * 
 *  Audio Manager
 *							  
 *	Music Player
 *      plays background music and allows transitions between tracks.
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
    /// MonoBehaviour Class | Static | The Music player, designed to play background music in your game.
    /// </summary>
    public class MusicPlayer : MonoBehaviour
    {
        private const string NotAValidTransitionString =
            "<color=#E77A7A><b>Audio Manager</b></color> | Music PLayer | <color=#D6BA64>Warning Code 5</color> Transition chosen was not valid, please select a valid transition to use.";
        
        // Start & End times for the track in use...
        [SerializeField] private float timeToStartFrom;
        [SerializeField] private float timeToLoopAt;

        // The track to play...
        [SerializeField] private AudioClip musicTrack;
        
        // Audio Source Edits...
        [SerializeField] private AudioMixerGroup mixer = default;
        [SerializeField] private bool shouldLoop = true;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private float volume = 1f;
        [SerializeField] private float pitch = 1f;
        [SerializeField] private float transitionLength = 1f;

        [SerializeField] private TransitionType introTransition;

        // Used in editor to show / hide audio source...
        [SerializeField] private bool showSource;

        private float transitionTime = 1f;
        private AudioSource[] sources;
        

        /// <summary>
        /// Gets invoked whenever a track has started.
        /// </summary>
        public static event Action OnTrackStarted;
        
        /// <summary>
        /// Gets invoked whenever a track has ended that wasn't set to loop.
        /// </summary>
        public static event Action OnTrackEnded;
        
        /// <summary>
        /// Gets invoked whenever a track has looped around to play again.
        /// </summary>
        public static event Action OnTrackLooped;
        
        /// <summary>
        /// Gets invoked whenever a track has been changed by the user.
        /// </summary>
        public static event Action OnTrackChanged;
        
        /// <summary>
        /// Gets invoked whenever a track transition has ended.
        /// </summary>
        public static event Action OnTrackTransitionComplete;
        
        /// <summary>
        /// Static instance of the music player
        /// </summary>
        public static MusicPlayer instance;

        /// <summary>
        /// Gets the track currently being played...
        /// </summary>
        public AudioClip GetActiveTrack => musicTrack;
        
        /// <summary>
        /// Gets the position where the track is at...
        /// </summary>
        public float GetTrackPosition => GetAudioSource.time;
        
        /// <summary>
        /// Gets the audio source for the music player...
        /// </summary>
        public AudioSource GetAudioSource { get; private set; }


        /// <summary>
        /// Edit whether or not the track should loop.
        /// </summary>
        public bool ShouldLoop
        {
            get => shouldLoop;
            set
            {
                shouldLoop = value;
                GetAudioSource.loop = shouldLoop;
            }
        }

        /// <summary>
        /// Returns if the music player is currently playing a track.
        /// </summary>
        public bool IsTrackPlaying => GetAudioSource.isPlaying;


        private void OnEnable()
        {
            // Instancing Setup...
            DontDestroyOnLoad(this);

            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        
        private void OnDisable() => StopAllCoroutines();
        private void OnDestroy() => StopAllCoroutines();
        private void Awake() => MusicPlayerSetup();
        
        
        private void Update()
        {
            if (!shouldLoop)
                TrackEndedCheck();
            else
                TrackLoopCheck();
        }


        /// <summary>
        /// Checks to see if the music track that is playing has ended
        /// </summary>
        /// <remarks>This will only run if the track is not set to loop</remarks>
        private void TrackEndedCheck()
        {
            if (GetAudioSource.isPlaying) return;
            if (!Mathf.Approximately(GetAudioSource.time, GetAudioSource.clip.length)) return;
            
            OnTrackEnded?.Invoke();
        }

        
        /// <summary>
        /// Checks to see if the music track that is playing has reached its defined loop point
        /// </summary>
        /// <remarks>This will only run if the track is set to loop</remarks>
        private void TrackLoopCheck()
        {
            if (timeToLoopAt.Equals(0)) return;
            if (!GetAudioSource.isPlaying) return;
            if (!(GetAudioSource.time > timeToLoopAt)) return;
            
            GetAudioSource.time = timeToStartFrom;
            OnTrackLooped?.Invoke();
        }
        
        
        /// <summary>
        /// Runs the setup for the values to play the music track...
        /// </summary>
        private void MusicPlayerSetup()
        {
            // Reference setup...
            sources = GetComponents<AudioSource>();
            GetAudioSource = sources[0];
            
            GetAudioSource.playOnAwake = playOnAwake;
            GetAudioSource.loop = shouldLoop;
            GetAudioSource.clip = musicTrack;
            GetAudioSource.volume = volume;
            GetAudioSource.pitch = pitch;
            GetAudioSource.outputAudioMixerGroup = mixer;

            var _inactive = GetInactiveSource();
            _inactive.loop = shouldLoop;
            _inactive.pitch = pitch;
            _inactive.outputAudioMixerGroup = mixer;

            if (!playOnAwake) return;
            
            PlayTrack(GetActiveTrack, timeToStartFrom, timeToLoopAt, introTransition, transitionLength);
            OnTrackStarted?.Invoke();
        }


        /// <summary>
        /// Sets the volume to the inputted value instantly.
        /// </summary>
        /// <param name="value">Float | The value to set the volume to.</param>
        public void SetVolume(float value)
        {
            AssignSource();
            volume = Mathf.Clamp01(value);
            GetAudioSource.volume = Mathf.Clamp01(value);
        }
        
        
        /// <summary>
        /// Plays the track current set in the inspector.
        /// </summary>
        public void PlayTrack()
        {
            GetAudioSource.Play();
        }


        /// <summary>
        /// Stops the active track when called...
        /// </summary>
        public void StopTrack()
        {
            GetAudioSource.Stop();
        }
        

        /// <summary>
        /// Changes the track to the inputted audio clip...
        /// </summary>
        /// <param name="track">The track to change to</param>
        /// <param name="transitionType">The transition type to perform</param>
        public void PlayTrack(AudioClip track, TransitionType transitionType = TransitionType.None)
        {
            ChangeTrackLogic(track, 0, track.length, transitionType);
        }
        
        
        /// <summary>
        /// Changes the track to the inputted audio clip...
        /// </summary>
        /// <param name="track">The track to change to</param>
        /// <param name="transitionType">The transition type to perform</param>
        /// <param name="time">The time this transition should take to complete</param>
        public void PlayTrack(AudioClip track, TransitionType transitionType, float time)
        {
            transitionTime = SetTransitionLength(time);
            ChangeTrackLogic(track, 0, track.length, transitionType);
        }
        
        
        /// <summary>
        /// Changes the track to the inputted audio clip... with a set start time...
        /// </summary>
        /// <param name="track">AudioClip | The track to change to...</param>
        /// <param name="startTime">Float | The time to start playing the clip at...</param>
        /// <param name="transitionType">The transition type to perform</param>
        public void PlayTrack(AudioClip track, float startTime, TransitionType transitionType = TransitionType.None)
        {
            ChangeTrackLogic(track, startTime, track.length, transitionType);
        }
        
        
        /// <summary>
        /// Changes the track to the inputted audio clip... with a set start time...
        /// </summary>
        /// <param name="track">AudioClip | The track to change to...</param>
        /// <param name="startTime">Float | The time to start playing the clip at...</param>
        /// <param name="transitionType">The transition type to perform</param>
        /// <param name="time">The time this transition should take to complete</param>
        public void PlayTrack(AudioClip track, float startTime, TransitionType transitionType, float time)
        {
            transitionTime = SetTransitionLength(time);
            ChangeTrackLogic(track, startTime, track.length, transitionType);
        }


        /// <summary>
        /// Changes the track to the inputted audio clip... with a set start time...
        /// </summary>
        /// <param name="track">AudioClip | The track to change to...</param>
        /// <param name="startTime">Float | The time to start playing the clip at...</param>
        /// <param name="endTime">Float | The time the track should loop at...</param>
        /// <param name="transitionType">The transition type to perform</param>
        public void PlayTrack(AudioClip track, float startTime, float endTime, TransitionType transitionType = TransitionType.None)
        {
            ChangeTrackLogic(track, startTime, endTime, transitionType);
        }
        
        
        /// <summary>
        /// Changes the track to the inputted audio clip... with a set start time...
        /// </summary>
        /// <param name="track">AudioClip | The track to change to...</param>
        /// <param name="startTime">Float | The time to start playing the clip at...</param>
        /// <param name="endTime">Float | The time the track should loop at...</param>
        /// <param name="transitionType">The transition type to perform</param>
        /// <param name="time">The time this transition should take to complete</param>
        public void PlayTrack(AudioClip track, float startTime, float endTime, TransitionType transitionType, float time)
        {
            transitionTime = SetTransitionLength(time);
            ChangeTrackLogic(track, startTime, endTime, transitionType);
        }

        
        /// <summary>
        /// Changes the track and performs the 
        /// </summary>
        private void ChangeTrackLogic(AudioClip track, float startTime, float endTime,
            TransitionType transitionType = TransitionType.None)
        {
            AssignSource();
            
            switch (transitionType)
            {
                case TransitionType.None:
                    SetTrack(track, GetAudioSource, startTime, endTime);
                    break;
                case TransitionType.FadeIn:
                    SetTrack(track, GetAudioSource, startTime, endTime);
                    FadeInTrack();
                    break;
                case TransitionType.FadeOut:
                    FadeOutTrack();
                    SetTrack(track, GetAudioSource, startTime, endTime);
                    break;
                case TransitionType.Fade:
                    FadeTrack(track, startTime, endTime);
                    break;
                case TransitionType.CrossFade:
                    CrossFadeTrack(track, startTime, endTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transitionType), transitionType, NotAValidTransitionString);
            }
            
            OnTrackChanged?.Invoke();
        }

        
        /// <summary>
        /// Sets the desired time to speed up or slow the transition down as desired...
        /// </summary>
        /// <param name="timeDesired">The time the user wants the transition to take...</param>
        /// <returns>Float</returns>
        private static float SetTransitionLength(float timeDesired)
        {
            if (timeDesired < 0f)
                return 0f; 
            
            if (timeDesired <= 1f)
                return timeDesired;
            
            return 1f / timeDesired;
        }
        
        
        
        /// <summary>
        /// Fades in the current track when called...
        /// </summary>
        private void FadeInTrack()
        {
            StartCoroutine(FadeInOut(true, GetAudioSource, transitionTime));
        }
        
        
        /// <summary>
        /// Fades in the current track when called...
        /// </summary>
        /// <param name="s">The source to play the track on</param>
        private void FadeInTrack(AudioSource s)
        {
            StartCoroutine(FadeInOut(true, s, transitionTime));
        }
        
        
        /// <summary>
        /// Fades out the current track when called...
        /// </summary>
        private void FadeOutTrack()
        {
            StartCoroutine(FadeInOut(false, GetAudioSource, transitionTime));
        }
        
        
        /// <summary>
        /// Fades out the current track when called...
        /// </summary>
        /// <param name="s">The source to play the track on</param>
        private void FadeOutTrack(AudioSource s)
        {
            StartCoroutine(FadeInOut(false, s, transitionTime));
        }
        
        
        /// <summary>
        /// Fades in the current track when called...
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <param name="start">the start time</param>
        /// <param name="end">the end time</param>
        private void FadeTrack(AudioClip clip, float start, float end)
        {
            StartCoroutine(FadeTrackInOut(clip, GetAudioSource, transitionTime, start, end));
        }


        /// <summary>
        /// Cross-fades the track in and out...
        /// </summary>
        private void CrossFadeTrack(AudioClip clip, float start, float end)
        {
            SetTrack(clip, GetInactiveSource(), start, end);
            FadeOutTrack(GetAudioSource);
            FadeInTrack(GetInactiveSource());
            OnTrackTransitionComplete += ChangeSource;
        }


        /// <summary>
        /// Sets the track to the track entered...
        /// </summary>
        /// <param name="track"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        private void SetTrack(AudioClip track, AudioSource s, float startTime, float endTime)
        {
            s.clip = track;
            musicTrack = track;
            timeToStartFrom = startTime;
            timeToLoopAt = endTime.Equals(0) ? track.length : endTime;
            s.time = startTime;
            s.Play();
            OnTrackStarted?.Invoke();
        }
        

        /// <summary>
        /// Assigns the audio source if it is not already, just to catch issues where the source is not assigned before use...
        /// </summary>
        private void AssignSource()
        {
            if (GetAudioSource) return;
            GetAudioSource = GetComponent<AudioSource>();
            GetAudioSource.loop = shouldLoop;
        }
        
        
        /// <summary>
        /// Coroutine | Fades the music volume in or out.
        /// </summary>
        /// <param name="fadeIn">Should the co fade in?</param>
        /// <param name="s">The source to play from</param>
        /// <param name="multiplier">How fast or slow should it go.</param>
        private IEnumerator FadeInOut(bool fadeIn, AudioSource s, float multiplier = 1f)
        {
            var _currentTime = 0f;
            var _start = s.volume;
            var _targetValue = 0f;
            var _duration = 1f;

            // Fade to 1...
            if (fadeIn)
            {
                _targetValue = volume;

                while (_currentTime < _duration)
                {
                    _currentTime += multiplier * Time.unscaledDeltaTime;
                    s.volume = Mathf.Lerp(0, _targetValue, _currentTime / _duration);
                    yield return null;
                }

                s.volume = volume;
                OnTrackTransitionComplete?.Invoke();
                yield break;
            }

            // Fade to 0...
            // Only runs if it is not fading to 1...
            _targetValue = 0f;

            while (_currentTime < _duration)
            {
                _currentTime += multiplier * Time.unscaledDeltaTime;
                s.volume = Mathf.Lerp(_start, _targetValue, _currentTime / _duration);
                yield return null;
            }

            s.volume = 0f;
            OnTrackTransitionComplete?.Invoke();
        }
        


        /// <summary>
        /// Coroutine | Fades out & then in the track into the game.
        /// </summary>
        /// <param name="clip">The clip to change</param>
        /// <param name="s">The source to play from</param>
        /// <param name="multiplier">The speed in seconds the transition should take</param>
        /// <param name="startTime">The start time of the clip</param>
        /// <param name="endTime">The end time of the clip</param>
        /// <returns></returns>
        private IEnumerator FadeTrackInOut(AudioClip clip, AudioSource s, float multiplier, float startTime = 0f, float endTime = 0f)
        {
            var _currentTime = 0f;
            var _start = s.volume;
            var _targetValue = 0f;
            var _duration = 1f;
            
            _targetValue = 0f;

            while (_currentTime < _duration)
            {
                _currentTime += multiplier * 2 * Time.unscaledDeltaTime;
                s.volume = Mathf.Lerp(volume, _targetValue, _currentTime / _duration);
                yield return null;
            }
            
            s.volume = 0f;
            SetTrack(clip, s, startTime, endTime);
            
            _start = s.volume;
            _currentTime = 0f;
            
            while (_currentTime < _duration)
            {
                _currentTime += multiplier * 2 * Time.unscaledDeltaTime;
                s.volume = Mathf.Lerp(_start, volume, _currentTime / _duration);
                yield return null;
            }
            
            s.volume = volume;
            OnTrackTransitionComplete?.Invoke();
        }
        
        
        
        /// <summary>
        /// Changes the active audio source to the now active source
        /// </summary>
        /// <remarks>Used in the cross fade ability to </remarks>
        private void ChangeSource()
        {
            GetAudioSource = GetAudioSource.Equals(sources[0]) 
                ? sources[1] 
                : sources[0];
            
            OnTrackTransitionComplete -= ChangeSource;
        }


        /// <summary>
        /// Gets the source that currently is not playing anything...
        /// </summary>
        /// <returns>AudioSource</returns>
        private AudioSource GetInactiveSource()
        {
            return sources.FirstOrDefault(s => !GetAudioSource.Equals(s));
        }
    }
}