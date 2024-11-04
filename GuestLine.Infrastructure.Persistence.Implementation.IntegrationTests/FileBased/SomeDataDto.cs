using System.Text.Json.Serialization;

namespace GuestLine.Infrastructure.Persistence.Implementation.IntegrationTests.FileBased;

// "id":  "id1",
// "some_propery": "property_val",
// "nested_object": {
//     "nested_object_property": "nested_value"
// }

public class NestedObjectDto
{
    [JsonPropertyName("nested_object_property")]
    public required string NestedObjectProperty { get; set; }
}

public class SomeDataDto
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("some_property")]
    public required string SomeProperty { get; set; }
    [JsonPropertyName("nested_object")]
    public required NestedObjectDto NestedObject { get; set; }
}