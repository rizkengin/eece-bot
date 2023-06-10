using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.TelegramUser;

namespace EECEBOT.Application.Common.Persistence;

public interface ITelegramUserRepository
{
    Task CreateAsync(TelegramUser telegramUser, CancellationToken cancellationToken = default);
    Task<TelegramUser?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task UpdateStudyYearAsync(TelegramUser telegramUser,StudyYear studyYear, CancellationToken cancellationToken = default);
}