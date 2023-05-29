using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Identity;

namespace Domain.Auth;

public sealed class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Token { get; init; }
    public required string JwtId { get; init; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Used { get; set; } = false;
    public bool Invalidated { get; set; }
    public required string UserId { get; init; }
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }
}