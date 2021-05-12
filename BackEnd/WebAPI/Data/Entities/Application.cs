using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Data.Entities
{
    public class Application: IEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public Guid AppId { get; set; }

        public User User { get; set; }
    }
}
