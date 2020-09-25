using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        public static List<Appointment> appointmentList1 = new List<Appointment>();
        static void Main(string[] args)
        {
            CustomerList customers = ReadCustomerData();
            TellerList tellers = ReadTellerData();
            Calculation(customers, tellers);
            OutputTotalLengthToConsole();

        }
        private static CustomerList ReadCustomerData()
        {
            string fileName = "CustomerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
            return customerData;

        }
        private static TellerList ReadTellerData()
        {
            string fileName = "TellerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
            return tellerData;

        }
        static void Calculation(CustomerList customers, TellerList tellers)
        {
            //// org code
            //foreach (Customer customer in customers.Customer)
            //{
            //    var appointment = new Appointment(customer, tellers.Teller[0]);
            //    appointmentList.Add(appointment);
            //}

            // Your code goes here .....
            // Re-write this method to be more efficient instead of a assigning all customers to the same teller
            foreach (Customer customer in customers.Customer)
            {
                Console.WriteLine($"{customer.Id}:{customer.duration}:{customer.type}");
                // Find a teller with min multiplier
                var tellerQuery =
                    from teller in tellers.Teller
                    where customer.type == teller.specialtyType
                    orderby teller.multiplier
                    select teller;

                // Gets all Tellers matching tellerQuery
                List<Teller> tellerList = tellerQuery.ToList<Teller>();

                // Find Teller with smallest queue (list)
                var minVal = tellerList.Min();
                IEnumerable<Teller> tellerSmallest = from t in tellerList
                                        orderby t.length
                                        where t.length == minVal.length
                                        select t;

                foreach (var teller in tellerSmallest) 
                {

                    var appointment = new Appointment(customer, teller);
                    appointmentList.Add(appointment);
                    break;
                }
            }
        }


        static void OutputTotalLengthToConsole()
        {

            var tellerAppointments =
                from appointment in appointmentList
                group appointment by appointment.teller into tellerGroup
                select new
                {
                    teller = tellerGroup.Key,
                    totalDuration = tellerGroup.Sum(x => x.duration),
                };
            var max = tellerAppointments.OrderBy(i => i.totalDuration).LastOrDefault();
            Console.WriteLine("Teller " + max.teller.id + " will work for " + max.totalDuration + " minutes!");
        }

    }
}
