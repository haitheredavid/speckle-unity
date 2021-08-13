using System.Collections.Generic;
using NUnit.Framework;
using Speckle.ConnectorUnity;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using ViewTo;
using ViewTo.Connector.Unity;
using ViewTo.Objects;
using ViewTo.Objects.Speckle;
using ViewTo.Objects.Speckle.Elements;
using Random = System.Random;

namespace ViewToUnity.Tests.Units
{

  public class RigCreationTests
  {

    [TestCase(true)]
    [TestCase(false)]
    public void To_RigObj(bool isValid)
    {
      var s = isValid ? TestMil.Study : new ViewStudy();
      var o = new RigObj();

      s.LoadStudyToRig(ref o);
      Assert.IsTrue(o.CanRun() == isValid);

      var mono = o.ToUnity();
      Assert.NotNull(mono);
    }

    public static List<CloudPointBase> CloudPoints(int count)
    {
      var rnd = new Random();
      var points = new List<CloudPointBase>();
      for (var i = 0; i < count; i++)
        points.Add(new CloudPointBase
        {
          x = rnd.NextDouble(), y = rnd.NextDouble(), z = rnd.NextDouble(), meta = "FunSpace"
        });

      return points;
    }

    [Test]
    public void Base_To_Native_To_Mono()
    {
      var @object = new ViewCloudBase
      {
        points = CloudPoints(100)
      };

      var s = Operations.Serialize(@object);
      var @base = Operations.Deserialize(s);

      Assert.NotNull(@base);

      var converter = new ViewObjMonoConverter();

      var mono = converter.ConvertRecursivelyToUnity(@base);

      Assert.NotNull(mono);
      Assert.NotNull(mono.GetComponent<ViewCloudMono>());
    }

  }

}