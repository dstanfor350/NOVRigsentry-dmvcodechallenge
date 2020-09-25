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
            //// original code
            //foreach (Customer customer in customers.Customer)
            //{
            //    var appointment = new Appointment(customer, tellers.Teller[0]);
            //    appointmentList.Add(appointment);
            //}

            // Your code goes here .....
            // Re-write this method to be more efficient instead of a assigning all customers to the same teller

            // Assign a Customer type to a Teller with the minimum queue length with that specialty
            foreach (Customer customer in customers.Customer)
            {
                // Build Query to find a teller with min multiplier
                var tellerQuery =
                    from teller in tellers.Teller
                    where customer.type == teller.specialtyType
                    orderby teller.multiplier
                    select teller;

                // Gets all Tellers matching tellerQuery
                List<Teller> tellerList = tellerQuery.ToList();
                if (tellerList.Count() == 0)
                {
                    Console.WriteLine($"No specialist for Customer Id {customer.Id} : Type {customer.type}");
                    continue;
                }

                // Find smallest queue length in TellersList
                var minVal = tellerList.Min();

                // Query for Tellers with minVal
                IEnumerable<Teller> tellerSmallest = from teller in tellerList
                                                     orderby teller.length
                                                     where teller.length == minVal.length
                                                     select teller;

                // Schedule the Customer appointment and display result
                foreach (var (teller, appointment) in from teller in tellerSmallest
                                                      let appointment = new Appointment(customer, teller)
                                                      select (teller, appointment))
                {
                    appointmentList.Add(appointment);

                    Console.WriteLine($"Customer {customer.Id} : Type {customer.type} : Duration {customer.duration} " +
                        $"| Teller {teller.id} : Queue length {teller.length} : Speciality {teller.specialtyType}");

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
