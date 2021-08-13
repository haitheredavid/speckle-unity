using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ConnectorUnity
{
  [CustomEditor(typeof(SimpleStreamManager))]
  public class SimpleStreamManagerEditor : Editor
  {

    private string[]
      _accounts = {"idle"},
      _streams = {"idle"},
      _branches = {"idle"},
      _commits = {"idle"},
      _kits = {"idle"};

    private string progressMessage = "Hello!";

    [SerializeField] private InputDataShell dataShell;
    private InputDataShell stale { get; set; } = new InputDataShell();

    private SerializedProperty kit, account, stream, branch, commit;
    private SerializedProperty cache;

    private void OnEnable()
    {
      Debug.Log("On Enable call ");

      cache = serializedObject.FindProperty("data");

      kit = serializedObject.FindProperty("kitIndex");
      account = serializedObject.FindProperty("accountIndex");
      stream = serializedObject.FindProperty("streamIndex");
      branch = serializedObject.FindProperty("branchIndex");
      commit = serializedObject.FindProperty("commitIndex");

      var manager = (SimpleStreamManager)serializedObject.targetObject;
      manager.StateChangeEvent += ManagerOnStateChange;
      ResetCache();
    }

    private void OnDisable()
    {
      Debug.Log("On Disable call ");
    }

    private async void Reset()
    {
      ResetCache();
      var manager = (SimpleStreamManager)serializedObject.targetObject;
      await manager.PrimeManager();
    }

    private void ResetCache()
    {
      dataShell = new InputDataShell();
      stale = new InputDataShell
      {
        account = -1, stream = -1, branch = -1, commit = -1
      };
    }

    private void ManagerOnStateChange(ManagerState state)
    {
      var manager = (SimpleStreamManager)target;
      progressMessage = state.ToString().Split('.').Last();

      switch (state)
      {
        case ManagerState.NoAccounts:
          Debug.LogWarning("No accounts found");
          ClearAll();
          break;
        case ManagerState.NoStreams:
          Debug.LogWarning("No streams found with this account");
          UpdateLists(true, true, true);
          break;
        case ManagerState.NoCommits:
          UpdateLists(false, false, true);
          break;

        case ManagerState.Primed:
          ClearAll();
          _accounts = manager.accounts.Format();
          break;

        case ManagerState.AccountSelected:
          UpdateLists(true, true, true);
          _streams = manager.streams.Format();
          break;

        case ManagerState.AccountLoaded:
          UpdateLists(false, true, true);
          _streams = manager.streams.Format();
          break;

        case ManagerState.StreamSelected:
          UpdateLists(false, true, true);
          _branches = manager.branches.Format();
          break;
        case ManagerState.BranchSelected:
          UpdateLists(false, false, false);
          _commits = manager.commits.Format();
          break;
        case ManagerState.CommitSelected:
          Debug.Log("commit selected and ready");
          break;

        case ManagerState.ClientError:
          UpdateLists(true, true, true);
          Debug.Log("Client was not setup with account");
          break;

        case ManagerState.KitSelected:
          _kits = manager.kits.Format();
          break;
        default:
          Debug.Log("Not valid content");
          ClearAll();
          break;
      }
      Repaint();
    }

    private void ClearAll()
    {
      ResetCache();
      _accounts = new[] {"none"};
      _streams = new[] {"none"};
      _branches = new[] {"none"};
      _commits = new[] {"none"};
    }

    private void UpdateLists(bool streams, bool branches, bool commits)
    {
      if (streams)
      {
        _streams = new[] {"none"};
        _branches = new[] {"none"};
        _commits = new[] {"none"};
        dataShell.stream = 0;
        dataShell.branch = 0;
        dataShell.commit = 0;
      }
      else if (branches)
      {
        _branches = new[] {"none"};
        _commits = new[] {"none"};
        dataShell.branch = 0;
        dataShell.commit = 0;
      }
      else if (commits)
      {
        _commits = new[] {"none"};
        dataShell.commit = 0;
      }
    }

    public override async void OnInspectorGUI()
    {
      var manager = (SimpleStreamManager)serializedObject.targetObject;
      var inProcess = manager.InProcess;
      var isValid = manager.IsValid && !inProcess;
      serializedObject.Update();

      GUILayout.BeginHorizontal();

      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.TextField("Progress", progressMessage, GUILayout.Height(20), GUILayout.ExpandWidth(true));
      EditorGUI.EndDisabledGroup();

      EditorGUI.BeginDisabledGroup(inProcess);
      if (GUILayout.Button("Referesh"))
      {
        await manager.PrimeManager();
        return;
      }
      EditorGUI.EndDisabledGroup();

      GUILayout.EndHorizontal();

      // EditorGUILayout.PropertyField(cache, true);
      // data.account = EditorGUILayout.Popup("test pop", data.account, _accounts, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("testvalues"));

      dataShell.account = EditorGUILayout.Popup("Accounts", dataShell.account, _accounts, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      dataShell.stream = EditorGUILayout.Popup("Streams", dataShell.stream, _streams, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      dataShell.branch = EditorGUILayout.Popup("Branches", dataShell.branch, _branches, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      dataShell.commit = EditorGUILayout.Popup("Commits", dataShell.commit, _commits, GUILayout.ExpandWidth(true), GUILayout.Height(20));

      GUILayout.BeginHorizontal();


      dataShell.kit = EditorGUILayout.Popup("Kits", dataShell.kit, _kits, GUILayout.ExpandWidth(true), GUILayout.Height(20));

      EditorGUI.BeginDisabledGroup(!isValid);

      if (GUILayout.Button("Recieve"))
        manager.Load();

      EditorGUI.EndDisabledGroup();

      GUILayout.EndHorizontal();

      serializedObject.FindProperty("kitIndex").intValue = dataShell.kit;
      serializedObject.FindProperty("accountIndex").intValue = dataShell.account;
      serializedObject.FindProperty("streamIndex").intValue = dataShell.stream;
      serializedObject.FindProperty("branchIndex").intValue = dataShell.branch;
      serializedObject.FindProperty("commitIndex").intValue = dataShell.commit;

      serializedObject.ApplyModifiedProperties();
    }

    private bool IsStale()
    {
      var value = false;
      // if (!data.Compare(stale))
      // {
      //   value = true;
      //   stale.account = data.account;
      //   stale.stream = data.stream;
      //   stale.branch = data.branch;
      //   stale.commit = data.commit;
      //   stale.kit = data.kit;
      // }
      return value;
    }
  }

}