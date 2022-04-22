using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity.GUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Speckle.ConnectorUnity
{
  [CustomEditor(typeof(SpeckleConnector))]
  public class SpeckleConnectorEditor : Editor
  {

    private DropdownField accounts, streams, branches, commits;
    private SpeckleConnector obj;
    private VisualElement root;
    private VisualTreeAsset tree;

    private void OnEnable()
    {
      tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/systems.speckle.speckle-unity/GUI/SpeckleConnectorEditor.uxml");
      obj = (SpeckleConnector)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
      if (tree == null)
        return base.CreateInspectorGUI();

      root = new VisualElement();
      tree.CloneTree(root);

      root.Q<ObjectField>("stream").objectType = typeof(SpeckleStream);

      accounts = SetDropDown("accounts", "accountIndex", obj.accounts.Format(), e => AccountChange(e).Forget());

      streams = SetDropDown("streams", "streamIndex", obj.streams.Format(), e => StreamChange(e).Forget());
      branches = SetDropDown("branches", "branchIndex", obj.branches.Format(), BranchChange);
      commits = SetDropDown("commits", "commitIndex", obj.commits.Format(), CommitChange);

      return root;
    }

    private DropdownField SetDropDown(string fieldName, string propName, IEnumerable<string> items, Action<ChangeEvent<string>> callback)
    {
      var dropDown = root.Q<DropdownField>(fieldName);
      dropDown.choices = items.ToList();
      dropDown.index = FindInt(propName);
      dropDown.RegisterValueChangedCallback(callback.Invoke);
      return dropDown;
    }

    private async UniTaskVoid AccountChange(ChangeEvent<string> evt)
    {
      var inputA = evt.newValue.ParseAccountEmail();
      var inputB = evt.newValue.ParseAccountServer();

      if (obj == null)
        return;

      var index = -1;
      for (var i = 0; i < obj.accounts.Count; i++)
      {
        var item = obj.accounts[i];
        if (item != null && item.userInfo.email.Equals(inputA) && item.serverInfo.name.Equals(inputB))
        {
          Debug.Log($"Setting active {nameof(item)} to {inputA}-{inputB}");
          index = i;
          break;
        }
      }

      if (index < 0)
        return;

      await obj.LoadAccount(index);
      RefreshAll();
    }

    private async UniTask StreamChange(ChangeEvent<string> evt)
    {
      var itemA = evt.newValue.ParseStreamName();
      var itemB = evt.newValue.ParseStreamId();
      var index = -1;

      for (var i = 0; i < obj.streams.Count; i++)
      {
        var item = obj.streams[i];
        if (item != null && item.name.Equals(itemA) && item.id.Equals(itemB))
        {
          Debug.Log($"Setting active {nameof(item)} to {itemA} | {itemB}");
          index = i;
          break;
        }
      }

      if (index < 0)
        return;

      await obj.LoadStream(index);

      Refresh(streams, obj.streams.Format(), "streamIndex");
      Refresh(branches, obj.branches.Format(), "branchIndex");
      Refresh(commits, obj.commits.Format(), "commitIndex");
    }

    private void BranchChange(ChangeEvent<string> evt)
    {
      Debug.Log(evt.newValue + $" from {evt.previousValue}");
      var itemA = evt.newValue;
      for (var i = 0; i < obj.branches.Count; i++)
      {
        var item = obj.branches[i];
        if (item != null && item.name.Equals(itemA))
        {
          obj.LoadBranch(i);
          break;
        }
      }

      Refresh(commits, obj.commits.Format(), "commitIndex");
    }

    private void CommitChange(ChangeEvent<string> evt)
    {
      var itemA = evt.newValue.ParseCommitId();
      var itemB = evt.newValue.ParseCommitMsg();

      for (var i = 0; i < obj.commits.Count; i++)
      {
        var item = obj.commits[i];
        if (item != null && item.id.Equals(itemA) && item.message.Equals(itemB))
        {
          Debug.Log($"Setting active commit to {itemA} | {itemB}");
          obj.LoadCommit(i);
          break;
        }
      }
    }

    private int FindInt(string propName)
    {
      return serializedObject.FindProperty(propName).intValue;
    }

    private void RefreshAll()
    {
      Refresh(accounts, obj.accounts.Format(), "accountIndex");
      Refresh(streams, obj.streams.Format(), "streamIndex");
      Refresh(branches, obj.branches.Format(), "branchIndex");
      Refresh(commits, obj.commits.Format(), "commitIndex");
    }

    private void Refresh(DropdownField dropdown, IEnumerable<string> items, string prop)
    {
      dropdown.choices = items.ToList();
      dropdown.index = FindInt(prop);
    }
  }
}