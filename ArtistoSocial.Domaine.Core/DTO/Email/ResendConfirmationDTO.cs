using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistoSocial.Domaine.Core.DTO.Email
{
    public class ResendConfirmationDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
