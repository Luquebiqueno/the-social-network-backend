using TheSocialNetwork.Domain.Users;
using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.UnitTests.Application.TestDoubles;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _usersById = [];

    public int AddCallCount { get; private set; }
    public int UpdateCallCount { get; private set; }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_usersById.GetValueOrDefault(userId));

    public Task<Result> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        AddCallCount++;

        if (_usersById.Values.Any(u => u.Email == user.Email))
            return Task.FromResult(Result.Failure(UserErrors.EmailAlreadyRegistered));

        if (_usersById.Values.Any(u =>
            u.ExternalIdentityProvider == user.ExternalIdentityProvider
            && u.ExternalIdentitySubject == user.ExternalIdentitySubject))
            return Task.FromResult(Result.Failure(UserErrors.ExternalIdentityAlreadyRegistered));

        _usersById[user.Id] = user;
        return Task.FromResult(Result.Success());
    }

    public Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        UpdateCallCount++;

        if (_usersById.Values.Any(u => u.Id != user.Id && u.Email == user.Email))
            return Task.FromResult(Result.Failure(UserErrors.EmailAlreadyRegistered));

        _usersById[user.Id] = user;
        return Task.FromResult(Result.Success());
    }

    public void Seed(User user) => _usersById[user.Id] = user;
}
