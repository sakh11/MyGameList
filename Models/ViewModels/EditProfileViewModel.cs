using System.ComponentModel.DataAnnotations;

namespace MyGameList.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "Display Name")]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_-]{3,20}$",
            ErrorMessage = "Display name must be 3–20 characters and only contain letters, numbers, underscores, or hyphens.")]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
    }

}
