using System.Collections.Generic;
using Objects.Converter.Unity;
using Objects.Geometry;
using Speckle.Core.Models;
using UnityEngine;
using ViewTo.Objects.Speckle;
using ViewTo.Objects.Structure;
using ViewTo.Connector.Unity;
using ViewTo.Objects;

namespace Speckle.ConnectorUnity
{

  public partial class ViewObjBaseMonoConverter : BaseMonoConverter<ViewToKit>
  {
    private ViewObjConverterScript _converter;
    private ConverterUnity _basicConverter;

    public GameObject ConvertRecursivelyToUnity(Base @base)
    {
      _converter ??= LoadConverter<ViewObjConverterScript>(app);
      if (_converter != null)
      {
        Debug.Log($"Trying to base to {@base.TypeName()}");
        if (@base is ViewStudyBase study)
        {
          
          var hackyMesh = new List<GameObject>();
          var hackyBase = new List<Base>();
          var hackyContent = new List<object>();
          Debug.Log("Doing a hacky load for study");
          foreach (var obj in study.objs)
          {
            if (obj is ContentBundleBase content)
            {
              Debug.Log("Found Content Bundle, loading");

              _basicConverter = new ConverterUnity();
              var simpleConverter = new SimpleRecursiveConverter();

              for (var i = 0; i < content.targets.Count; i++)
              {
                var item = content.targets[i];
                var contentName = item.TypeName() + " " + i + " " + (item.viewName.Valid() ? item.viewName : "");
                // item.content = new Box();
                var go = simpleConverter.ConvertRecursivelyToNative(item.content, contentName);
                if (go != null)
                {
                  hackyContent.Add(_converter.ConvertToNative(item));
                  hackyBase.Add(item.content);
                  hackyMesh.Add(go);
                }

              }
              for (int i = 0; i < content.blockers.Count; i++)
              { 
                var item = content.blockers[i];
                var contentName = item.TypeName() + i;
                item.content = new Box();
                var go = simpleConverter.ConvertRecursivelyToNative(item.content, contentName);
                if (go != null)
                {
                  hackyContent.Add(_converter.ConvertToNative(item));
                  hackyMesh.Add(go);
                }
              }
              for (int i = 0; i < content.designs.Count; i++)
              {
                var item = content.designs[i];
                var contentName = item.TypeName() + i + (item.viewName.Valid() ? item.viewName : "");
                item.content = new Box();
                var go = simpleConverter.ConvertRecursivelyToNative(item.content, contentName);
                if (go != null)
                {
                  hackyContent.Add(_converter.ConvertToNative(item));
                  hackyMesh.Add(go);
                }
              }
            }

          }

          var viewObj = _converter.ConvertToNative(study);
          if (viewObj != null && viewObj is ViewStudy vs)
          {
            var mono = new GameObject().SetMono<ViewStudyMono>(vs);
            mono.SetHackyContent(hackyContent, hackyMesh);
            return mono.gameObject;
          }
        }
        var o = _converter.ConvertToNative(@base);
        if (o != null)
          return o.ConvertToViewMono().gameObject;
      }
      return null;
    }

  }

}