namespace Astralum.Harmony
{
  public class TelescopeReportData
  {
    public readonly bool useConstellationReport;
    public readonly string report;
    
    public TelescopeReportData(bool useConstellationReport, string report)
    {
      this.useConstellationReport = useConstellationReport;
      this.report = report;
    }
  }
}