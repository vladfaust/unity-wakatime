using UnityEditor;
using UnityEngine;

namespace WakaTime
{
    public class ProjectEditWindow : EditorWindow
    {
        private static ProjectEditWindow _window;
        
        private static readonly Vector2 Size = new Vector2(400, 138);
        private static string[] _projectSettings;
        private static string _branch;
        private static readonly GUIStyle BranchStyle = new GUIStyle(EditorStyles.helpBox) {richText = true};
        
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
            
            var pos = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) - Size;
            _window.position = new Rect(pos/2, Size);
            _window.titleContent = new GUIContent("Change project name");
            
            _projectSettings = new[] {"", ""};
            Plugin.GetProjectFile().CopyTo(_projectSettings, 0);
            _branch = string.IsNullOrEmpty(_projectSettings[1]) ? "<i>none</i>" : _projectSettings[1];
        }
        
        void OnGUI()
        {
            EditorGUILayout.LabelField("Overrides project name while sending to WakaTime. If empty, will be used Product Name from PlayerSettings", BranchStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Project name");
                _projectSettings[0] = EditorGUILayout.TextField(_projectSettings[0]);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Overrides branch name while sending to WakaTime. If empty, will be used current branch <i>(not implemented in this plugin yet)</i>", BranchStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Current branch override");
                EditorGUILayout.SelectableLabel(_branch, BranchStyle,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
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