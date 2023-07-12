using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Interfaces;

namespace EECEBOT.Domain.DomainEvents;

public sealed record LinksUpdatedDomainEvent(Year Year) : IDomainEvent;