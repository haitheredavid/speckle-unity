using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

    public struct BuilderDataInput
    {
        public readonly Component unityObj;
        public readonly Base speckleObj;

        public BuilderDataInput(Base speckleObj, Component unityObj)
        {
            this.speckleObj = speckleObj;
            this.unityObj = unityObj;
        }

    }

}
