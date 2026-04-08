![AM Logo 160](https://github.com/user-attachments/assets/b587215c-2cae-4b16-bd9a-9e52ede3dd5e)

# Audio Manager

A free audio/sound management system for Unity with automatic library management & more!

![GitHub release (latest by date)](https://img.shields.io/github/v/release/CarterGames/AudioManager?style=for-the-badge&color=bf6f31)
<br>
![GitHub License](https://img.shields.io/github/license/CarterGames/AudioManager?style=for-the-badge&color=1e77fa)
<br>
![GitHub all releases](https://img.shields.io/github/downloads/CarterGames/AudioManager/total?style=for-the-badge&color=8d6ca1)
<br>
![GitHub repo size](https://img.shields.io/github/repo-size/CarterGames/AudioManager?style=for-the-badge)
<br>
![Unity](https://img.shields.io/badge/Unity-2020.3.x_or_higher-critical?style=for-the-badge&color=757575)
<br/>

## Key Features
✔️ Automatic scanning of audio clips in the project.<br>
✔️ Dynamic start time for each clip to start where it starts playing audable audio, cutting out deadspace.<br>
✔️ Flexible API for playing audio clips or groups of clips.<br>
✔️ Editor to manage the library, assign groups of clips together and music track lists.<br>
✔️ Entirely static API, no scene references needed.<br>
✔️ Inspector players for quick prototyping without needing to write any code.<br>
<br/>

## Unity Supported Versions
The asset is developed and maintained in 2020.3.x and make use of available .Net updates in the version. Older versions of Unity are not supported for this asset. The asset has been tested pre-release in its development version: 2020.3.0f1.
<br/>

## Legacy 2.x version
Any 2.x changes will be based around the `2.x` branch in this repo.
<br/>
<br/>

## How To Install

### Unity Package Manager (Git URL) [Recommended]
<b>Latest:</b>
<br>
<i>The most up-to-date version of the repo that is considered stable enough for public use.</i>

```
https://github.com/CarterGames/AudioManager.git
```
<br>
<b>Specific branch:</b>
<br>
<i>You can also pull any public branch with the following structure.</i>

```
https://github.com/CarterGames/AudioManager.git#[BRANCH NAME HERE]
```

<i>An example using the pre-release branch for 3.0.0 would be:</i>

```
https://github.com/CarterGames/AudioManager.git#prerelease/3.0.0
```
<br>
<b>Unity Package:</b>
<br>
<i>You can download the .unitypackage from each release of the repo to get a importable package of that version. You do sacrifice the ease of updating if you go this route. See the latest releases <a href="https://github.com/CarterGames/AudioManager/releases">here</a></i>
<br/>

## Setup & Basic Usage
Unlike the 2.x version of the asset. 3.x doesn't have any user setup needed for it to actually function. Once you import the asset into your project, you'll be prompted to scan for audio, do the scan and you'll be all set for use.

```For more detailed instructions and API reference, please refer to the documentation.```
<br/><br/>

### Basic Scripting To Play A Clip
You can play audio either through the inspector clip player which lets you setup a clip or a group through an editor or through code.

#### Inspector
![am_player_inspector_example](https://github.com/CarterGames/AudioManager/assets/33253710/f19ee974-33c3-4dde-82eb-8d4148662471)

A inspector class to allow users to play audio from the audio library like you can with the normal API but just from the inspector. The editor has options to apply some of the edit modules, play a single track/defined group and listen to events the setup would normally trigger.


To play a clip from the inspector player, just reference it to another class or use a button unity event etc. to call `Play()` on the class. 

Example:
``` csharp
[SerializeField] private InspectorAudioClipPlayer player;

private void OnEnable()
{
    player.Play();
}
```
<br/>

#### Code
Like ```2.x``` the API is mostly the same but with a few edits. The same ```Play()```, ```PlayFromTime()```, ```PlayWithDelay()``` etc are present, but you can apply ```Edit Modules``` to any method which let you make these edits as needed. There are the common volume & pitch edits for all method variations, but for other edits the modules are used instead to save needing 1000s of lines of method overrides.

``` csharp
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

``` csharp
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
<br/>



## Documentation
You can access a online of the documentation here: <a href="https://carter.games/audiomanager">Online Documentation</a>. An offline copy is provided with the package and asset if needed. 
<br><br>

## Authors
- <a href="https://github.com/JonathanMCarter">Jonathan Carter</a>
<br><br>

## Licence
GNU V3
