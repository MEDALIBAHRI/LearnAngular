using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group()
        {
            
        }
        public Group(string groupName)
        {
            Name = groupName;
        }
        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Conenctions { get; set; } = new List<Connection>();
    }
}