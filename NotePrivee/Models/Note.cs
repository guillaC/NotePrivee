using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace NotePrivee.Models
{
    public partial class Note
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string Contenu { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime DateCreation { get; set; }
        [DefaultValue(1)]
        [Display(Name = "Nombre de lecteurs")]
        public int NombreVue { get; set; }
        [Display(Name = "Date d'expiration")]
        public DateTime? DateExpiration { get; set; }
        
    }
}
