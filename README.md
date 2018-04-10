# unity-wakatime
A [WakaTime](https://wakatime.com) plugin for [Unity](https://unity.com).

## About

Existing solutions (neither https://github.com/josec89/wakatime-unity nor https://github.com/bengsfort/WakaTime-Unity) didn't work for me, so I decided to implement my own variant.

The code has been successfuly tested with following Unity versions:

* 2017.4.0f1
* 2018.1.0b13 (although getting `'UnityEditor.EditorApplication.hierarchyWindowChanged' is obsolete: Use 'EditorApplication.hierarchyChanged'` warning)

## Installation

1. Copy `Plugins` folder into your project Assets
2. Go to `Window/WakaTime` in Editor and set API key (grab one from https://wakatime.com/settings/account)
3. Check if `"Unity"` editor appears at https://wakatime.com/api/v1/users/current/user_agents (may be a bit delayed)
4. Enjoy!

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
