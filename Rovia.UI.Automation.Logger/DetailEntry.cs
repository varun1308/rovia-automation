﻿using System.Collections.Generic;

namespace Rovia.UI.Automation.Logger
{
    class DetailEntry
    {
        public int Id { get; set; }
        private readonly List<string> _details;

        public DetailEntry()
        {
            _details=new List<string>();
        }

        public void Append(string s)
        {
            _details.Add(s);
        }

        public string Description { get; set; }

        public override string ToString()
        {
            return Id.ToString() + ">> " + Description + "\n" + string.Join("\n", _details) +
                   "------------------------------------------------------------------------------------------------------------------------------------------------------";
        }
    }
}
