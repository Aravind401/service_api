namespace ServiceApi.Models;

public class CertificateScanLog
{
    public int Id { get; set; }
    public int EmployeeRecordId { get; set; }
    public EmployeeRecord? EmployeeRecord { get; set; }
    public DateTime ScannedAtUtc { get; set; } = DateTime.UtcNow;
    public string? ScannerIp { get; set; }
}
