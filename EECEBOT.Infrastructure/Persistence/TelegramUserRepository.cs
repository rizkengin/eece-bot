using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.TelegramUserAggregate;
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

    public async Task<IEnumerable<TelegramUser>> GetByAcademicYearAsync(Year year, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<TelegramUser>()
            .Where(x => x.Year == year)
            .ToListAsync(cancellationToken);
    }

    public void UpdateAcademicYear(TelegramUser telegramUser, Year year)
    {
        telegramUser.UpdateAcademicYear(year);
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

    public void Remove(long telegramId)
    {
        var telegramUser = _documentSession.Query<TelegramUser>()
            .FirstOrDefault(x => x.TelegramId == telegramId);
        
        if (telegramUser is not null)
            _documentSession.Delete(telegramUser);
    }
}