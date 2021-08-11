using UnityEngine;

namespace ViewToUnity
{
  public class RigSpinner : MonoBehaviour
  {

    [SerializeField] private float speed = 20f;
    public bool Spin { get; set; }

    private void Update()
    {
      if (Spin)
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
  }
}