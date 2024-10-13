#if UNITY_EDITOR
using Shrimp2DWFC.Runtime;
using UnityEditor;
using UnityEngine;

namespace EditorExtension
{
    [CustomEditor(typeof(ModuleData))]
    public class ModuleDataInspectorEditor : Editor
    {
        private ModuleData _target;

        private void OnEnable()
        {
            _target = target as ModuleData;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Set name"))
            {
                _target._moduleName = _target.gameObject.name;
            }
        }
    }
}
#endif