using System;
using System.Collections.Generic;
using SolarForMe.CalulationControllers;
using SolarForMe.Models;
using SolarForMe.Persistance;
using SQLite;
using Xamarin.Forms;

namespace SolarForMe.Views
{
    public partial class EnterData : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private List<BatteryClass> batterylist;
        private List<BatteryClass> FinalStorageBank = new List<BatteryClass>(); 

        private BatteryClass battteries = new BatteryClass();


        public EnterData()
        {
            InitializeComponent();
            batterylist = new List<BatteryClass>();
            AddBatteries();

            //These Paramters will be retrieved from the user

            SolarCalculations slc = new SolarCalculations(1500,5.5,330,batterylist,60);
            var solarPanels = slc.CalculatePanels();
            var Inverter = slc.CalculateInverter();
            //MPPT is represented as an array
            var MppT = slc.CalculateMPPT();
            var Bat = slc.CalculateBattery();

        }

        public void AddBatteries()
        {
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            if (_connection.Table<BatteryClass>() == null)
            {
                _connection.CreateTableAsync<BatteryClass>();
            }
            //Eventually this Battery List would be populated from sql-server

            battteries = new BatteryClass(2, 1284, 255, 140640, 1500);
            batterylist.Add(battteries);
            battteries = new BatteryClass(6, 428, 85, 43280, 1250);
            batterylist.Add(battteries);
            battteries = new BatteryClass(6, 250, 40, 20080, 1250);
            batterylist.Add(battteries);
            battteries = new BatteryClass(8, 182, 35, 14580, 1250);
            batterylist.Add(battteries);

            battteries = new BatteryClass(12, 60, 200, 30600, 3000);
            batterylist.Add(battteries);
            battteries = new BatteryClass(12, 40, 200, 25220, 1500);
            batterylist.Add(battteries);
            battteries = new BatteryClass(12, 30, 100, 14640, 3000);
            batterylist.Add(battteries);
            battteries = new BatteryClass(12, 25, 155, 13240, 1250);
            batterylist.Add(battteries);

            _connection.InsertAllAsync(batterylist);

        }
    }
}
