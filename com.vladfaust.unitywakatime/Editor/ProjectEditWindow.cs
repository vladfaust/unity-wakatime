using System.IO;
using UnityEditor;
using UnityEngine;

namespace WakaTime
{
    public class ProjectEditWindow : EditorWindow
    {
        private static ProjectEditWindow _window;
        
        private static Vector2 _size = new Vector2(400, 138);
        private static string[] _projectSettings;
        private static string _branch;
        private static readonly GUIStyle RichHelpBoxStyle = new GUIStyle(EditorStyles.helpBox) {richText = true};
        private static bool _isProjectFileMissed;

        public static void Display()
        {
            if (_window)
            {
                FocusWindowIfItsOpen<ProjectEditWindow>();
            }
            else
            {
                _window = CreateInstance<ProjectEditWindow>();
                _window.ShowPopup();
            }

            var pos = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) - _size;
            

            _projectSettings = new[] {"", ""};
            var projectFile = Plugin.GetProjectFile();
            
            if (projectFile == null)
                _isProjectFileMissed = true;
            else
            {
                _isProjectFileMissed = false;
                projectFile.CopyTo(_projectSettings, 0);
            }
            _branch = string.IsNullOrEmpty(_projectSettings[1]) ? "" : _projectSettings[1];

            _size.y = _isProjectFileMissed ? 170 : 138;
            _window.position = new Rect(pos / 2, _size);
            _window.titleContent = new GUIContent("Change project name");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("A project name to send to WakaTime (Product Name from Player Settings by default)", RichHelpBoxStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Project name");
                _projectSettings[0] = EditorGUILayout.TextField(_projectSettings[0]);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("A branch name to send to WakaTime (current git branch by default) <i>(not implemented in this plugin yet)</i>", RichHelpBoxStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Current branch override");
                EditorGUILayout.SelectableLabel(_branch, RichHelpBoxStyle,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
            if (_isProjectFileMissed)
                GUILayout.Label($"<b>{Path.GetFullPath(".wakatime_project")}</b> is missing. Press Save to create it", RichHelpBoxStyle);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save"))
                {
                    Plugin.SetProjectFile(_projectSettings);
                    Plugin.Initialize();
                    CloseAndNull();
                    FocusWindowIfItsOpen<Window>();
                }

                if (GUILayout.Button("Cancel"))
                {
                    CloseAndNull();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnLostFocus()
        {
            CloseAndNull();
        }

        private void CloseAndNull()
        {
            Close();
            _window = null;
        }
    }
}