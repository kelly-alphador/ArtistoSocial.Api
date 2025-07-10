using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistoSocial.Domaine.Core.DTO.ApiResponse
{
    public class APiResponseWithData<T>:ApiResponse
    {
        public T? Data { get; set; }
    }
}
