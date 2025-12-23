using System.Security.Claims;
using System.Text.Json;
namespace PersonalizacionProyectoGradoWASM.Helpers
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return new List<Claim>();
            }

            try
            {
                var claims = new List<Claim>();
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs != null)
                {
                    ExtractRolesFromJWT(claims, keyValuePairs);
                    claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? "")));
                }

                return claims;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error parsing JWT: {ex.Message}");
                return new List<Claim>();
            }
        }
        private static void ExtractRolesFromJWT(List<Claim> claims, Dictionary<string, object> keyValuePairs)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
            if (roles != null)
            {
                if (roles is string roleString)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleString));
                }
                else if (roles is JsonElement roleElement && roleElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roleElement.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString()));
                    }
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
