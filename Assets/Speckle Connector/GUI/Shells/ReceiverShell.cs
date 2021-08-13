using System;

namespace ConnectorUnity
{
  [Serializable]
  public class ReceiverShell
  {
    public string stream, streamId;
    public string branch;
    public string commit;
  }
}