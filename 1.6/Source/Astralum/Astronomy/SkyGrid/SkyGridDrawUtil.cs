using UnityEngine;
using Verse;

namespace Astralum.Astronomy.SkyGrid;

public static class SkyGridDrawUtil
{
  public const float DistanceToGrid = 20f;
  public const int Segments = 192;

  public static void PrintGreatCircle(LayerSubMesh subMesh, Vector3 normal, float width)
  {
    GetCircleBasis(normal, out Vector3 a, out Vector3 b);

    Vector3 previous = DirectionOnCircle(a, b, 0f);

    for (int i = 1; i <= Segments; i++)
    {
      float angle = i / (float)Segments * Mathf.PI * 2f;
      Vector3 current = DirectionOnCircle(a, b, angle);

      PrintLineSegment(
        subMesh,
        previous * DistanceToGrid,
        current * DistanceToGrid,
        width
      );

      previous = current;
    }
  }

  public static void PrintLatitudeCircle(LayerSubMesh subMesh, Vector3 pole,
    float signedHeight, float width)
  {
    pole.Normalize();

    float height = Mathf.Clamp(signedHeight, -0.99f, 0.99f);
    float radius = Mathf.Sqrt(1f - height * height);

    GetCircleBasis(pole, out Vector3 a, out Vector3 b);

    Vector3 previous = (pole * height + DirectionOnCircle(a, b, 0f) * radius).normalized;

    for (int i = 1; i <= Segments; i++)
    {
      float angle = i / (float)Segments * Mathf.PI * 2f;
      Vector3 current = (pole * height + DirectionOnCircle(a, b, angle) * radius).normalized;

      PrintLineSegment(
        subMesh,
        previous * DistanceToGrid,
        current * DistanceToGrid,
        width
      );

      previous = current;
    }
  }

  public static void PrintEquatorTicks(LayerSubMesh subMesh, Vector3 pole, int tickCount,
    float tickLength, float width)
  {
    GetCircleBasis(pole, out Vector3 a, out Vector3 b);

    for (int i = 0; i < tickCount; i++)
    {
      float angle = i / (float)tickCount * Mathf.PI * 2f;

      Vector3 equatorDir = DirectionOnCircle(a, b, angle);
      Vector3 tickDir = pole.normalized;

      Vector3 start = (equatorDir - tickDir * tickLength * 0.5f).normalized * DistanceToGrid;
      Vector3 end = (equatorDir + tickDir * tickLength * 0.5f).normalized * DistanceToGrid;

      PrintLineSegment(subMesh, start, end, width);
    }
  }

  public static void PrintMeridian(LayerSubMesh subMesh, Vector3 pole,
    Vector3 referenceDirection, float width)
  {
    Vector3 equatorDir = Vector3.ProjectOnPlane(referenceDirection, pole);

    if (equatorDir.sqrMagnitude < 0.001f)
      equatorDir = Vector3.ProjectOnPlane(Vector3.up, pole);

    if (equatorDir.sqrMagnitude < 0.001f)
      equatorDir = Vector3.ProjectOnPlane(Vector3.right, pole);

    equatorDir.Normalize();

    Vector3 meridianNormal = Vector3.Cross(pole, equatorDir).normalized;

    PrintGreatCircle(subMesh, meridianNormal, width);
  }

  private static void GetCircleBasis(Vector3 normal, out Vector3 a, out Vector3 b)
  {
    normal.Normalize();

    a = Vector3.Cross(normal, Vector3.up);

    if (a.sqrMagnitude < 0.001f)
      a = Vector3.Cross(normal, Vector3.right);

    a.Normalize();
    b = Vector3.Cross(normal, a).normalized;
  }

  private static Vector3 DirectionOnCircle(Vector3 a, Vector3 b, float angle)
  {
    return (Mathf.Cos(angle) * a + Mathf.Sin(angle) * b).normalized;
  }

  private static void PrintLineSegment(LayerSubMesh subMesh, Vector3 start,
    Vector3 end, float width)
  {
    Vector3 mid = (start + end) * 0.5f;
    Vector3 normal = mid.normalized;
    Vector3 dir = (end - start).normalized;
    Vector3 side = Vector3.Cross(normal, dir).normalized;

    float halfWidth = width * 0.5f;

    Vector3 v0 = start - side * halfWidth;
    Vector3 v1 = start + side * halfWidth;
    Vector3 v2 = end + side * halfWidth;
    Vector3 v3 = end - side * halfWidth;

    int baseIndex = subMesh.verts.Count;

    subMesh.verts.Add(v0);
    subMesh.verts.Add(v1);
    subMesh.verts.Add(v2);
    subMesh.verts.Add(v3);

    subMesh.uvs.Add(new Vector2(0f, 0f));
    subMesh.uvs.Add(new Vector2(0f, 1f));
    subMesh.uvs.Add(new Vector2(1f, 1f));
    subMesh.uvs.Add(new Vector2(1f, 0f));

    subMesh.tris.Add(baseIndex + 0);
    subMesh.tris.Add(baseIndex + 1);
    subMesh.tris.Add(baseIndex + 2);

    subMesh.tris.Add(baseIndex + 0);
    subMesh.tris.Add(baseIndex + 2);
    subMesh.tris.Add(baseIndex + 3);
  }
}