namespace FacilityOS.Application.Common.Settings;

public class BCryptSettings
{
    public const string SectionName = "BCrypt";
    public int WorkFactor { get; init; } = 12;
}