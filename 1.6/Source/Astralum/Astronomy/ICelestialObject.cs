using UnityEngine;

namespace Astralum.Astronomy
{
  public interface ICelestialObject
  {
    public string Id { get; }
    public float RenderSize { get; }
    public float Rotation { get; }
    public Vector3 LocalSkyPosition { get; }
    //public Vector3 WorldViewDirection { get; }
  }
}