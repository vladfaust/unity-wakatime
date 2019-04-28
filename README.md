# unity-wakatime
A [WakaTime](https://wakatime.com) plugin for [Unity](https://unity.com).

![Screenshot](https://user-images.githubusercontent.com/7955682/38732057-79cf45b4-3f25-11e8-958f-07ba5290caba.PNG)

## About

Existing solutions didn't work for me (https://github.com/bengsfort/WakaTime-Unity is obsolete and https://github.com/josec89/wakatime-unity requires Python), so I decided to implement my own variant.

The code has been successfully tested with following Unity versions:

* 2017.4.0f1
* 2018.1.0b13
* 2018.1.2f1
* 2018.3.6f1
* 2018.3.10f1
* 2018.3.14f1
* 2019.1.0f2

## Installation using the Unity Package Manager (Unity 2018.1+)

The [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html) (UPM) is a new method to manage external packages. It keeps package contents separate from your main project files.

1. Modify your project's `Packages/manifest.json` file to add the line:
    ```json
    "com.vladfaust.unitywakatime": "https://github.com/vladfaust/unity-wakatime.git#upm"
    ```
    Make sure it's still a valid JSON file. For example:
    ```json
    {
        "dependencies": {
            "com.unity.ads": "2.0.8",
            "com.vladfaust.unitywakatime": "https://github.com/vladfaust/unity-wakatime.git#upm"
        }
    }
    ```
2. To manually update you need to delete from `lock` section in `Packages/manifest.json` something like this:
    ```json
    "com.vladfaust.unitywakatime": {
      "hash": "31fe84232fc9f9c6e9606dc9e5a285886a94f26b",
      "revision": "upm"
    }
    ```

## Installation (all other Unity versions)

If not using the Unity Package Manager, copying the files locally also work.

1. Copy the `Editor` folder from inside `com.vladfaust.unitywakatime` into your project Assets

## Setup

1. Run the Unity editor, go to `Window/WakaTime`, and insert your API key (grab one from https://wakatime.com/settings/account)
1. Press `Save Preferences`
1. Check if `"Unity"` editor appears at https://wakatime.com/api/v1/users/current/user_agents (may be a bit delayed)
1. Enjoy!

## Usage

The plugin will automatically send heartbeats to WakaTime after following events:

* DidReloadScripts
* EditorApplication.playModeStateChanged
* EditorApplication.contextualPropertyMenu
* EditorApplication.hierarchyWindowChanged
* EditorSceneManager.sceneSaved
* EditorSceneManager.sceneOpened
* EditorSceneManager.sceneClosing
* EditorSceneManager.newSceneCreated

[![Become Patron](https://vladfaust.com/img/patreon-small.svg)](https://www.patreon.com/vladfaust)
