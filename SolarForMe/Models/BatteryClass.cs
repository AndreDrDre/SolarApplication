using System;
using SolarForMe.Persistance;
using SQLite;
using Xamarin.Forms;

namespace SolarForMe.Models
{
    [Table("Batteries")]
    public class BatteryClass
    {

        [PrimaryKey]
        public int Id { get; set; }


        public int Voltage { get; set; }
        public int AmpHours { get; set; }
        public int ChargeRate { get; set; }
        public int Price { get; set; }
        public int Cycles { get; set; }

        public BatteryClass(int voltage, int ampHours, int chargeRate, int price, int cycles)
        {

            this.AmpHours = ampHours;
            this.Voltage = voltage;
            this.ChargeRate = chargeRate;
            this.Price = price;
            this.Cycles = cycles;

        }

        public BatteryClass() { 
        }
    }
}
