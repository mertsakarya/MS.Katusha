using System;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Encryption
{
    public class EncryptedStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string output;
            if (value is string) {
                output = EncryptionManager.Encrypt((string)value);
            } else {
                throw new Exception("Expected string value.");
            }
            writer.WriteValue(output);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.String) 
                throw new Exception( String.Format("Unexpected token parsing date. Expected String, got {0}.", reader.TokenType));
            var cipherText = (string)reader.Value;
            if (String.IsNullOrWhiteSpace(cipherText)) return "";
            var result = EncryptionManager.Decrypt(cipherText);
            return result;
        }

        public override bool CanConvert(Type objectType) { return objectType == typeof (string); }
    }
}
