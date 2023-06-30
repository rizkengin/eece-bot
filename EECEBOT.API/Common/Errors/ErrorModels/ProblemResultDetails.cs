using System.Text.Json.Serialization;

namespace EECEBOT.API.Common.Errors.ErrorModels;

public class ProblemResultDetails
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("status")]
    public int? Status { get; set; }
    
    [JsonPropertyName("invocationId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string InvocationId { get; set; }
    
    [JsonPropertyName("detail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; set; }
    
    [JsonPropertyName("errorCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }
    
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }
}