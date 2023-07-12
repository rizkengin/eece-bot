using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.DomainEvents;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EECEBOT.Application.AcademicYears.Events;

internal sealed class DeadlinesUpdatedDomainEventHandler : INotificationHandler<DeadlinesUpdatedDomainEvent>
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public DeadlinesUpdatedDomainEventHandler(
        ITelegramBotClient telegramBotClient,
        ITelegramUserRepository telegramUserRepository)
    {
        _telegramBotClient = telegramBotClient;
        _telegramUserRepository = telegramUserRepository;
    }

    public async Task Handle(DeadlinesUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var telegramUsers = await _telegramUserRepository.GetByAcademicYearAsync(notification.Year, cancellationToken);

       const string message = "<b><u>Deadlines section</u> has been updated!</b>\n" +
                              "<b>You can check the update here : /deadlines</b>";
        
       var tasks = telegramUsers
           .Select(telegramUser => _telegramBotClient.SendTextMessageAsync(telegramUser.ChatId,
               message, parseMode: ParseMode.Html,
               cancellationToken: cancellationToken))
           .Cast<Task>()
           .ToList();

       await Task.WhenAll(tasks);
    }
}