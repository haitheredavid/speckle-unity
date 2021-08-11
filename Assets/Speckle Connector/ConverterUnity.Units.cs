namespace Objects.Converter.Unity
{
  public partial class ConverterUnity
  {
    public string ModelUnits = Speckle.Core.Kits.Units.Meters; //the default Unity units are meters
    private double ScaleToNative(double value, string units)
    {
      var f = Speckle.Core.Kits.Units.GetConversionFactor(units, ModelUnits);
      return value * f;
    }


  }
}
