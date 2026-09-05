using TheSocialNetwork.Domain.SeedWork;

namespace TheSocialNetwork.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result> AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        User user,
        CancellationToken cancellationToken = default);
}
