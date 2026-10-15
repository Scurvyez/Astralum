using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Auroras
{
  public static class AuroraMeshGenerator
  {
    // final mesh resolution
    private const int SegmentsPerTurn = 128;
    private const float Turns = 8f;
    private const int Segments = (int)(Turns * SegmentsPerTurn);

    // structural spline
    private const int ControlPointsPerTurn = 10;
    private const int ControlPointCount = (int)(Turns * ControlPointsPerTurn) + 1;

    // maximum structural deformation of each control point
    private const float RadialFoldStrength = 4.5f;
    private const float TangentialFoldStrength = 6.5f;

    // base spiral
    private const float InnerRadius = 8f;
    private const float OuterRadius = 32f;

    // curtain
    private const float BaseAltitude = 1.25f;
    private const float Height = 6.5f;
    private const float TopRadiusScale = 1.05f;

    // centripetal Catmull-Rom
    private const float CatmullRomAlpha = 0.5f;
    private const float MinimumKnotDistance = 0.0001f;

    public static void PrintAurora(LayerSubMesh subMesh, float planetRadius, Vector3 poleDirection, int seed)
    {
      poleDirection.Normalize();
      BuildBasis(poleDirection, out Vector3 tangent, out Vector3 bitangent);
      List<Vector2> controlPoints = GenerateControlPoints(seed);
      
      PrintSpiralCurtain(subMesh, planetRadius, poleDirection, tangent, bitangent, controlPoints);
    }

    private static List<Vector2> GenerateControlPoints(int seed)
    {
      List<Vector2> points = new(ControlPointCount);
      
      Rand.PushState(seed);
      
      try
      {
        for (int i = 0; i < ControlPointCount; i++)
        {
          float t = i / (float)(ControlPointCount - 1);

          float angle = t * Mathf.PI * 2f * Turns;
          float radius = Mathf.Lerp(InnerRadius, OuterRadius, t);
          float cos = Mathf.Cos(angle);
          float sin = Mathf.Sin(angle);

          // Base Archimedean spiral directions.
          Vector2 radialDirection = new(cos, sin);
          Vector2 tangentDirection = new(-sin, cos);

          // Seeded structural deformation.
          float radialFold = Rand.Range(-RadialFoldStrength, RadialFoldStrength);
          float tangentialFold = Rand.Range(-TangentialFoldStrength, TangentialFoldStrength);

          // Fade deformation near both ends.
          float startFade = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * 8f));
          float endFade = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((1f - t) * 8f));
          float deformationFade = startFade * endFade;

          radialFold *= deformationFade;
          tangentialFold *= deformationFade;

          Vector2 point = radialDirection * radius;
          point += radialDirection * radialFold;
          point += tangentDirection * tangentialFold;

          points.Add(point);
        }
      }
      finally
      {
        Rand.PopState();
      }

      return points;
    }

    private static void PrintSpiralCurtain(LayerSubMesh subMesh, float planetRadius, Vector3 poleDirection,
      Vector3 tangent, Vector3 bitangent, List<Vector2> controlPoints)
    {
      int baseVertex = subMesh.verts.Count;

      for (int i = 0; i <= Segments; i++)
      {
        float t = i / (float)Segments;

        Vector2 spiralPoint = EvaluateSpline(controlPoints, t);
        Vector3 radialOffset = tangent * spiralPoint.x + bitangent * spiralPoint.y;
        Vector3 baseDirection = (poleDirection * planetRadius + radialOffset).normalized;
        Vector3 topDirection = (poleDirection * planetRadius + radialOffset * TopRadiusScale).normalized;

        float baseDistance = planetRadius + BaseAltitude;
        float topDistance = baseDistance + Height;

        Vector3 basePosition = baseDirection * baseDistance;
        Vector3 topPosition = topDirection * topDistance;

        subMesh.verts.Add(basePosition);
        subMesh.uvs.Add(new Vector2(t, 0f));
        
        subMesh.verts.Add(topPosition);
        subMesh.uvs.Add(new Vector2(t, 1f));
      }

      for (int i = 0; i < Segments; i++)
      {
        int current = baseVertex + i * 2;
        int next = current + 2;

        AddQuad(subMesh, current, next, next + 1, current + 1);
        
      }
    }

    private static Vector2 EvaluateSpline(List<Vector2> points, float t)
    {
      int pointCount = points.Count;

      if (pointCount == 0)
        return Vector2.zero;

      if (pointCount == 1)
        return points[0];

      int sectionCount = pointCount - 1;
      float scaledT = Mathf.Clamp01(t) * sectionCount;
      int section = Mathf.Min(Mathf.FloorToInt(scaledT), sectionCount - 1);
      float localT = scaledT - section;

      Vector2 p1 = points[section];
      Vector2 p2 = points[Mathf.Min(section + 1, pointCount - 1)];

      // For the two endpoint neighbors, extrapolate rather
      // than duplicating p1/p2.
      //
      // This gives the spline a meaningful tangent at the
      // beginning and end.
      Vector2 p0 = section > 0
          ? points[section - 1]
          : p1 + (p1 - p2);

      Vector2 p3 = section + 2 < pointCount
          ? points[section + 2]
          : p2 + (p2 - p1);

      return CentripetalCatmullRom(p0, p1, p2, p3, localT);
    }

    private static Vector2 CentripetalCatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
      float t0 = 0f;
      float t1 = GetKnot(t0, p0, p1);
      float t2 = GetKnot(t1, p1, p2);
      float t3 = GetKnot(t2, p2, p3);

      // Map normalized section parameter [0, 1]
      // into the actual knot interval [t1, t2].
      float knotT = Mathf.Lerp(t1, t2, Mathf.Clamp01(t));

      Vector2 a1 = Interpolate(p0, p1, t0, t1, knotT);
      Vector2 a2 = Interpolate(p1, p2, t1, t2, knotT);
      Vector2 a3 = Interpolate(p2, p3, t2, t3, knotT);
      
      Vector2 b1 = Interpolate(a1, a2, t0, t2, knotT);
      Vector2 b2 = Interpolate(a2, a3, t1, t3, knotT);

      return Interpolate(b1, b2, t1, t2, knotT);
    }

    private static float GetKnot(float previousKnot, Vector2 a, Vector2 b)
    {
      float distance = Vector2.Distance(a, b);
      distance = Mathf.Max(distance, MinimumKnotDistance);
      
      return previousKnot + Mathf.Pow(distance, CatmullRomAlpha);
    }

    private static Vector2 Interpolate(Vector2 a, Vector2 b, float knotA, float knotB, float t)
    {
      float denominator = knotB - knotA;

      if (Mathf.Abs(denominator) < MinimumKnotDistance)
        return a;

      float blend = (t - knotA) / denominator;

      return Vector2.LerpUnclamped(a, b, blend);
    }

    private static void AddQuad(LayerSubMesh subMesh, int a, int b, int c, int d)
    {
      subMesh.tris.Add(a);
      subMesh.tris.Add(b);
      subMesh.tris.Add(c);

      subMesh.tris.Add(a);
      subMesh.tris.Add(c);
      subMesh.tris.Add(d);
    }

    private static void BuildBasis(Vector3 poleDirection, out Vector3 tangent, out Vector3 bitangent)
    {
      Vector3 reference = Mathf.Abs(Vector3.Dot(poleDirection, Vector3.forward)) > 0.95f
          ? Vector3.right
          : Vector3.forward;

      tangent = Vector3.Cross(poleDirection, reference).normalized;
      bitangent = Vector3.Cross(poleDirection, tangent).normalized;
    }
  }
}