using EECEBOT.Domain.TelegramUser;
using Telegram.Bot.Types;

namespace EECEBOT.Application.Common.TelegramBot;

public interface ITelegramBotMessageHandler
{
    Task HandleStartFlow(Message message, CancellationToken cancellationToken);
    Task HandlePickingSectionFlow(Message message, CancellationToken cancellationToken);
    Task HandlePickingBenchNumberFlow(Message message, CancellationToken cancellationToken);
    Task HandlePickingAcademicYearError(Message message, CancellationToken cancellationToken);
    Task HandlePickingSectionFlowError(Message message, CancellationToken cancellationToken);
    Task HandlePickingBenchNumberFlowError(Message message, CancellationToken cancellationToken);
    Task HandleScheduleCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleExamsCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleDeadlinesCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleHelpCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleLinksCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
    Task HandleUnknownInput(Message message, CancellationToken cancellationToken);
    Task HandleResetCommand(TelegramUser user, Message message, CancellationToken cancellationToken);
}