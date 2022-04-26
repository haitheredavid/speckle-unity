using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Speckle.ConnectorUnity.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Speckle.ConnectorUnity
{
  [CustomEditor(typeof(SpeckleConnector))]
  public class SpeckleConnectorEditor : Editor
  {

    private DropdownField accounts, streams;
    private DropdownField branches, commits;
    private DropdownField converters;

    private Image img;
    private SpeckleConnector obj;
    private VisualElement root;

    private VisualTreeAsset streamCard;
    private ListView streamList;
    private VisualTreeAsset tree;

    private void OnEnable()
    {
      obj = (SpeckleConnector)target;
      obj.onRepaint += RefreshAll;

      tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/systems.speckle.speckle-unity/GUI/SpeckleConnectorEditor.uxml");
      streamCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/systems.speckle.speckle-unity/GUI/Elements/StreamCard/StreamCard.uxml");
    }

    private void OnDisable()
    {
      obj.onRepaint -= RefreshAll;
    }

    public override VisualElement CreateInspectorGUI()
    {
      if (tree == null)
        return base.CreateInspectorGUI();

      root = new VisualElement();
      tree.CloneTree(root);

      accounts = SetDropDown("account", obj.Accounts.Format(), e => AccountChange(e).Forget());

      // streams = SetDropDown("stream", obj.Streams.Format(), e => StreamChange(e, streams.choices).Forget());
      // branches = SetDropDown("branch", obj.Branches.Format(), e => DropDownChange(e, branches.choices, i =>
      // {
      //   obj.LoadBranch(i);
      //   Refresh(commits, obj.Commits.Format(), "commitIndex");
      // }));
      //
      // commits = SetDropDown("commit", obj.Commits.Format(), e => DropDownChange(e, commits.choices, i => obj.LoadCommit(i)));
      //
      //
      // converters = SetDropDown("converter", obj.Converters.Format(), e => Debug.Log(e.newValue));

      SetupList();

      return root;
    }

    private void SetupList()
    {

      VisualElement makeItem()
      {
        var card = new VisualElement();
        streamCard.CloneTree(card);
        return card;
      }

      void bindItem(VisualElement e, int i)
      {
        var stream = obj.Streams[i];

        e.Q<Label>("title").text = stream.name;
        e.Q<Label>("id").text = stream.id;
        e.Q<Label>("description").text = stream.description;

        var isActive = FindInt("streamIndex") == i;

        e.style.backgroundColor = isActive ? new StyleColor(Color.white) : new StyleColor(Color.clear);

        var ops = e.Q<VisualElement>("operation-container");
        if (ops == null)
        {
          Debug.Log("No container found");
          return;
        }

        ops.visible = isActive;

        // unbind all objects first just in case
        ops.Q<Button>("open-button").clickable.clickedWithEventInfo -= obj.OpenStreamInBrowser;
        ops.Q<Button>("send-button").clickable.clickedWithEventInfo -= obj.CreateSender;
        ops.Q<Button>("receive-button").clickable.clickedWithEventInfo -= obj.CreateReceiver;

        if (isActive)
        {
          ops.Q<Button>("open-button").clickable.clickedWithEventInfo += obj.OpenStreamInBrowser;
          ops.Q<Button>("send-button").clickable.clickedWithEventInfo += obj.CreateSender;
          ops.Q<Button>("receive-button").clickable.clickedWithEventInfo += obj.CreateReceiver;
        }
      }

      streamList = root.Q<ListView>("stream-list");

      streamList.bindItem = bindItem;
      streamList.makeItem = makeItem;
      streamList.fixedItemHeight = 50f;

      SetAndRefreshList();

      // Handle updating all list view items after selection 
      streamList.RegisterCallback<ClickEvent>(_ => streamList.RefreshItems());

      // Pass new selection back to object
      streamList.onSelectedIndicesChange += i => obj.SetStream(i.FirstOrDefault());

    }

    private DropdownField SetDropDown(string fieldName, IEnumerable<string> items, Action<ChangeEvent<string>> callback)
    {
      var dropDown = root.Q<VisualElement>(fieldName + "-container").Q<DropdownField>("items");
      dropDown.choices = items.ToList();
      dropDown.index = FindInt(fieldName + "Index");
      dropDown.RegisterValueChangedCallback(callback.Invoke);
      return dropDown;
    }
    private void SetAndRefreshList()
    {
      streamList.ClearSelection();
      streamList.itemsSource = obj.Streams;
      streamList.RefreshItems();
    }

    private async UniTask AccountChange(ChangeEvent<string> evt)
    {
      var index = accounts.DropDownChange(evt);

      if (index < 0)
        return;

      await obj.LoadAccountAndStream(index, false);
      RefreshAll();
    }

    private int FindInt(string propName)
    {
      return serializedObject.FindProperty(propName).intValue;
    }

    private void RefreshAll()
    {
      Refresh(accounts, obj.Accounts.Format(), "accountIndex");
      SetAndRefreshList();
    }

    private void Refresh(DropdownField dropdown, IEnumerable<string> items, string prop)
    {
      dropdown.choices = items.ToList();
      dropdown.index = FindInt(prop);
    }
  }
}