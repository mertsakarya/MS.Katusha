using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MS.Katusha.Services.Encryption.Converters
{
    public class EncryptedStringConverter : JsonConverter
    {
        private readonly IEncryptionService _encryptionService;
        public EncryptedStringConverter()
        {
            _encryptionService = DependencyResolver.Current.GetService<IEncryptionService>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string output;
            if (value is string) {
                output = _encryptionService.Encrypt((string)value);
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
            var result = _encryptionService.Decrypt(cipherText);
            return result;
        }

        public override bool CanConvert(Type objectType) { return objectType == typeof (string); }
    }
}
