using System;
using System.Collections.Generic;
using System.Text;

namespace DmvAppointmentScheduler
{
    public class Teller : IComparable
    {
        public string id { get; set; }
        public string specialtyType { get; set; }
        public string multiplier { get; set; }
        public int length { get; set; } = 0;

        public int CompareTo(object obj)
        {
            Teller otherTeller = obj as Teller;
            return this.length.CompareTo(otherTeller.length); ;
        }
    }

    public class TellerList
    {
        public List<Teller> Teller { get; set; }
    }
}
