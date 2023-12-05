using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.DTO
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
