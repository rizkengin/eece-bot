using MediatR;
using Telegram.Bot.Types;

namespace EECEBOT.Application.TelegramBot.Queries.CallbackQueryQuery;

public sealed record CallbackQueryQuery(CallbackQuery CallbackQuery) : IRequest;