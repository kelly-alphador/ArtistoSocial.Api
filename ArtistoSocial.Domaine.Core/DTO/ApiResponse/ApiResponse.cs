using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistoSocial.Domaine.Core.DTO.ApiResponse
{
    public class ApiResponse
    {
        public string Message {  get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }= new List<string>();
    }
}
