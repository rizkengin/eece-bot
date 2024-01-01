using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EECEBOT.Application.AcademicYears.Events;

internal sealed class LabScheduleFileUpdatedDomainEventHandler : INotificationHandler<LabScheduleFileUpdatedDomainEvent>
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly ILogger<LabScheduleFileUpdatedDomainEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LabScheduleFileUpdatedDomainEventHandler(
        ITelegramBotClient telegramBotClient,
        ITelegramUserRepository telegramUserRepository,
        ILogger<LabScheduleFileUpdatedDomainEventHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _telegramBotClient = telegramBotClient;
        _telegramUserRepository = telegramUserRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LabScheduleFileUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var telegramUsers = await _telegramUserRepository.GetByAcademicYearAsync(notification.Year, cancellationToken);

       const string message = "<b><u>Lab schedule file</u> has been updated!</b>\n" +
                              "<b>You can check the update here : /schedule</b>";
        
        foreach(var user in telegramUsers)
        {
            try
            {
                await _telegramBotClient.SendTextMessageAsync(user.ChatId,
                    message, parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending lab schedule file updated notification to user {ChatId}", user.ChatId);

                _telegramUserRepository.Remove(user.TelegramId);

                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", user.ChatId, user.FirstName);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}