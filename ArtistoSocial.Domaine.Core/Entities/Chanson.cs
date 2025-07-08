using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistoSocial.Domaine.Core.Entities
{
    public class Chanson
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string legende { get; set; }
        public string UrlFichier { get; set; }
        public DateTime DatePublication { get; set; } = DateTime.UtcNow;
        public int nbLikes { get; set; } = 0;
        public Artiste Artiste { get; set; } // Propriété de navigation vers l'artiste
        public int ArtisteId { get; set; } // Clé étrangère vers l'artiste
        // Autres propriétés et méthodes spécifiques à la chanson
    }
}
