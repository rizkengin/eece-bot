using System.Collections.Immutable;

namespace EECEBOT.API.Common.Errors.Common;

public static class ClientData
{
    public static readonly ImmutableDictionary<int, string>  ErrorMapping = new Dictionary<int, string>
    {
        {400, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.1"},
        {403, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.3"},
        {404, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.4"},
        {405, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.5"},
        {406, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.6"},
        {408, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.7"},
        {409, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.8"},
        {410, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.9"},
        {411, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.10"},
        {412, "https://www.rfc-editor.org/rfc/rfc7232.html#section-4.2"},
        {413, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.11"},
        {414, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.12"},
        {415, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.13"},
        {416, "https://www.rfc-editor.org/rfc/rfc7233.html#section-4.4"},
        {417, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.14"},
        {418, "https://www.rfc-editor.org/rfc/rfc2324.html"},
        {421, "https://www.rfc-editor.org/rfc/rfc7540.html#section-9.1.2"},
        {422, "https://www.rfc-editor.org/rfc/rfc4918.html#section-11.2"},
        {423, "https://www.rfc-editor.org/rfc/rfc4918.html#section-11.3"},
        {424, "https://www.rfc-editor.org/rfc/rfc4918.html#section-11.4"},
        {426, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.5.15"},
        {428, "https://www.rfc-editor.org/rfc/rfc6585.html#section-3"},
        {429, "https://www.rfc-editor.org/rfc/rfc6585.html#section-4"},
        {431, "https://www.rfc-editor.org/rfc/rfc6585.html#section-5"},
        {500, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.6.1"},
        {501, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.6.2"},
        {502, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.6.3"},
        {503, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.6.4"},
        {504, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.6.5"},
        {505, "https://www.rfc-editor.org/rfc/rfc7231.html#section-6.6.6"},
        {506, "https://www.rfc-editor.org/rfc/rfc2295.html#section-8.1"},
        {507, "https://www.rfc-editor.org/rfc/rfc4918.html#section-11.5"},
        {508, "https://www.rfc-editor.org/rfc/rfc5842.html#section-7.2"},
        {510, "https://www.rfc-editor.org/rfc/rfc2774.html#section-7"},
        {511, "https://www.rfc-editor.org/rfc/rfc6585.html#section-6"}
    }.ToImmutableDictionary();
}