using System.Collections.Generic;
using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Elements;
using ViewTo.Objects.Structure;

namespace ViewToUnity.Tests
{
  public static class TestMil
  {

    public static RigParameters RigParams
    {
      get =>
        new RigParameters
        {
          bundles = ViewerBundle()
        };
    }

    public static double RV
    {
      get => Random.Range(0, 100);
    }
    public static ViewStudy Study
    {
      get
      {
        var s = new ViewStudy
        {
          viewName = "StudyCloudTest"
        };

        var cloud1 = Cloud(1000);
        var cloud2 = Cloud(100);

        var bundle1 = new ViewerBundle
        {
          layouts = new List<ViewerLayout>
          {
            new ViewerLayout(), new ViewerLayoutCube()
          }
        };

        var bundle2 = new ViewerBundleLinked
        {
          layouts = new List<ViewerLayout>
          {
            new ViewerLayoutCube(), new ViewerLayoutFocus()
          },

          linkedClouds = new List<MetaShell>
          {
            Shell(cloud2)
          }
        };

        var target1 = new TargetContent
        {
          viewName = "GlobalFunSpot"
        };

        var target2 = new TargetContent
        {
          viewName = "IsolatedTarget",
          isolate = true,
          bundles = new List<ViewerBundle>
          {
            bundle2
          }
        };


        var content = new ContentBundle
        {
          targets = new List<TargetContent>
          {
            target1, target2
          },
          blockers = new List<BlockerContent>
          {
            new BlockerContent(), new BlockerContent()
          },
          designs = new List<DesignContent>
          {
            new DesignContent
            {
              viewName = "Design1"
            },
            new DesignContent
            {
              viewName = "Design2"
            }
          }
        };

        var rigParams = new RigParameters
        {
          bundles = new List<ViewerBundle>
          {
            bundle1, bundle2
          }
        };

        s.objs = new List<ViewObj>
        {
          cloud1, cloud2, content, rigParams
        };

        return s;
      }
    }

    public static RigParametersIsolated RigParamsIso(ViewCloud c) => new RigParametersIsolated
    {
      bundles = ViewerBundle(c), colors = new List<ViewColor>
        {new ViewColor(255, 255, 255, 255, 0)}
    };

    public static TargetContent TC(bool isolate) => new TargetContent
    {
      viewName = "TestName",
      isolate = isolate,
      bundles = isolate ? ViewerBundle(Cloud(100)) : ViewerBundle()
    };
    public static DesignContent DC() => new DesignContent {viewName = "TestName"};

    public static ViewCloud Cloud(int count)
    {
      var pts = new CloudPoint[count];
      for (var i = 0; i < pts.Length; i++)
        pts[i] = new CloudPoint(RV, RV, RV) {meta = "Floor1"};

      return new ViewCloud
        {points = pts};
    }

    public static MetaShell Shell(ViewCloud c) => new MetaShell(c, c.viewID, c.points.Length);

    public static List<ViewerBundle> ViewerBundle(ViewCloud c) => new List<ViewerBundle>
    {
      new ViewerBundleLinked
      {
        linkedClouds = new List<MetaShell>
        {
          Shell(c)
        },
        layouts = new List<ViewerLayout>
        {
          new ViewerLayoutFocus()
        }
      }
    };
    public static List<ViewerBundle> ViewerBundle() => new List<ViewerBundle>
    {
      new ViewerBundle
      {
        layouts = new List<ViewerLayout>
        {
          new ViewerLayout(), new ViewerLayoutCube()
        }
      }
    };
  }

}