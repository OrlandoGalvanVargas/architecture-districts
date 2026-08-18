namespace FacilityOS.API.Services;

public interface IResourceAuthorizationService
{
    Task<bool> CanAccessSchoolAsync(int schoolId, CancellationToken cancellationToken = default);
    Task<bool> CanManageSchoolAsync(int schoolId, CancellationToken cancellationToken = default);
    bool CanCreateSchoolInDistrict(int targetDistrictId);
    Task<bool> CanAccessDistrictAsync(int districtId, CancellationToken cancellationToken = default);
}