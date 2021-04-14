using System.Collections.Generic;

namespace Pets.Models
{
    public class Owner
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public IEnumerable<Pet> Pets { get; set; }
    }
}
