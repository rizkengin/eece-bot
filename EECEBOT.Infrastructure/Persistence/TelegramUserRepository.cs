using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule.Enums;
using EECEBOT.Domain.TelegramUser;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class TelegramUserRepository : ITelegramUserRepository
{
    private readonly IDocumentSession _documentSession;

    public TelegramUserRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public void Create(TelegramUser telegramUser)
    {
        _documentSession.Store(telegramUser);
    }

    public async Task<TelegramUser?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<TelegramUser>().FirstOrDefaultAsync(x => x.ChatId == chatId,
            cancellationToken);
    }

    public async Task<TelegramUser?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<TelegramUser>()
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId, cancellationToken);
    }

    public void UpdateAcademicYear(TelegramUser telegramUser, AcademicYear academicYear)
    {
        telegramUser.UpdateAcademicYear(academicYear);
        _documentSession.Update(telegramUser);
    }

    public void UpdateSection(TelegramUser telegramUser, Section section)
    {
        telegramUser.UpdateSection(section);
        _documentSession.Update(telegramUser);
    }

    public void UpdateBenchNumber(TelegramUser telegramUser, int benchNumber)
    {
        telegramUser.UpdateBenchNumber(benchNumber);
        _documentSession.Update(telegramUser);
    }

    public void ResetAcademicYear(TelegramUser telegramUser)
    {
        telegramUser.ResetAcademicYear();
        _documentSession.Update(telegramUser);
    }
}