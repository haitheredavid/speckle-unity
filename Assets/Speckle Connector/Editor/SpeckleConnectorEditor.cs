using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConnectorUnity.GUI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectorUnity
{

  [CustomEditor(typeof(SpeckleConnector))]
  [CanEditMultipleObjects]
  public class SpeckleConnectorEditor : Editor
  {

    private SerializedProperty managerData;
    private SerializedProperty receivers;
    private SerializedProperty receiveEvent;

    private IEnumerable<MethodInfo> _methods;
    
    private void OnEnable()
    {
      managerData = serializedObject.FindProperty("input");
      receivers = serializedObject.FindProperty("receivers");
      receiveEvent = serializedObject.FindProperty("receiveEvent");

      _methods = ReflectionUtility.GetAllMethods(
        target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();


      EditorGUILayout.LabelField("stream setup", SpeckleGUIStyle.LabelStyle());
      EditorGUILayout.PropertyField(managerData, GUILayout.ExpandWidth(true));

      EditorGUILayout.Space();


      EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

      foreach (var methodInfo in _methods)
      {
        if (methodInfo.GetParameters().All(p => p.IsOptional))
        {
          var buttonAttribute = (ButtonAttribute)methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
          if (GUILayout.Button(buttonAttribute.Text, SpeckleGUIStyle.ButtonStyle()))
          {
            object[] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();
            IEnumerator methodResult = methodInfo.Invoke(target, defaultParams) as IEnumerator;

            if (!Application.isPlaying)
            {
              EditorUtility.SetDirty(target);
              EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
          }
        }
      }

      EditorGUILayout.EndHorizontal();

      EditorGUILayout.PropertyField(receivers, GUILayout.ExpandWidth(true));


      EditorGUILayout.Space();

      // EditorGUILayout.PropertyField(receiveEvent, GUILayout.ExpandWidth(true));


      serializedObject.ApplyModifiedProperties();
    }

  }
}