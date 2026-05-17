using System.ComponentModel.DataAnnotations;

namespace noKeyCloud.Contracts.Authenticate;

public record LogoutUserRequest(
    [Required(ErrorMessage = "Refresh token is required.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Invalid token length.")]
    string RefreshToken);