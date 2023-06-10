using EECEBOT.Domain.TelegramUser;
using Telegram.Bot.Types;

namespace EECEBOT.Application.Common.TelegramBot;

public interface ITelegramBotMessageHandler
{
    Task HandleStartFlow(Message message, CancellationToken cancellationToken);
    Task HandlePickingStudyYearError(Message message, CancellationToken cancellationToken);
    Task HandleScheduleCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleExamsCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleDeadlinesCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleHelpCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleLinksCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleUnknownInput(Message message, CancellationToken cancellationToken);
}