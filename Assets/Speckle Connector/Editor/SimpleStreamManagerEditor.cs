using System;
using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle_Connector;
using UnityEditor;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  [CustomEditor(typeof(SimpleStreamManager))]
  public class SimpleStreamManagerEditor : Editor
  {

    private void Awake()
    {
      _current = new ManagerCache();
      _stale = new ManagerCache();
      progressMessage = "I'm awake";
    }

    private bool _foldOutAccount;

    private int _totalChildrenCount = 0;

    private StreamManager _streamManager;

    private ManagerCache _current, _stale;

    private int SelectedAccountIndex
    {
      get { return _current.account; }
      set { _current.account = value; }
    }

    private int SelectedStreamIndex
    {
      get { return _streamManager.SelectedStreamIndex; }
      set { _streamManager.SelectedStreamIndex = value; }
    }

    private int SelectedBranchIndex
    {
      get { return _streamManager.SelectedBranchIndex; }
      set { _streamManager.SelectedBranchIndex = value; }
    }

    private int SelectedCommitIndex
    {
      get { return _streamManager.SelectedCommitIndex; }
      set { _streamManager.SelectedCommitIndex = value; }
    }

    private int OldSelectedAccountIndex
    {
      get { return _stale.account; }
      set { _stale.account = value; }
    }

    private int OldSelectedStreamIndex
    {
      get { return _streamManager.OldSelectedStreamIndex; }
      set { _streamManager.OldSelectedStreamIndex = value; }
    }

    private void UpdateInputs(int account, int stream, int branch, int commit)
    { }

    private string progressMessage = "Hello!";

    public override async void OnInspectorGUI()
    {
      DrawDefaultInspector( );
      serializedObject.Update( );
      
      var manager = (SimpleStreamManager) target;
      

      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.TextField("Progress", progressMessage,
                                GUILayout.Height(20),
                                GUILayout.ExpandWidth(true));
      EditorGUI.EndDisabledGroup();



      EditorGUILayout.BeginHorizontal();

      SelectedAccountIndex = EditorGUILayout.Popup("Accounts", SelectedAccountIndex, manager.GetAccounts,
                                                   GUILayout.ExpandWidth(true), GUILayout.Height(20));

      if (OldSelectedAccountIndex != SelectedAccountIndex)
      {
        progressMessage = "Selecting New Account";
        await manager.SelectAccount(0);
        return;
      }
      EditorGUILayout.EndHorizontal();

      
      if (!manager.Primed)
      {
        progressMessage = "Manager is not Primed Yet";
      }

    }

    internal class ManagerCache
    {
      public int account = -1;
      public int stream = -1;
      public int branch = -1;
      public int commit = -1;
    }

  }
}