using System.Collections.Generic;
using Speckle.Core.Models;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

    public interface IWantContextObj
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ApplicationObject> contextObjects
        {
            get;
            set;
        }
    }

}
