using System;
using System.Collections.Generic;
using SolarForMe.Models;
using SolarForMe.Persistance;
using SQLite;
using Xamarin.Forms;

namespace SolarForMe.CalulationControllers
{

    //Put the option for huse to supply only 60% of demand.
    //Maybe they want less batteries


    public class SolarCalculations
    {
        private double _peakSunHours;
        public double _monthylyLoad;
        public int _PanelWatt;
        private double _usagePercent;
        private const int modseries = 3;
        private int counter100A = 0;
        private int counter60A = 0;
        private int counter40A = 0;
        private int counter20A = 0;
        // private SQLiteAsyncConnection _connection;
        private List<BatteryClass> batterylist;
        private List<BatteryClass> batterylistX2String;
        private List<BatteryClass> batterylistX3String;
        private List<BatteryClass> BatteryRefiner;


        public SolarCalculations(double monthylyLoad, double peakSunhours, int panel, List<BatteryClass> batteryListRecieved, double usagePercent)
        {

            this._monthylyLoad = monthylyLoad;
            this._peakSunHours = peakSunhours;
            this._PanelWatt = panel;
            this._usagePercent = usagePercent;
            batterylist = batteryListRecieved;

        }

        //Calculate Required Inverter Size

        public double CalculateInverter()
        {
            var inverter = (_monthylyLoad / (0.8 * (_peakSunHours * 30)));
            switch (inverter)
            {
                case var expression when (inverter <= 5): //This still needs to be edited
                    inverter = 5;
                    break;

                case var expression when (inverter >= 5.5 && inverter < 8.5):
                    inverter = 8;
                    break;

                case var expression when (inverter > 8 && inverter < 10):
                    inverter = 10;
                    break;

                case var expression when (inverter > 10 && inverter < 15.2):
                    inverter = 15;
                    break;

                case var expression when (inverter > 15):
                    inverter = 0;
                    break;
            }

            if (inverter == 0)
            {
                //business solution, we need a 3 phase combination ; 3 8's or 3 15's
                inverter = 1;
            }

            return inverter;
        }

        //Add Roof Area Contraint ; This will automatically limit the number of MPPTs


        //Calculate Required Solar PanelNumber 

        public double CalculatePanels()
        {
            var dailyLoad = (_monthylyLoad / 30) * 1000;
            var NoOfPanels = (Math.Ceiling((dailyLoad / (_peakSunHours * _PanelWatt)) / 3)) * 3;
            return NoOfPanels;
        }

        //Calculte Required MPPT

        public int[] CalculateMPPT()
        {
            var noOfPanels = CalculatePanels();
            var mpptPower = (noOfPanels * _PanelWatt) / 1000; //Convert to kWh

            if (mpptPower > 5 || mpptPower > 3) //Also if it greater than say 2.8 we might want a bigger system in future suggest a bigger expansion system
            {
                do
                {
                    counter100A++;
                    mpptPower = mpptPower - 5;
                    noOfPanels = noOfPanels - 15;

                } while (mpptPower > 5 || mpptPower > 3);

            }

            if (mpptPower > 2 & mpptPower <= 3)
            {
                do
                {
                    counter60A++;
                    mpptPower = mpptPower - 3;
                    noOfPanels = noOfPanels - 9;

                } while (mpptPower > 2 & mpptPower <= 3);

            }

            if (mpptPower > 1 & mpptPower <= 2)
            {
                do
                {
                    counter40A++;
                    mpptPower = mpptPower - 2;
                    noOfPanels = noOfPanels - 6;

                } while (mpptPower > 1 & mpptPower <= 2);
            }

            if (mpptPower <= 1 & mpptPower > 0 || noOfPanels == 3)
            {
                do
                {
                    counter20A++;
                    mpptPower = mpptPower - 1;
                    noOfPanels = noOfPanels - 3;

                } while (mpptPower <= 1 & mpptPower > 0);
            }

            //We need to put in a roof Size and solar panel check therefore 2 by 1

            int[] arrayMppT = new int[4];

            arrayMppT[0] = counter100A;
            arrayMppT[1] = counter60A;
            arrayMppT[2] = counter40A;
            arrayMppT[3] = counter20A;
            return arrayMppT;
        }


        public List<BatteryClass> CalculateBattery()
        {
            BatteryRefiner = new List<BatteryClass>();
            batterylistX2String = new List<BatteryClass>();
            batterylistX3String = new List<BatteryClass>();


            //Converting the orginal battery bank price to 3 and 2 strings
            for (int i = 0; i < batterylist.Count; i++)
            {
                batterylistX2String[i].AmpHours = batterylist[i].AmpHours * 2;
                batterylistX3String[i].AmpHours = batterylist[i].AmpHours * 3;

                batterylistX2String[i].ChargeRate = batterylist[i].ChargeRate * 2;
                batterylistX3String[i].ChargeRate = batterylist[i].ChargeRate * 3;

                batterylistX2String[i].Price = batterylist[i].Price * 2;
                batterylistX3String[i].Price = batterylist[i].Price * 3;

            }

            //Calculting Charge Rate

            int[] mpptMaxOutPut = CalculateMPPT();
            int[] mpptPowerAddition = new int[] { 100, 60, 40, 20 };
            var mpptpower = 0;

            foreach (var i in mpptMaxOutPut)
            {
                mpptpower = mpptpower + mpptMaxOutPut[i] * mpptPowerAddition[i];
            }

            //Calculating ampH required

            var UsagePercentage = 100 - _usagePercent;
            int AmpHourRequired = (int)(Math.Round((((_monthylyLoad * 1000) / 30) * 1.2 * 2) / 48) * (UsagePercentage / 100));

            if (UsagePercentage >= 65)
            {
                //Thus we need to charge fast. Because we are using alot of power in the day
                // Thus within 5%

                for (int i = 0; i < batterylist.Count - 1; i++)
                {
                    if (AmpHourRequired < batterylist[i].AmpHours) //If the amount of storage requird is less than that of what of the battery is able to produce
                    {
                        if (mpptpower >= batterylist[i].ChargeRate * 0.95 && mpptpower <= batterylist[i].ChargeRate * 1.05)
                        {
                            BatteryRefiner.Add(batterylist[i]);
                            break;
                        }
                    }

                }

                if (BatteryRefiner.Count == 0)
                {
                    for (int i = 0; i < batterylistX2String.Count; i++)
                    {
                        if (mpptpower >= batterylistX2String[i].ChargeRate * 0.95 && mpptpower <= batterylistX2String[i].ChargeRate * 1.05)
                        {
                            BatteryRefiner.Add(batterylistX2String[i]);
                            break;
                        }

                    }
                }

                if (BatteryRefiner.Count == 0)
                {
                    for (int i = 0; i < batterylistX3String.Count; i++)
                    {
                        if (mpptpower >= batterylistX3String[i].ChargeRate * 0.95 && mpptpower <= batterylistX3String[i].ChargeRate * 1.05)
                        {
                            BatteryRefiner.Add(batterylistX3String[i]);
                            break;
                        }

                    }
                }

                else if (UsagePercentage < 65)
                {
                    //Thus we dont need to charge so fast; therefore a slower charge rate is okay
                    //Thus within 20%
                    for (int i = 0; i < batterylist.Count - 1; i++)
                    {
                        if (AmpHourRequired < batterylist[i].AmpHours) //If the amount of storage requird is less than that of what of the battery is able to produce
                        {
                            if (mpptpower >= batterylist[i].ChargeRate * 0.8 && mpptpower <= batterylist[i].ChargeRate * 1.2)
                            {
                                BatteryRefiner.Add(batterylist[i]);
                                break;
                            }
                        }

                    }

                    if (BatteryRefiner.Count == 0)
                    {
                        for (int i = 0; i < batterylistX2String.Count; i++)
                        {
                            if (mpptpower >= batterylistX2String[i].ChargeRate * 0.8 && mpptpower <= batterylistX2String[i].ChargeRate * 1.2)
                            {
                                BatteryRefiner.Add(batterylistX2String[i]);
                                break;
                            }

                        }
                    }

                    if (BatteryRefiner.Count == 0)
                    {
                        for (int i = 0; i < batterylistX3String.Count; i++)
                        {
                            if (mpptpower >= batterylistX3String[i].ChargeRate * 0.8 && mpptpower <= batterylistX3String[i].ChargeRate * 1.2)
                            {
                                BatteryRefiner.Add(batterylistX3String[i]);
                                break;
                            }

                        }

                    }
                }
            
             

            }
            List<BatteryClass> batteryFinalList = new List<BatteryClass>();

            //batteryFinalList.Add(BatteryRefiner);

            return batteryFinalList;
        }

    }
}