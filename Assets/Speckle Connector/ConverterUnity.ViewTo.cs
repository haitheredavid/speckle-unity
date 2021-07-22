using Objects.ViewTo;
using ViewTo;
using ViewTo.Objects;

namespace Objects.Converter.Unity
{
  public partial class ConverterUnity
  {

    public void Item()
    {
      var @base = new ViewStudyBase();
      var @object = new ViewStudy();

      var rig = new RigObj();
      @object.LoadStudyToRig(ref rig);
    }
    
  }
}