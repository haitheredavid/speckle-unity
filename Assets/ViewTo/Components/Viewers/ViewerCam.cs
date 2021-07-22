using UnityEngine;

namespace ViewTo.Connector.Unity {
    public class ViewCamera : MonoBehaviour {

        [SerializeField] [Range( 1, 16 )] private int depthBuffer = 16;
        [SerializeField] private Camera cam;

        // public ViewProcessor processor;

        private const int ViewSize = 512;

        public RenderTexture RenderText { get; set; }

        public bool AnalysisComplete { get; set; }
        public bool ViewerRunning { get; set; }

        public int CamDistance {
            set {
                if ( cam != null )
                    cam.farClipPlane = value;
            }
        }

        public Color CameraBackground {
            set {
                if ( cam != null )
                    cam.backgroundColor = value;
            }
        }

        private void Awake( )
            {
                cam = gameObject.GetComponent<Camera>( );
                if ( cam == null ) {
                    cam = gameObject.AddComponent<Camera>( );
                }

                RenderText = RenderTexture.GetTemporary( ViewSize, ViewSize, depthBuffer );
                RenderText.name = "CameraTexture";
                cam.clearFlags = CameraClearFlags.Color;
                cam.fieldOfView = 90f;
                cam.backgroundColor = Color.black;
            }

        public float OrthoSize {
            set {
                if ( cam != null ) {
                    if ( !cam.orthographic )
                        cam.orthographic = true;

                    cam.orthographicSize = value;
                }
            }
        }

        // public RigStage SetLayerMask {
        //     set {
        //         cam.cullingMask = value switch {
        //             RigStage.Target => 1 << ViewHelper.TargetLayer,
        //             RigStage.Blocker => 1 << ViewHelper.TargetLayer | 1 << ViewHelper.BlockerLayer,
        //             RigStage.Design => 1 << ViewHelper.TargetLayer | 1 << ViewHelper.BlockerLayer | 1 << ViewHelper.DesignLayer,
        //             _ => -1
        //         };
        //     }
        // }

        private void SafeClean( )
            {
                if ( RenderText != null ) RenderText.Release( );
            }

        private void OnDestroy( )
            {
                SafeClean( );
            }

        private void OnDisable( )
            {
                SafeClean( );
            }

        private void OnPreRender( )
            {
                if ( cam != null ) {
                    cam.targetTexture = RenderText;
                }
            }

        // NOTE this will only call when a camera is displayed
        private void OnPostRender( )
            {
                // if ( ViewerRunning && !AnalysisComplete && cam != null && RenderText != null && processor != null ) {
                //     processor.Process( RenderText );
                // }
            }

    }

}