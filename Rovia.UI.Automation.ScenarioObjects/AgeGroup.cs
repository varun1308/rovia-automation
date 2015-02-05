using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AgeGroup
    {
        public AgeGroup(string ageGrp)
        {
            var ages = Array.ConvertAll(ageGrp.Split()[0].Remove(0, 1).Split('-'), int.Parse);
            MinAge = ages[0];
            MaxAge = ages[1];
        }
        public int MinAge { get; private set; }
        public int MaxAge { get; private set; }
    }
}
