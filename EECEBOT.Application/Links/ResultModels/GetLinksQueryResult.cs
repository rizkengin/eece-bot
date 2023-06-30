namespace EECEBOT.Application.Links.ResultModels;

public sealed record GetLinksQueryResult(IEnumerable<LinkResult> Links);

public sealed record LinkResult(Guid Id, string Name, string Url);