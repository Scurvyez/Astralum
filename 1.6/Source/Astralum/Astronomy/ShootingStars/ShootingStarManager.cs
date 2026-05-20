using System.Collections.Generic;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.ShootingStars
{
  public static class ShootingStarManager
  {
    private const float DistanceToShootingStars = 20f;

    private static readonly List<ShootingStar> Active = [];

    public static IReadOnlyList<ShootingStar> ActiveStars => Active;

    public static bool Dirty { get; private set; }

    public static void Tick(ModExt_ShootingStars ext)
    {
      bool changed = false;

      for (int i = Active.Count - 1; i >= 0; i--)
      {
        Active[i].age += Time.deltaTime;
        changed = true;

        if (Active[i].Expired)
          Active.RemoveAt(i);
      }

      if (Rand.MTBEventOccurs(ext.mTBIntervalSeconds, 1f, Time.deltaTime))
      {
        SpawnRandom(ext);
        changed = true;
      }

      if (changed)
        Dirty = true;
    }

    public static void Clear()
    {
      Active.Clear();
      Dirty = true;
    }

    public static void ClearDirty()
    {
      Dirty = false;
    }

    private static void SpawnRandom(ModExt_ShootingStars ext)
    {
      Vector3 dir = WorldUtils.RandomGalacticPlaneDirection(ext.galacticPlaneBounds);
      Vector3 origin = dir * DistanceToShootingStars;

      Vector3 tangent = Vector3.Cross(dir, Rand.UnitVector3);

      if (tangent.sqrMagnitude < 0.001f)
        tangent = Vector3.Cross(dir, Vector3.up);

      tangent.Normalize();

      if (Rand.Value < 0.5f)
        tangent = -tangent;

      ShootingStar star = new()
      {
        origin = origin,
        travelDir = tangent,

        age = 0f,
        lifetime = ext.lifetime.RandomInRange,
        travelDistance = ext.travelDistance.RandomInRange,
        length = ext.length.RandomInRange,
        width = ext.width.RandomInRange
      };

      Active.Add(star);
    }
  }
}