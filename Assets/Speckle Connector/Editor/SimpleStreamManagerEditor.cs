using System.Linq;
using Speckle_Connector;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  [CustomEditor(typeof(SimpleStreamManager))]
  public class SimpleStreamManagerEditor : Editor
  {

    private EditorCoroutine logCoro;
    private string[]
      _accounts = new[] {"idle"},
      _streams = new[] {"idle"},
      _branches = new[] {"idle"},
      _commits = new[] {"idle"};

    private string progressMessage = "Hello!";

    private ManagerCache cache { get; set; } = new ManagerCache();
    private ManagerCache stale { get; set; } = new ManagerCache();

    private void OnEnable()
    {
      var manager = (SimpleStreamManager)serializedObject.targetObject;
      manager.StateChangeEvent += ManagerOnStateChange;
      ResetCache();
    }

    private void ResetCache()
    {
      cache = new ManagerCache();
      stale = new ManagerCache
      {
        account = -1, stream = -1, branch = -1, commit = -1
      };

    }

    private void ManagerOnStateChange(SimpleStreamManager.ManagerState state)
    {
      var manager = (SimpleStreamManager)target;
      progressMessage = state.ToString().Split('.').Last();

      switch (state)
      {
        case SimpleStreamManager.ManagerState.NoAccounts:
          Debug.LogWarning("No accounts found");
          ClearAll();
          break;
        case SimpleStreamManager.ManagerState.NoStreams:
          Debug.LogWarning("No streams found with this account");
          UpdateLists(true, true, true);
          break;
        case SimpleStreamManager.ManagerState.NoCommits:
          UpdateLists(false, false, true);
          break;

        case SimpleStreamManager.ManagerState.Primed:
          ClearAll();
          _accounts = manager.accounts.Format();
          break;

        case SimpleStreamManager.ManagerState.AccountSelected:
          UpdateLists(true, true, true);
          _streams = manager.streams.Format();
          break;
        
        case SimpleStreamManager.ManagerState.AccountLoaded:
          UpdateLists(false, true, true);
          _streams = manager.streams.Format();
          break;
        
        case SimpleStreamManager.ManagerState.StreamSelected:
          UpdateLists(false, true, true);
          _branches = manager.branches.Format();
          break;
        case SimpleStreamManager.ManagerState.BranchSelected:
          UpdateLists(false, false, false);
          _commits = manager.commits.Format();
          break;
        case SimpleStreamManager.ManagerState.CommitSelected:
          Debug.Log("commit selected and ready");
          break;
        
        case SimpleStreamManager.ManagerState.ClientError:
          UpdateLists(true, true, true);
          Debug.Log("Client was not setup with account");
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
        cache.stream = 0;
        cache.branch = 0;
        cache.commit = 0;
      }
      else if (branches)
      {
        _branches = new[] {"none"};
        _commits = new[] {"none"};
        cache.branch = 0;
        cache.commit = 0;
      }
      else if (commits)
      {
        _commits = new[] {"none"};
        cache.commit = 0;
      }
    }

    public override async void OnInspectorGUI()
    {
      // DrawDefaultInspector( );
      var manager = (SimpleStreamManager)serializedObject.targetObject;

      GUILayout.BeginHorizontal();
      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.TextField("Progress", progressMessage, GUILayout.Height(20), GUILayout.ExpandWidth(true));
      EditorGUI.EndDisabledGroup();

      EditorGUI.BeginDisabledGroup(manager.InProcess);
      if (GUILayout.Button("Load Accounts!"))
      {
        await manager.PrimeManager();
        return;
      }
      EditorGUI.EndDisabledGroup();
      GUILayout.EndHorizontal();

      cache.account = EditorGUILayout.Popup("Accounts", cache.account, _accounts, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      cache.stream = EditorGUILayout.Popup("Streams", cache.stream, _streams, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      cache.branch = EditorGUILayout.Popup("Branches", cache.branch, _branches, GUILayout.ExpandWidth(true), GUILayout.Height(20));
      cache.commit = EditorGUILayout.Popup("Commits", cache.commit, _commits, GUILayout.ExpandWidth(true), GUILayout.Height(20));

      if (!manager.InProcess && IsStale())
      {
        Debug.Log("Cache is stale");
        await manager.SetInput(cache);
      }
    }

    private bool IsStale()
    {
      var value = false;
      if (!cache.Compare(stale))
      {
        value = true;
        stale.account = cache.account;
        stale.stream = cache.stream;
        stale.branch = cache.branch;
        stale.commit = cache.commit;
      }
      return value;
    }
  }

  internal class ManagerCache
  {
    public int account;
    public int stream;
    public int branch;
    public int commit;

    public bool Compare(ManagerCache input)
    {
      return input != null && input.account == account && input.stream == stream && input.branch == branch && input.commit == commit;

    }
  }
}