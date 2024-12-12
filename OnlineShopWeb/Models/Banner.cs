using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShopWeb.Models
{
    public class Banner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string title { get; set; }
        [Required]
        [MaxLength(500)]
        public string description {get; set; }
        [Required]
        public byte[] Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}