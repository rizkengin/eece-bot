namespace EECEBOT.Application.AcademicYears.ResultModels.LinksResultModels;

public sealed record GetLinksQueryResult(IEnumerable<LinkResult> Links);

public sealed record LinkResult(Guid Id, string Name, string Url);