using System.Collections.Generic;

namespace CV19.Models.Decanat
{
    internal class Group
    {
        public string Name { get; set; }
        public ICollection<Student> Student { get; set; }
    }
}