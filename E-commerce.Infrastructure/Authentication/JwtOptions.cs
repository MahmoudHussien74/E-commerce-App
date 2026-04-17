using System.ComponentModel.DataAnnotations;

namespace E_commerce.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        [Required]
        public string Key { get; init; } = string.Empty;
        [Required]
        public string Issuer { get; init; } = string.Empty;
        [Required]
        public string Audience { get; init; } = string.Empty;
        [Range(1,int.MaxValue)]
        public int ExpiryMinutes { get; init; }

        public string EffectiveKey
            => string.IsNullOrWhiteSpace(Key)
                ? Environment.GetEnvironmentVariable("Jwt__Key") ?? string.Empty
                : Key;
    }
}
