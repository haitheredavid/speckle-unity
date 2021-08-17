using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  public class ContentBundleMono : ViewObjBehaviour<ContentBundle>
  {

    // TODO compile lists added during edit mode  
    [SerializeField] private List<ViewContentMono> contents;

    public List<TargetContent> targets
    {
      get => viewObj.targets;
      private set => viewObj.targets = value;
    }

    public List<BlockerContent> blockers
    {
      get => viewObj.blockers;
      private set => viewObj.blockers = value;
    }

    public List<DesignContent> designs
    {
      get => viewObj.designs;
      private set => viewObj.designs = value;
    }

    public void Set(IEnumerable<ViewContentMono> items)
    {
      foreach (var i in items)
        if (i != null)
          Set(i);
    }

    public void Set(ViewContentMono item)
    {
      contents ??= new List<ViewContentMono>();
      item.transform.SetParent(transform);
      contents.Add(item);
    }

    public List<ViewContentMono> GetAll => contents.Valid() ? contents : new List<ViewContentMono>();
    
    public List<TContent> Get<TContent>() where TContent : ViewContentMono
    {
      var item = new List<TContent>();
      foreach (var i in contents)
        if (i is TContent casted)
          item.Add(casted);

      return item;
    }

    // public override ContentBundle CopyObj()
    // {
    //   return new ContentBundle
    //   {
    //     targets = viewObj.targets, blockers = viewObj.blockers, designs = viewObj.designs
    //   };
    // }

    // protected override void ImportValidObj()
    // {
    //   Purge();
    //
    //   gameObject.name = "ContentBundle";
    //
    //   var objs = new List<ViewContent>();
    //
    //   if (targets.Valid()) objs.AddRange(targets);
    //   if (blockers.Valid()) objs.AddRange(blockers);
    //   if (designs.Valid()) objs.AddRange(designs);
    //
    //
    //   foreach (var obj in objs.Select(item => item.ToUnity()))
    //   {
    //     obj.transform.SetParent(transform);
    //     contents.Add(obj);
    //   }
    // }

    private void Purge()
    {
      if (contents.Valid())
        for (var i = contents.Count - 1; i >= 0; i--)
        {
          if (Application.isPlaying)
            Destroy(contents[i].gameObject);
          else
            DestroyImmediate(contents[i].gameObject);
        }

      contents = new List<ViewContentMono>();

    }

  }
}