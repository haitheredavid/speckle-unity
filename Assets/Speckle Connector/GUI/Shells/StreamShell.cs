using System;

namespace ConnectorUnity
{
  [Serializable]
  public class StreamShell
  {

    [ReadOnly] public string streamName, streamId;
    [ReadOnly] public string branch;
    [ReadOnly] public string commitId;
    [ReadOnly] public int totalChildCount;

    /// <summary>
    /// boolean flag for notifying displaying if active stream has new updates
    /// </summary>
    [ReadOnly] public bool expired;
    /// <summary>
    /// If true, it will automatically receive updates sent to this stream
    /// </summary>
    [ReadOnly] public bool autoReceive;
    /// <summary>
    /// If true, it will delete previously received objects when new one are received.\n
    /// Set to true by default 
    /// </summary>
    [ReadOnly] public bool clearOnUpdate = true;

  }
}