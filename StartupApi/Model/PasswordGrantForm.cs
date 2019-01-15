using System;
using System.ComponentModel.DataAnnotations;
using StartupApi.Infrastructure;

namespace StartupApi.Model
{
    public class PasswordGrantForm
    {
        [Required]
        [Display(Name = "grant_type")]
        public string GrantType { get; set; } = "password";

        [Required]
        [Display(Name = "username", Description = "Email address")]
        public string UserName { get; set; }

        [Required, Secret]
        [Display(Name = "password", Description = "Password")]
        public string Password { get; set; }
    }
}
