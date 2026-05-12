namespace Marketplace.Application.Abstractions;

public interface IFavoritesRepository
{
    Task<IReadOnlySet<Guid>> GetFavoriteTeacherIdsAsync(string sessionId, CancellationToken cancellationToken);
    Task AddFavoriteAsync(string sessionId, Guid teacherId, CancellationToken cancellationToken);
    Task RemoveFavoriteAsync(string sessionId, Guid teacherId, CancellationToken cancellationToken);
}
