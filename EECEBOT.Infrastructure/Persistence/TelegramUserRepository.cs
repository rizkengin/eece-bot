using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
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

    public async Task CreateAsync(TelegramUser telegramUser, CancellationToken cancellationToken = default)
    {
        _documentSession.Store(telegramUser);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task<TelegramUser?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
    {
        return await _documentSession.Query<TelegramUser>().FirstOrDefaultAsync(x => x.ChatId == chatId,
            cancellationToken);
    }

    public async Task UpdateStudyYearAsync(TelegramUser telegramUser, StudyYear studyYear,
        CancellationToken cancellationToken = default)
    {
        telegramUser.UpdateStudyYear(studyYear);
        _documentSession.Update(telegramUser);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }
}