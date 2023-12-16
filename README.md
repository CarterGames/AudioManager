![AM Banner 1200x630](https://github.com/CarterGames/AudioManager/assets/33253710/c12da612-d4a7-40ff-9fe2-0eae107a402b)

<b>Audio Manager</b> is a <b>FREE</b> audio/sound management library for Unity with options to play clips & background music for any game. 

## Badges
![CodeFactor](https://www.codefactor.io/repository/github/cartergames/audiomanager/badge?style=for-the-badge)
![GitHub all releases](https://img.shields.io/github/downloads/CarterGames/AudioManager/total?style=for-the-badge)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/CarterGames/AudioManager?style=for-the-badge)
![GitHub repo size](https://img.shields.io/github/repo-size/CarterGames/AudioManager?style=for-the-badge)
![Unity](https://img.shields.io/badge/Unity-2020.3.x_or_higher-critical?style=for-the-badge)

## Legacy Version
You can still acces the legacy 2.x version in the ```2.x``` branch of this repo. 

## Key Features
- Automatic scanning of audio clips in the project.
- Dynamic start time for each clip to start where it starts playing audable audio, cutting out deadspace.
- Flexiable API for playing audio clips or groups of clips.
- Editor to manage the library, assign groups of clips together and music track lists.
- No setup needed, just import and go.
- Entirely static API, no scene references needed.
- Music playing setup.
- Inspector players for quick prototyping without needing to write any code.
- Regularly updated and maintained with ❤️ 

## How To Install
Either download and import the package from the releases section or the <a href="https://assetstore.unity.com/packages/tools/audio/audio-manager-cg-149123">Unity Asset Store</a> and use the package manager. Alternatively, download this repo and copy all files into your project.

## Setup
Unlike the ```2.x``` version of the asset. ```3.x``` doesn't have any user setup needed for it to actually function. Once you import the asset into your project, you'll be prompted to scan for audio, do the scan and you'll be all set for use. 


### Basic Scripting To Play A Clip
You can play audio either through the inspector clip player which lets you setup a clip or a group through an editor or through code. 

#### Inspector
![Untitled](https://github.com/CarterGames/AudioManager/assets/33253710/f19ee974-33c3-4dde-82eb-8d4148662471)

A inspector class to allow users to play audio from the audio library like you can with the normal API but just from the inspector. The editor has options to apply some of the edit modules, play a single track/defined group and listen to events the setup would normally trigger. 

To play a clip from the inspector player, just reference it to another class or use a button unity event etc. to call Play() on the class. Example:
```
[SerializeField] private InspectorAudioClipPlayer player;

private void OnEnable()
{
    player.Play();
}
```

#### Code
Like ```2.x``` the API is mostly the same but with a few edits. The same ```Play()```, ```PlayFromTime()```, ```PlayWithDelay()``` etc are present, but you can apply ```Edit Modules``` to any method which let you make these edits as needed. There are the common volume & pitch edits for all method variations, but for other edits the modules are used instead to save needing 1000s of lines of method overrides. 

```
private void OnEnable()
{
    // Plays the clip with no user edits.
    AudioManager.Play("MyClip");

    // Plays the clip with edits to volume.
    AudioManager.Play("MyClip", .5f);

    // Plays the clip with edits to volume via edit modules.
    AudioManager.Play("MyClip", new VolumeEdit(.5f));
}
```

You can play from a collection of clips with the group play methods, which work the exact same as the standard ones, but with groups. These can be defined in the library editor or in code should you wish. Example:

```
private void OnEnable()
{
    // Plays the group with no user edits.
    AudioManager.PlayGroup(Group.MyGroup);

    // Plays the group with edits to volume.
    AudioManager.PlayGroup(Group.MyGroup, .5f);

    // Plays the group with edits to volume via edit modules.
    AudioManager.PlayGroup(Group.MyGroup, new VolumeEdit(.5f));

    // Plays a group from an array of clip names.
    string[] clips = new string[3] { "Click_01", "Click_02", "Click_03" };
    AudioManger.PlayGroup(clips, GroupPlayMode.Random);
}
```

For more information on all of this, please consult the documentation: https://carter.games/docs/audiomanager/3x

## Example Scene
Please see the example project .zip for an example of the asset usage. It couldn't be provided in the package due to how the asset handles the audio library automation.

## Documentation
You can access a online of the documentation here: <a href="https://carter.games/audiomanager">Online Documentation</a>. A offline copy if provided with the package and asset if needed. 

## Authors
- <a href="https://github.com/JonathanMCarter">Jonathan Carter</a>

## Additional Contributors
- <a href="https://github.com/Yemeni">Yousef Al-Hadhrami</a> - (Legacy - V:2.6.1) - Hotfix to AudioPool.cs class throwing a null reference exception error.

## Licence
MIT Licence
