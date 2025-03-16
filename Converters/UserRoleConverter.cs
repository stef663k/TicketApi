using System.Text.Json;
using System.Text.Json.Serialization;
using TicketApi.Models;
namespace TicketApi.Converters;
public class UserRoleConverter : JsonConverter<UserRole>
{
    public override UserRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var roleString = reader.GetString();
                if (Enum.TryParse<UserRole>(roleString, true, out var role))
                {
                    return role;
                }
                break;
                
            case JsonTokenType.Number when reader.TryGetInt32(out var numericValue):
                if (Enum.IsDefined(typeof(UserRole), numericValue))
                {
                    return (UserRole)numericValue;
                }
                break;
        }
        
        throw new JsonException($"Unable to convert {reader.GetString()} to {nameof(UserRole)}.");
    }

    public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());  // Writing role as string
    }
}
