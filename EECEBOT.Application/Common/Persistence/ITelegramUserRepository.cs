using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.TelegramUserAggregate;

namespace EECEBOT.Application.Common.Persistence;
public interface ITelegramUserRepository
{
    void Create(TelegramUser telegramUser);
    Task<TelegramUser?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task<TelegramUser?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TelegramUser>> GetByAcademicYearAsync(Year year, CancellationToken cancellationToken = default);
    void UpdateAcademicYear(TelegramUser telegramUser,Year year);
    void UpdateSection(TelegramUser telegramUser, Section section);
    void UpdateBenchNumber(TelegramUser telegramUser, int benchNumber);
    void ResetAcademicYear(TelegramUser telegramUser);
}