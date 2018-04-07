# unity-wakatime
A *minimalistic* WakaTime plugin for Unity

## About

Existing solutions (neither https://github.com/josec89/wakatime-unity nor https://github.com/bengsfort/WakaTime-Unity) didn't work for me, so I decided to implement my own variant. This plugin doesn't have any visual Unity Editor integration.

The code has been successfuly tested with following Unity versions:

* 2017.4.0f1
* 2018.1.0b13 (although getting `'UnityEditor.EditorApplication.hierarchyWindowChanged' is obsolete: Use 'EditorApplication.hierarchyChanged'` warning)

## Installation

1. Copy `WakaTime.cs` somewhere into your project (maybe `Plugins/WakaTime/Editor/WakaTime.cs`)
2. Edit `apiKey` variable within the script (grab from https://wakatime.com/settings/account)
3. (optional) Set `isDebug` variable to `false` to disable logging
4. Check if `"Unity"` editor appears at https://wakatime.com/api/v1/users/current/user_agents
5. Enjoy!
