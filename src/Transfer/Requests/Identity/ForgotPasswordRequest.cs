using System.ComponentModel.DataAnnotations;

namespace Uni.Scan.Transfer.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}