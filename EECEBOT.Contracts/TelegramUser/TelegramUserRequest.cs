using EECEBOT.Domain.Common.Enums;

namespace EECEBOT.Contracts.TelegramUser;

public record TelegramUserRequest(
    string Username,
    string FirstName,
    string LastName,
    long ChatId,
    AcademicYear AcademicYear);