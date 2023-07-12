using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Interfaces;

namespace EECEBOT.Domain.DomainEvents;

public sealed record DeadlinesUpdatedDomainEvent(Year Year) : IDomainEvent;