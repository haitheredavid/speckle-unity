using System.Collections.Generic;
using System.Linq;
using Speckle.ConnectorUnity.GUI;
using UnityEditor;
using UnityEngine.UIElements;

namespace Speckle.ConnectorUnity
{
  [CustomEditor(typeof(Receiver))]
  public class ReceiverEditor : Editor
  {

    private DropdownField branches, commits, converters;

    private Receiver obj;

    private VisualElement root;
    private Button searchButton, runButton;
    private TextField streamUrlField;
    private VisualTreeAsset tree;

    private void OnEnable()
    {
      obj = (Receiver)target;
      obj.onRepaint += () =>
      {
        Refresh(branches, obj.Branches.Format().ToList(), "branchIndex");
        Refresh(commits, obj.Commits.Format().ToList(), "commitIndex");
      };

      tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/systems.speckle.speckle-unity/GUI/Elements/SpeckleClient/Receiver.uxml");
    }

    public override VisualElement CreateInspectorGUI()
    {
      if (tree == null)
        return base.CreateInspectorGUI();

      root = new VisualElement();
      tree.CloneTree(root);

      branches = root.SetDropDown(
        "branch",
        FindInt("branchIndex"),
        obj.Branches.Format(),
        e => branches.DropDownChange(e, i =>
        {
          obj.SetBranch(i);
          Refresh(commits, obj.Commits.Format(), "commitIndex");
        }));

      commits = root.SetDropDown(
        "commit",
        FindInt("commitIndex"),
        obj.Commits.Format(),
        e => commits.DropDownChange(e, i => { obj.SetCommit(i); }));

      converters = root.SetDropDown(
        "converter",
        FindInt("converterIndex"),
        obj.Converters.Format(),
        e => converters.DropDownChange(e, i => { obj.SetConverter(i); }));

      streamUrlField = root.Q<TextField>("url");

      searchButton = root.Q<Button>("search-button");
      searchButton.clickable.clicked += () =>
      {
        if (SpeckleConnector.TryGetSpeckleStream(streamUrlField.value, out var speckleStream))
          obj.Init(speckleStream);
      };


      return root;
    }

    private void Refresh(DropdownField dropdown, IEnumerable<string> items, string prop)
    {
      dropdown.choices = items.ToList();
      dropdown.index = FindInt(prop);

    }
    private int FindInt(string propName)
    {
      return serializedObject.FindProperty(propName).intValue;
    }
  }

}