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

    private DropdownField branches, commits;
    private Receiver obj;
    private VisualElement root;
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

      //
      // converters = SetDropDown("converter", obj.Converters.Format(), e => Debug.Log(e.newValue));

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