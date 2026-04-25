namespace Elara.Application.Common.Interfaces
{
    public interface IRagService
    {
        Task<string> GetRelevantChunksAsync(
            string query,
            string subject,
            CancellationToken cancellationToken = default);
    }
}
