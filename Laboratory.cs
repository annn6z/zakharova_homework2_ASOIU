using System;

namespace LaboratoryExperimentsApp
{
    class Laboratory
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }

        public Laboratory(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Laboratory() : this(0, "") { }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }
}