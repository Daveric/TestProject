using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Data.Entities
{
    public class Application: IEntity
    {
        public int Id { get; set; }

        [MaxLength(60)]
        [Required]
        public string Name { get; set; }

        public Guid AppId { get; set; }

        public User User { get; set; }
    }
}
