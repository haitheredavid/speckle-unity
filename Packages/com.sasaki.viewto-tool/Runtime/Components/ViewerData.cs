using System;
using UnityEngine;

namespace ViewToUnity
{
  public class ViewerData : MonoBehaviour
  {

    [SerializeField] private uint[] sampleValues;
    public uint[][] LocalData { get; set; }
    public uint[] GetSamples => sampleValues;

    public void Create(uint keyLength) =>
      // Debug.Log( $"Creating Collection {keyLength}" );
      LocalData = new uint[keyLength][];
    public void Clear()
    {
      Debug.Log("Clearing Collection");
      LocalData = null;
    }

    public void Collect(uint[] data, uint colorCount, uint pointIndex)
    {
      if (LocalData != null)
      {

        var dataCount = (uint)data.Length;
        var joinedValues = new uint[colorCount];

        for (uint i = 0; i < dataCount; i++)
        {
          var index = i % colorCount;
          joinedValues[index] += data[i];
        }

        // store locally
        LocalData[pointIndex] = joinedValues;

        sampleValues = new uint[joinedValues.Length];
        Array.Copy(joinedValues, sampleValues, joinedValues.Length);
      }
      else
      {
        Debug.LogWarning("Collection is not created yet!");
      }
    }
  }
}