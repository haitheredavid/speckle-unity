using System.Collections.Generic;
using ConnectorUnity;
using Speckle.Core.Kits;
using ViewTo.Connector.Unity;
using ViewTo.Objects.Speckle;
using ViewTo.Objects.Speckle.Contents;

namespace ViewTo.Objects.Converter.Unity
{
  public partial class ViewObjUnityConverter : ViewObjConverter
  {

    public ViewObjUnityConverter()
    {
      defaultConverter = new ConverterUnity();
    }

    public override string Name => nameof(ViewObjUnityConverter);

    public override string Description => "Converter for unity objects into base view objects";

    public override IEnumerable<string> GetServicedApplications() => new[] {Applications.Unity};

    private ConverterUnity defaultConverter { get; }

    protected override object StudyToNative(ViewStudyBase @base)
    {
      var study = Create<ViewStudyMono>();
      study.ViewName = @base.viewName;

      var imported = new List<ViewObjBehaviour>();
      // load all objects
      foreach (var obj in @base.objs)
      {
        if (obj == null) continue;

        // first convert to view obj
        var mono = ConvertToNative(obj) as ViewObjBehaviour;
        if (mono == null) continue;

        // add to study 
        mono.transform.SetParent(study.transform);
        imported.Add(mono);
      }
      study.ViewObjs = imported;
      return study;
    }
    protected override object ViewCloudToNative(ViewCloudBase @base)
    {
      var viewObj = base.ViewCloudToNative(@base) as ViewCloud;
      return viewObj.SetMono();
    }

    protected override object ContentBundleToNative(ContentBundleBase @base)
    {
      // build mono obj
      var mono = Create<ContentBundleMono>();
      mono.name = "ContentBundle";

      var items = new List<ViewContentBase>();
      items.AddRange(@base.targets);
      items.AddRange(@base.blockers);
      items.AddRange(@base.designs);

      foreach (var i in items)
        if (ViewContentToNative(i) is ViewContentMono content)
          mono.Set(content);

      return mono;
    }

    protected override object ViewContentToNative(ViewContentBase @base)
    {
      var content = base.ViewContentToNative(@base) as ViewContent;
      if (content == null || defaultConverter == null) return content;

      content.objects = new List<object>();
      foreach (var m in @base.meshes)
      {
        if (m == null) continue;

        content.objects.Add(defaultConverter.ConvertToNative(m));
      }
      return content.SetMono();
    }
  }

}