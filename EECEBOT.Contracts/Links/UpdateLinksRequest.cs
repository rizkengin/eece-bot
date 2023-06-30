namespace EECEBOT.Contracts.Links;

public sealed record UpdateLinksRequest(List<LinkRequest> Links);

public sealed record LinkRequest(string Name, string Url);