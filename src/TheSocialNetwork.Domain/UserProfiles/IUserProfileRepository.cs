using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.UserProfiles;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(
        Guid profileId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default);

    Task<bool> IsUsernameAvailableAsync(
        Username username,
        Guid? excludingProfileId = null,
        CancellationToken cancellationToken = default);

    Task<Result> AddAsync(
        UserProfile profile,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        UserProfile profile,
        CancellationToken cancellationToken = default);
}
