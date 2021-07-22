using System.Collections.Generic;
using UnityEngine;
using ViewTo.Structure;

namespace ViewTo.Connector.Unity {
    public static partial class ViewConverter {
        
        public static Texture2D DrawPixelLine( this List<Color32> c, bool readAlpha = false )
            {
                Texture2D tempTexture = new Texture2D( c.Count, 1 );

                for ( int x = 0; x < tempTexture.width; x++ ) {
                    Color32 temp = !readAlpha ? new Color32( c[ x ].r, c[ x ].g, c[ x ].b, 255 ) : new Color32( c[ x ].r, c[ x ].g, c[ x ].b, c[ x ].a );

                    tempTexture.SetPixel( x, 0, temp );
                }
                tempTexture.Apply( );
                return tempTexture;
            }


        public static ViewColor ToNative( this Color32 value, int index )
            {
                return new ViewColor( value.r, value.g, value.b, value.a, index );
            }
        
        public static Color32 ToUnity( this ViewColor value )
            {
                return new Color32( value.R, value.G, value.B, value.A );
            }
    }
}