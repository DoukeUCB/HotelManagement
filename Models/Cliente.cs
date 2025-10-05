using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApi.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required, StringLength(20)]
        public string Razon_Social { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string NIT { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(30)]
        public string Email { get; set; } = string.Empty;
    }
}
