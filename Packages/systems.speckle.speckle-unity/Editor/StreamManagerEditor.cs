// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Sentry;
// using Speckle.Core.Api;
// using Speckle.Core.Credentials;
// using Speckle.Core.Kits;
// using Speckle.Core.Logging;
// using Speckle.Core.Transports;
// using UnityEditor;
// using UnityEngine;
//
// namespace Speckle.ConnectorUnity
// {
//   [CustomEditor(typeof(SpeckleConnector))]
//   [CanEditMultipleObjects]
//   public class StreamManagerEditor : Editor
//   {
//     private bool _foldOutAccount;
//     private SpeckleConnector _speckleConnector;
//     private int _totalChildrenCount;
//
//     public int StreamsLimit { get; set; } = 30;
//     public int BranchesLimit { get; set; } = 30;
//     public int CommitsLimit { get; set; } = 25;
//
//     private int SelectedAccountIndex
//     {
//       get => _speckleConnector.SelectedAccountIndex;
//       set => _speckleConnector.SelectedAccountIndex = value;
//     }
//
//     private int SelectedStreamIndex
//     {
//       get => _speckleConnector.SelectedStreamIndex;
//       set => _speckleConnector.SelectedStreamIndex = value;
//     }
//
//     private int SelectedBranchIndex
//     {
//       get => _speckleConnector.SelectedBranchIndex;
//       set => _speckleConnector.SelectedBranchIndex = value;
//     }
//
//     private int SelectedCommitIndex
//     {
//       get => _speckleConnector.SelectedCommitIndex;
//       set => _speckleConnector.SelectedCommitIndex = value;
//     }
//
//     private int OldSelectedAccountIndex
//     {
//       get => _speckleConnector.OldSelectedAccountIndex;
//       set => _speckleConnector.OldSelectedAccountIndex = value;
//     }
//
//     private int OldSelectedStreamIndex
//     {
//       get => _speckleConnector.OldSelectedStreamIndex;
//       set => _speckleConnector.OldSelectedStreamIndex = value;
//     }
//
//     private Client Client
//     {
//       get => _speckleConnector.Client;
//       set => _speckleConnector.Client = value;
//     }
//
//     private Account SelectedAccount
//     {
//       get => _speckleConnector.SelectedAccount;
//       set => _speckleConnector.SelectedAccount = value;
//     }
//
//     private Stream SelectedStream
//     {
//       get => _speckleConnector.SelectedStream;
//       set => _speckleConnector.SelectedStream = value;
//     }
//
//     public List<Account> Accounts
//     {
//       get => _speckleConnector.Accounts;
//       set => _speckleConnector.Accounts = value;
//     }
//
//     private List<Stream> Streams
//     {
//       get => _speckleConnector.Streams;
//       set => _speckleConnector.Streams = value;
//     }
//
//     private List<Branch> Branches
//     {
//       get => _speckleConnector.Branches;
//
//       set => _speckleConnector.Branches = value;
//     }
//
//     private async Task LoadAccounts()
//     {
//       //refresh accounts just in case
//       Accounts = AccountManager.GetAccounts().ToList();
//       if (!Accounts.Any())
//         Debug.Log("No Accounts found, please login in Manager");
//       else
//         await SelectAccount(0);
//     }
//
//     private async Task SelectAccount(int i)
//     {
//       SelectedAccountIndex = i;
//       OldSelectedAccountIndex = i;
//       SelectedAccount = Accounts[i];
//
//       Client = new Client(SelectedAccount);
//       await LoadStreams();
//     }
//
//     private async Task LoadStreams()
//     {
//       EditorUtility.DisplayProgressBar("Loading streams...", "", 0);
//       Streams = await Client.StreamsGet(StreamsLimit);
//       EditorUtility.ClearProgressBar();
//       if (Streams.Any())
//         await SelectStream(0);
//     }
//
//     private async Task SelectStream(int i)
//     {
//       SelectedStreamIndex = i;
//       OldSelectedStreamIndex = i;
//       SelectedStream = Streams[i];
//
//       EditorUtility.DisplayProgressBar("Loading stream details...", "", 0);
//       Branches = await Client.StreamGetBranches(SelectedStream.id, BranchesLimit, CommitsLimit);
//       if (Branches.Any())
//       {
//         SelectedBranchIndex = 0;
//         if (Branches[SelectedBranchIndex].commits.items.Any()) SelectedCommitIndex = 0;
//       }
//
//       EditorUtility.ClearProgressBar();
//     }
//
//     private async Task Receive()
//     {
//       EditorUtility.DisplayProgressBar("Receiving data...", "", 0);
//
//       try
//       {
//         //TODO: Replace with new tracker stuff
//         // Tracker.TrackPageview(Tracker.RECEIVE);
//
//         var transport = new ServerTransport(SelectedAccount, SelectedStream.id);
//         var @base = await Operations.Receive(
//           Branches[SelectedBranchIndex].commits.items[SelectedCommitIndex].referencedObject,
//           transport,
//           onProgressAction: dict =>
//           {
//             EditorApplication.delayCall += () =>
//             {
//               EditorUtility.DisplayProgressBar("Receiving data...", "",
//                                                Convert.ToSingle(dict.Values.Average() / _totalChildrenCount));
//             };
//           },
//           onTotalChildrenCountKnown: count => { _totalChildrenCount = count; }
//         );
//
//         var go = _speckleConnector.ConvertRecursivelyToNative(@base,
//                                                            Branches[SelectedBranchIndex].commits.items[SelectedCommitIndex].id);
//
//         try
//         {
//           await Client.CommitReceived(new CommitReceivedInput
//           {
//             streamId = SelectedStream.id,
//             commitId = Branches[SelectedBranchIndex].commits.items[SelectedCommitIndex].id,
//             message = $"received commit from {Applications.Unity} Editor",
//             sourceApplication = Applications.Unity
//           });
//         }
//         catch
//         {
//           // Do nothing!
//         }
//
//       }
//       catch (Exception e)
//       {
//         EditorApplication.delayCall += () => { EditorUtility.ClearProgressBar(); };
//         throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
//       }
//
//       EditorApplication.delayCall += () => { EditorUtility.ClearProgressBar(); };
//     }
//
//     public override async void OnInspectorGUI()
//     {
//       _speckleConnector = (SpeckleConnector)target;
//
//
//       #region Account GUI
//       if (Accounts == null)
//       {
//         await LoadAccounts();
//         return;
//       }
//
//
//       EditorGUILayout.BeginHorizontal();
//
//       SelectedAccountIndex = EditorGUILayout.Popup("Accounts", SelectedAccountIndex,
//                                                    Accounts.Select(x => x.userInfo.email + " | " + x.serverInfo.name).ToArray(),
//                                                    GUILayout.ExpandWidth(true), GUILayout.Height(20));
//
//       if (OldSelectedAccountIndex != SelectedAccountIndex)
//       {
//         await SelectAccount(SelectedAccountIndex);
//         return;
//       }
//
//       if (GUILayout.Button("Refresh", GUILayout.Width(60), GUILayout.Height(20)))
//       {
//         await LoadAccounts();
//         return;
//       }
//
//       EditorGUILayout.EndHorizontal();
//
//
//       #region Speckle Account Info
//       _foldOutAccount = EditorGUILayout.BeginFoldoutHeaderGroup(_foldOutAccount, "Account Info");
//
//       if (_foldOutAccount)
//       {
//         EditorGUI.BeginDisabledGroup(true);
//
//         EditorGUILayout.TextField("Name", SelectedAccount.userInfo.name,
//                                   GUILayout.Height(20),
//                                   GUILayout.ExpandWidth(true));
//
//         EditorGUILayout.TextField("Server", SelectedAccount.serverInfo.name,
//                                   GUILayout.Height(20),
//                                   GUILayout.ExpandWidth(true));
//
//         EditorGUILayout.TextField("URL", SelectedAccount.serverInfo.url,
//                                   GUILayout.Height(20),
//                                   GUILayout.ExpandWidth(true));
//
//         EditorGUI.EndDisabledGroup();
//       }
//
//       EditorGUILayout.EndFoldoutHeaderGroup();
//       #endregion
//       #endregion
//
//       #region Stream List
//       if (Streams == null)
//         return;
//
//       EditorGUILayout.BeginHorizontal();
//
//       SelectedStreamIndex = EditorGUILayout.Popup("Streams",
//                                                   SelectedStreamIndex, Streams.Select(x => x.name).ToArray(), GUILayout.Height(20),
//                                                   GUILayout.ExpandWidth(true));
//
//       if (OldSelectedStreamIndex != SelectedStreamIndex)
//       {
//         await SelectStream(SelectedStreamIndex);
//         return;
//       }
//
//       if (GUILayout.Button("Refresh", GUILayout.Width(60), GUILayout.Height(20)))
//       {
//         await LoadStreams();
//         return;
//       }
//
//       EditorGUILayout.EndHorizontal();
//       #endregion
//
//       #region Branch List
//       if (Branches == null)
//         return;
//
//       EditorGUILayout.BeginHorizontal();
//
//       SelectedBranchIndex = EditorGUILayout.Popup("Branches",
//                                                   SelectedBranchIndex, Branches.Select(x => x.name).ToArray(), GUILayout.Height(20),
//                                                   GUILayout.ExpandWidth(true));
//       EditorGUILayout.EndHorizontal();
//
//
//       if (!Branches[SelectedBranchIndex].commits.items.Any())
//         return;
//
//
//       EditorGUILayout.BeginHorizontal();
//
//       SelectedCommitIndex = EditorGUILayout.Popup("Commits",
//                                                   SelectedCommitIndex,
//                                                   Branches[SelectedBranchIndex].commits.items.Select(x => x.message).ToArray(),
//                                                   GUILayout.Height(20),
//                                                   GUILayout.ExpandWidth(true));
//
//       EditorGUILayout.EndHorizontal();
//       #endregion
//
//       #region Generate Materials
//       EditorGUILayout.BeginHorizontal();
//
//       GUILayout.Label("Generate material assets");
//       GUILayout.FlexibleSpace();
//       SpeckleConnector.GenerateMaterials = GUILayout.Toggle(SpeckleConnector.GenerateMaterials, "");
//
//       EditorGUILayout.EndHorizontal();
//       #endregion
//
//
//       EditorGUILayout.BeginHorizontal();
//
//       var receive = GUILayout.Button("Receive!");
//
//       EditorGUILayout.EndHorizontal();
//
//       if (receive) await Receive();
//
//
//     }
//   }
// }