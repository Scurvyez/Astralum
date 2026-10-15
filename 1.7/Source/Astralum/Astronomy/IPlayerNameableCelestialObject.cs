namespace Astralum.Astronomy
{
  public interface IPlayerNameableCelestialObject
  {
    string GeneratedName { get; set; }
    string PlayerSetName { get; set; }
    string DisplayName { get; }
    bool HasPlayerSetName { get; }
  }
}