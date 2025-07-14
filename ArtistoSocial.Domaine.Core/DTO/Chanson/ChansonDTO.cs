using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistoSocial.Domaine.Core.DTO.Chanson
{
    public class ChansonDTO
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Legende { get; set; }
        public string UrlFichier { get; set; }
        public DateTime DatePublication { get; set; }
        public int NbLikes { get; set; }

        public int ArtisteId { get; set; }
        public string ArtisteUserName { get; set; }
    }
}
