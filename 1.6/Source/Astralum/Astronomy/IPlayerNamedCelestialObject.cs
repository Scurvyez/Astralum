namespace Astralum.Astronomy
{
  public interface IPlayerNamedCelestialObject
  {
    string GeneratedName { get; set; }
    string PlayerSetName { get; set; }
    string DisplayName { get; }
    bool HasPlayerSetName { get; }
  }
}