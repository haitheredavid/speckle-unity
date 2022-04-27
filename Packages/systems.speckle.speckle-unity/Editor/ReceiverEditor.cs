using System.Collections.Generic;
using System.Linq;
using Speckle.ConnectorUnity.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Speckle.ConnectorUnity
{
	[CustomEditor(typeof(Receiver))]
	public class ReceiverEditor : Editor
	{
		private DropdownField branches, commits, converters;

		private Receiver obj;
		private StreamPreview preview;
		private ProgressBar progress;

		private VisualElement root;
		private Button searchButton, runButton;
		private Toggle showPreview, renderPreview;
		private TextField streamUrlField;
		private VisualTreeAsset tree;

		private void OnEnable()
		{
			obj = (Receiver)target;

			obj.onRepaint += RefreshAll;
			obj.onPreviewSet += Refresh;

			tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/systems.speckle.speckle-unity/GUI/Elements/SpeckleClient/Receiver.uxml");
		}

		private void OnDisable()
		{
			obj.onRepaint -= RefreshAll;
			obj.onPreviewSet -= Refresh;
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
			streamUrlField.value = obj.StreamUrl;

			searchButton = root.Q<Button>("search-button");
			searchButton.clickable.clicked += () =>
			{
				if (SpeckleConnector.TryGetSpeckleStream(streamUrlField.value, out var speckleStream))
					obj.Init(speckleStream);
			};

			runButton = root.Q<Button>("receive");
			runButton.clickable.clicked += () =>
			{
				if (!obj.isWorking)
					obj.Receive();
			};

			preview = root.Q<StreamPreview>("preview");
			preview.thumbnail.image = GetPreview();

			showPreview = root.Q<Toggle>("show-preview");
			showPreview.RegisterCallback<ClickEvent>(_ => { preview.thumbnail.image = GetPreview(); });

			renderPreview = root.Q<Toggle>("render-preview");
			renderPreview.RegisterCallback<ClickEvent>(_ => obj.RenderPreview());

			progress = root.Q<ProgressBar>("receive-progress");
			obj.onProgressReport += values =>
			{
				Debug.Log($"Value update with {values.Count}");
				foreach (var v in values)
				{
					Debug.Log(v.Key + "-" + v.Value);
				}
				progress.value = values.Values.FirstOrDefault() / 100f;
			};

			return root;
		}

		private Texture GetPreview()
		{
			return obj.ShowPreview ? obj.Preview : null;
		}

		private void RefreshAll()
		{
			Refresh(branches, obj.Branches.Format().ToList(), "branchIndex");
			Refresh(commits, obj.Commits.Format().ToList(), "commitIndex");
		}

		private void Refresh()
		{
			preview.thumbnail.image = obj.Preview;
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