using TheSocialNetwork.Domain.UserProfiles;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.UnitTests.Application.TestDoubles;

public sealed class FakeUserProfileRepository : IUserProfileRepository
{
    private readonly Dictionary<Guid, UserProfile> _profilesById = [];

    public int AddCallCount { get; private set; }
    public int UpdateCallCount { get; private set; }

    public Task<UserProfile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_profilesById.GetValueOrDefault(profileId));

    public Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_profilesById.Values.SingleOrDefault(p => p.UserId == userId));

    public Task<UserProfile?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default) =>
        Task.FromResult(_profilesById.Values.SingleOrDefault(p => p.Username == username));

    public Task<bool> IsUsernameAvailableAsync(
        Username username,
        Guid? excludingProfileId = null,
        CancellationToken cancellationToken = default)
    {
        var isTaken = _profilesById.Values.Any(p => p.Username == username && p.Id != excludingProfileId);
        return Task.FromResult(!isTaken);
    }

    public Task<Result> AddAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        AddCallCount++;

        if (_profilesById.Values.Any(p => p.UserId == profile.UserId))
            return Task.FromResult(Result.Failure(UserProfileErrors.AlreadyExists));

        if (_profilesById.Values.Any(p => p.Username == profile.Username))
            return Task.FromResult(Result.Failure(UserProfileErrors.UsernameAlreadyTaken));

        _profilesById[profile.Id] = profile;
        return Task.FromResult(Result.Success());
    }

    public Task<Result> UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        UpdateCallCount++;
        _profilesById[profile.Id] = profile;
        return Task.FromResult(Result.Success());
    }

    public void Seed(UserProfile userProfile) => _profilesById[userProfile.Id] = userProfile;
}
