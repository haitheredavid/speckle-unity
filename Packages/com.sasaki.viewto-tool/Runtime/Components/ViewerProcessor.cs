using System;
using UnityEngine;


namespace ViewTo.Connector.Unity
{
  public class ViewerProcessor : MonoBehaviour
  {

    public ComputeShader pixelShader;

    [Header("|| Runtime Creations ||")]
    [SerializeField] private Texture2D viewTexture;
    [SerializeField] private int colorCount;
    [SerializeField] private uint[] histogramData;
    private bool _counterReady;

    private ComputeBuffer _histogramBuffer;

    private void OnEnable() => _histogramBuffer?.Dispose();

    private void OnDisable() => _histogramBuffer?.Dispose();

    public event Action<uint[]> Complete;

    public void Initialize(Texture2D colors)
    {
      pixelShader = Instantiate(Resources.Load<ComputeShader>("PixelFinder"));

      if (viewTexture != null)
      {
        Debug.Log("Destroying view texture");
        Destroy(viewTexture);
      }

      if (colors != null && colors.width > 0)
      {

        viewTexture = colors;
        colorCount = colors.width;
        SetKernels();
      }
      else
      {
        Debug.LogError($"Could not create texture for {gameObject.name} ");
      }
    }

    private void CreateBuffers()
    {
      // Debug.Log( "Creating new buffer" );
      _histogramBuffer?.Release();

      _histogramBuffer = new ComputeBuffer(colorCount, sizeof(uint));
      histogramData = new uint[colorCount];

      pixelShader.SetInt(ColorArraySize, colorCount);

    }

    private void SetKernels()
    {
      // Debug.Log( $"Setting Kernel for {gameObject.name}" );

      if (viewTexture == null)
      {
        Debug.LogError($"texture on {this} is not ready yet");
      }
      else
      {
        CreateBuffers();

        _kernInitialize = pixelShader.FindKernel(PixelFinderInit);
        pixelShader.SetBuffer(_kernInitialize, PixelCountBuffer, _histogramBuffer);

        _kernMain = pixelShader.FindKernel(PixelFinder);
        pixelShader.SetBuffer(_kernMain, PixelCountBuffer, _histogramBuffer);
        pixelShader.SetTexture(_kernMain, ColorArrayTexture, viewTexture);

        pixelShader.SetInt(InputTextureSize, 512);

        if (_kernInitialize < 0 || _kernMain < 0 || null == _histogramBuffer || null == histogramData)
          _counterReady = false;
        else
          _counterReady = true;
      }
    }

    public void Process(RenderTexture source)
    {
      if (_counterReady && _histogramBuffer != null)
      {
        if (_histogramBuffer.count != colorCount)
        {
          Debug.Log("Updating Buffer");
          CreateBuffers();
        }

        pixelShader.SetBuffer(_kernInitialize, PixelCountBuffer, _histogramBuffer);
        pixelShader.SetBuffer(_kernMain, PixelCountBuffer, _histogramBuffer);
        pixelShader.SetTexture(_kernMain, InputTexture, source);

        // var buf = source.depthBuffer;
        pixelShader.Dispatch(_kernInitialize, 256 / 64, 1, 1);
        pixelShader.Dispatch(_kernMain, (source.width + 7) / 8, (source.height + 7) / 8, 1);

        // NOTE performance impact 
        if (IsRunning)
        {
          _histogramBuffer.GetData(histogramData);
          Complete?.Invoke(histogramData);
        }
      }
    }

    #region Shader Parameters
    private const string PixelFinderInit = "PixelFinderInitialize";
    private const string PixelFinder = "PixelFinderMain";

    private const string InputTexture = "InputTexture";
    private const string InputTextureSize = "InputTextureSize";

    private const string ColorArraySize = "ColorArraySize";
    private const string ColorArrayTexture = "ColorArrayTexture";

    private const string PixelCountBuffer = "PixelCountBuffer";

    private int _kernMain;
    private int _kernInitialize;
    public bool IsRunning { get; set; } = false;
    #endregion

  }
}