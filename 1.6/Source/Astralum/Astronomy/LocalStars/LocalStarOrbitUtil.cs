using System.Collections.Generic;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalStars
{
  public static class LocalStarOrbitUtil
  {
    private static Vector3 SystemCenter => Vector3.forward * LocalStarGenerationUtil.DistanceToLocalStars;

    private static LocalStarOrbitalState StateFor(SavedLocalStar star)
    {
      if (star == null)
        return CenterState;

      WorldComponent_CelestialObjectDataCache data = LocalStarDataUtil.Data;
      
      if (data == null || data.LocalStars.NullOrEmpty())
        return CenterState;
      
      SavedLocalStarSystem system = data.LocalStarSystem;
      
      if (system == null)
      {
        return new LocalStarOrbitalState(ProjectToSkySphere(star.LocalSkyPosition), 0f);
      }
      
      List<SavedLocalStar> stars = data.LocalStars;
      
      return stars.Count switch
      {
        1 => CenterState,
        2 => BinaryState(star, stars, system),
        3 => TripleState(star, stars, system),
        _ => CenterState
      };
    }
    
    public static Vector3 PositionFor(SavedLocalStar star)
    {
      return StateFor(star).Position;
    }
    
    public static float DepthFor(SavedLocalStar star)
    {
      return StateFor(star).Depth;
    }
    
    private static LocalStarOrbitalState CenterState => new(SystemCenter, 0f);
    
    private static LocalStarOrbitalState BinaryState(SavedLocalStar star, List<SavedLocalStar> stars,
      SavedLocalStarSystem system)
    {
      SavedLocalStar starA = stars[0];
      SavedLocalStar starB = stars[1];
      
      float totalMass = Mathf.Max(starA.mass + starB.mass, 0.0001f);
      float phase = CurrentPhase(system.innerInitialPhaseRadians, system.innerOrbitalPeriodTicks);
      
      float radiusA = system.innerSeparation * starB.mass / totalMass;
      float radiusB = system.innerSeparation * starA.mass / totalMass;
      
      return star.systemIndex == 0 
        ? ProjectOrbitState(SystemCenter, radiusA, phase, system) 
        : ProjectOrbitState(SystemCenter, radiusB, phase + Mathf.PI, system);
    }
    
    private static LocalStarOrbitalState TripleState(SavedLocalStar star, List<SavedLocalStar> stars,
      SavedLocalStarSystem system)
    {
      SavedLocalStar starA = stars[0];
      SavedLocalStar starB = stars[1];
      SavedLocalStar starC = stars[2];

      float massAB = starA.mass + starB.mass;
      float totalMass = Mathf.Max(massAB + starC.mass, 0.0001f);

      float outerPhase = CurrentPhase(system.outerInitialPhaseRadians, system.outerOrbitalPeriodTicks);
      float radiusAB = system.outerSeparation * starC.mass / totalMass;
      float radiusC = system.outerSeparation * massAB / totalMass;
      
      LocalStarOrbitalState abState = ProjectOrbitState(SystemCenter, radiusAB, outerPhase, system);
      
      if (star.systemIndex == 2)
      {
        return ProjectOrbitState(SystemCenter, radiusC, outerPhase + Mathf.PI, system);
      }
      
      float innerMass = Mathf.Max(massAB, 0.0001f);
      float innerPhase = CurrentPhase(system.innerInitialPhaseRadians, system.innerOrbitalPeriodTicks);
      
      float radiusA = system.innerSeparation * starB.mass / innerMass;
      float radiusB = system.innerSeparation * starA.mass / innerMass;
      
      LocalStarOrbitalState innerState;
      
      innerState = star.systemIndex == 0 
        ? ProjectOrbitOffsetState(radiusA, innerPhase, system) 
        : ProjectOrbitOffsetState(radiusB, innerPhase + Mathf.PI, system);
      
      Vector3 position = ProjectToSkySphere(abState.Position + innerState.Position);
      
      return new LocalStarOrbitalState(position, abState.Depth + innerState.Depth);
    }
    
    private static float CurrentPhase(float initialPhase, int orbitalPeriodTicks)
    {
      if (orbitalPeriodTicks <= 0)
        return initialPhase;
      
      int ticks = Find.TickManager?.TicksGame ?? 0;
      float cycles = ticks / (float)orbitalPeriodTicks;
      
      return initialPhase + cycles * Mathf.PI * 2f;
    }
    
    private static Vector3 ProjectToSkySphere(Vector3 position)
    {
      if (position.sqrMagnitude <= 0.000001f)
        return SystemCenter;
      
      return position.normalized * LocalStarGenerationUtil.DistanceToLocalStars;
    }
    
    private static LocalStarOrbitalState ProjectOrbitState(Vector3 center, float radius, float phase,
      SavedLocalStarSystem system)
    {
      LocalStarOrbitalState offset = ProjectOrbitOffsetState(radius, phase, system);
      Vector3 position = ProjectToSkySphere(center + offset.Position);
      
      return new LocalStarOrbitalState(position, offset.Depth);
    }
    
    private static LocalStarOrbitalState ProjectOrbitOffsetState(float radius, float phase,
      SavedLocalStarSystem system)
    {
      float orbitX = Mathf.Cos(phase) * radius;
      float orbitY = Mathf.Sin(phase) * radius;
      
      float projectedY = orbitY * Mathf.Cos(system.inclinationRadians);
      float depth = orbitY * Mathf.Sin(system.inclinationRadians);
      
      float cosAngle = Mathf.Cos(system.positionAngleRadians);
      float sinAngle = Mathf.Sin(system.positionAngleRadians);
      
      float rotatedX = orbitX * cosAngle - projectedY * sinAngle;
      float rotatedY = orbitX * sinAngle + projectedY * cosAngle;
      
      Vector3 offset = Vector3.right * rotatedX + Vector3.up * rotatedY;
      
      return new LocalStarOrbitalState(offset, depth);
    }
  }
}