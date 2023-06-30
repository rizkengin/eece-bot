namespace EECEBOT.Contracts.Links;

public record GetLinksResponse(IEnumerable<LinkResponse> Links);

public record LinkResponse(Guid Id,
    string Name,
    string Url);