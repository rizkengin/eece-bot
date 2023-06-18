using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule.Enums;
using EECEBOT.Domain.TelegramUser;

namespace EECEBOT.Application.Common.Persistence;
public interface ITelegramUserRepository
{
    void Create(TelegramUser telegramUser);
    Task<TelegramUser?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task<TelegramUser?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);
    void UpdateAcademicYear(TelegramUser telegramUser,AcademicYear academicYear);
    void UpdateSection(TelegramUser telegramUser, Section section);
    void UpdateBenchNumber(TelegramUser telegramUser, int benchNumber);
    void ResetAcademicYear(TelegramUser telegramUser);
}