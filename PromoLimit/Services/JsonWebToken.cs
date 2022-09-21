using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using PromoLimit.States;

namespace PromoLimit.Services
{
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        public object? Deserialize(Type type, string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize(json, type);

        }

        public T? Deserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }

    public class JsonWebToken
    {
        private const string secret = "Jinx? Stands for Jinx! Duh...";

        private IJwtEncoder encoder = new JwtEncoder(new HMACSHA256Algorithm(), new SystemTextJsonSerializer(),
            new JwtBase64UrlEncoder());

        private IJwtDecoder decoder = new JwtDecoder(new SystemTextJsonSerializer(),
            new JwtValidator(new SystemTextJsonSerializer(), new UtcDateTimeProvider()), new JwtBase64UrlEncoder(),
            new HMACSHA256Algorithm());

        public string EncodeToken(ApiKeyInfo keyInfo, TimeSpan validFor = default)
        {
            var payload = new Dictionary<string, object>();
            payload.Add("uuid", keyInfo.uuid);
            //payload.Add("setorId", keyInfo.setorId);
            //payload.Add("gerencia", keyInfo.gerencia);
            payload.Add("displayName", keyInfo.displayName);
            if (validFor != default)
            {
                payload.Add("exp", DateTimeOffset.UtcNow.Add(validFor));
            }

            var token = encoder.Encode(payload, secret);
            return token;
        }

        public bool TryDecodeToken(string token, out string decodedToken)
        {
            try
            {
                decodedToken = decoder.Decode(token, secret, verify: true);
                return true;
            }
            catch (TokenExpiredException tee)
            {
                decodedToken = new SystemTextJsonSerializer().Serialize(tee.PayloadData);
                return false;
            }
        }

        public bool TryDecodeToken<T>(string token, out T result) where T : new()
        {
            string decodedToken;
            try
            {
                decodedToken = decoder.Decode(token, secret, verify: true);
                var deserializedToken = new SystemTextJsonSerializer().Deserialize<T>(decodedToken);
                if (deserializedToken is null)
                {
                    result = new T();
                    return false;
                }
                result = deserializedToken;
                return true;
            }
            catch (TokenExpiredException tee)
            {
                decodedToken = new SystemTextJsonSerializer().Serialize(tee.PayloadData);
                var deserializedToken = new SystemTextJsonSerializer().Deserialize<T>(decodedToken);
                if (deserializedToken is null)
                {
                    result = new T();
                    return false;
                }
                result = deserializedToken;
                return false;
            }
        }
    }

}
