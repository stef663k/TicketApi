using System.Text.Json;
using System.Text.Json.Serialization;
using TicketApi.Models;
namespace TicketApi.Converters;
public class UserRoleConverter : JsonConverter<UserRole>
{
    public override UserRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var roleString = reader.GetString();
            if (Enum.TryParse<UserRole>(roleString, true, out var role))
            {
                return role;
            }
            throw new JsonException($"Unable to convert {roleString} to {nameof(UserRole)}.");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return (UserRole)reader.GetInt32();
        }

        throw new JsonException($"Unable to convert {reader.GetString()} to {nameof(UserRole)}.");
    }

    public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());  // Writing role as string
    }
}
