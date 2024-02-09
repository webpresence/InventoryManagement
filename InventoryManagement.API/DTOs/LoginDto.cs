using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.API.DTOs
{
    /// <summary>
    /// Represents a data transfer object for user login.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
