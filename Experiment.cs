using System;

namespace LaboratoryExperimentsApp
{
    class Experiment
    {
       
        public int Id { get; set; }

        public int LaboratoryId { get; set; }

        public string Name { get; set; }

        private int _durationHours;

        public int DurationHours
        {
            get => _durationHours;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Длительность эксперимента не может быть отрицательной");
                _durationHours = value;
            }
        }

        public Experiment(int id, int laboratoryId, string name, int durationHours)
        {
            Id = id;
            LaboratoryId = laboratoryId;
            Name = name;
            DurationHours = durationHours;
        }

        public Experiment() : this(0, 0, "", 0) { }

        public override string ToString() => $"[{Id}] {Name}, лаборатория #{LaboratoryId}, часов: {DurationHours}";
    }
}