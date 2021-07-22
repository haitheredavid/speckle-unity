using ViewTo.Objects;

namespace ViewTo.Connector.Unity {
  public static partial class ViewHelper {


    public static int MaskByType( this ViewContent t )
      {
        return t switch {
          DesignContent _ => 6,
          TargetContent _ => 7,
          BlockerContent _ => 8,
          _ => 0
        };
      }
    
    


    public static int MaskByType<T>( ) where T : ViewContent
      {
        return typeof( T ) switch {
          var c when c == typeof( DesignContent ) => 6,
          var c when c == typeof( TargetContentMono ) => 7,
          var c when c == typeof( BlockerContentMono ) => 8,
          _ => 0
        };

      }

  }
}