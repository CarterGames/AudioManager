![AM Banner 1200x630](https://github.com/CarterGames/AudioManager/assets/33253710/c12da612-d4a7-40ff-9fe2-0eae107a402b)

<b>Audio Manager</b> is a <b>FREE</b> audio/sound management library for Unity with options to play clips & background music for any game. 

## Badges
![CodeFactor](https://www.codefactor.io/repository/github/cartergames/audiomanager/badge?style=for-the-badge)
![GitHub all releases](https://img.shields.io/github/downloads/CarterGames/AudioManager/total?style=for-the-badge)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/CarterGames/AudioManager?style=for-the-badge)
![GitHub repo size](https://img.shields.io/github/repo-size/CarterGames/AudioManager?style=for-the-badge)

## Key Features
- Automatic clip adding so you don't have to.
- Optional satic instancing for the audio manager script.
- Basic Audio Player & Music Player included.
- Custom inspectors for main scripts.

## How To Install
Either download and import the package from the releases section or the <a href="https://assetstore.unity.com/packages/tools/audio/audio-manager-cg-149123">Unity Asset Store</a> and use the package manager. Alternatively, download this repo and copy all files into your project.

## Setup

### Audio Manager

![AM-Setup-1](https://user-images.githubusercontent.com/33253710/154436213-3b2d109a-b513-4012-b746-85a2214cf1d9.png)

First add the audio manager component to any gameobject you like. We recommend putting the script on a empty gameobject. Once added the inspector will show up asking for an Audio Manager File before moving forward. If this is your first time adding an Audio Manager script to an object, you'll find that the field may be populated for you by the scripts automatic setup which generates the file structure for the asset and makes a default Audio Manager File for you to use. 

![AM-Setup-2](https://user-images.githubusercontent.com/33253710/154436245-b61e06f7-ac93-45a9-bf67-14cacf35ca4d.png)

Next you'll need to assign the prefab that is to play the audio when you request it. We provide a correctly setup prefab in the package which we recommend you use with the asset. If you have lost it or are upgrading from an older version you may find your prefab is outdated. See the Audio Prefab section of the documentation for more on how the prefab should be setup. 

![AM-Setup-3](https://user-images.githubusercontent.com/33253710/154436310-81415441-fb4e-4cc7-bf4e-e2fc66d02309.png)

You can also assign audio mixers should you wish, though these are optional and are not required to use the manager to function. When you add mixer groups here you can use the ID listed next to them to use them when calling for a clip to be played without needed a direct reference to the mixer in the script you are calling from. 

![AM-Setup-4](https://user-images.githubusercontent.com/33253710/154436342-f727640a-f7e0-470c-a879-bb6b9726dc93.png)

From here you are almost ready to use the Audio Manager. The inspector should now show the options for directories & clips. Open the directories tab by pressing the show directories button if it is not already open. Here you'll see an empty field and a button to continue. If you audio clips are going to be stored in the base scan directory you can just press the continue button and the manager will scan and add any clips it finds. However if you want to use a folder structure you'll need to define the subdirectories here. See the How Scanning Works section of the documentation for examples of how to write these. You can add or remove elements with the + & - buttons along each row. When all the directories are valid the manager will scan, you may notice a little editor lag as this runs. 

![AM-Setup-5](https://user-images.githubusercontent.com/33253710/154436374-66e25f8c-dc89-464d-a59d-1203f8cefd89.png)

From here all of the clips in the directories selected should be in the clips section, press the show clips button to reveal the clips if needed, with both the directories and clip sections you can hide them to reduce the space the inspector takes up when not needed. Assuming you see the clips all listed you will be ready to use the Audio Manager. 

### Basic Scripting To Play A Clip

Static instance disabled

With not other edits
> audioManager.Play("MySound");

With the volume set to 0.5
> audioManager.Play("MySound", .5f);

Static instance enabled (an instance of the manager needs to be in the game still to function)

With not other edits
> AudioManager.instance.Play("MySound");

With the volume set to 0.5
> AudioManager.instance.Play("MySound", .5f);

Lots more methods provided which can be seen in the documentation.

## Documentation
You can access a online of the documentation here: <a href="https://carter.games/audiomanager">Online Documentation</a>. A offline copy if provided with the package and asset if needed. 

## Authors
- <a href="https://github.com/JonathanMCarter">Jonathan Carter</a>

## Additional Contributors
- <a href="https://github.com/Yemeni">Yousef Al-Hadhrami</a> - (2.6.1) - Hotfix to AudioPool.cs class throwing a null reference exception error.

## Licence
MIT Licence
