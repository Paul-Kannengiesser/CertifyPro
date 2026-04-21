namespace CertifyPro.Models;

public class CriteriaAnswer
{
    public string CriteriaId { get; set; } = string.Empty;
    public string CriteriaName { get; set; } = string.Empty;
    public int Rating { get; set; } = 3;
    public string? Comment { get; set; }
}
