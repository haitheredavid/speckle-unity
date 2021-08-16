using System.Collections.Generic;
using System.Linq;
using ConnectorUnity;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using ViewTo.Connector.Unity;
using ViewTo.Objects.Speckle;
using ViewTo.Objects.Speckle.Contents;

namespace ViewTo.Objects.Converter.Unity
{
  public class ViewObjUnityConverter : ViewObjConverter
  {

    public ViewObjUnityConverter()
    {
      defaultConverter = new ConverterUnity();
    }

    public override string Name => nameof(ViewObjUnityConverter);

    public override string Description => "Converter for unity objects into base view objects";

    public override IEnumerable<string> GetServicedApplications() => new[] {Applications.Unity};

    private ISpeckleConverter defaultConverter { get; }

    public override object ConvertToNative(Base @base)
    {
      var obj = base.ConvertToNative(@base) as ViewObj;
      return obj?.ConvertToViewMono();
    }
    
    
    
    protected override TObj ViewContentToNative<TObj>(ViewContentBase @base)
    {
      var content = base.ViewContentToNative<TObj>(@base);
      if (content == null || defaultConverter == null) return content;

      content.objects = @base.meshes.Select(o => defaultConverter.ConvertToNative(o)).Where(o => o != null).ToList();
      return content;
    }

  }

}