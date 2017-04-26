using System;

namespace PsychImmersion.Experiment
{
    [Flags]
    public enum AnimalType {
        None = 0,           //0
        Bee = 1 << 1,       //1
        Mouse = 1 << 2,     //2
        Spider = 1 << 3     //4
    }
}
