namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

    public class BaseNode
    {
        public BaseNode(BaseNode left, BaseNode right, int key)
        {
            this.left = left;
            this.right = right;
            this.key = key;
        }

        public readonly int key;

        public readonly BaseNode left;

        public readonly BaseNode right;

    }


    }
