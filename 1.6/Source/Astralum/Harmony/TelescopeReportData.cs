namespace Astralum.Harmony
{
  public class TelescopeReportData
  {
    public readonly bool UseConstellationReport;
    public readonly string Report;
    
    public TelescopeReportData(bool useConstellationReport, string report)
    {
      UseConstellationReport = useConstellationReport;
      Report = report;
    }
  }
}