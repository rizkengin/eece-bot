namespace EECEBOT.Application.Common.Services;

public interface IBackgroundTasksService
{
    Task ProcessOutboxMessagesAsync();
    Task RequestGithubRepoStarFromUsersAsync();
    Task ExpiredRefreshTokensCleanupAsync();
    Task AcademicYearResetProcessAsync();
}