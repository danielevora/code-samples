using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FleetManagement {
    public class Program {
        static void Main(string[] args) {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IFleetService, FleetService>()
                .BuildServiceProvider();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            var fleetService = serviceProvider.GetService<IFleetService>();
            var fleet = fleetService.GetFleet();
            var isValidFleet = IsValidFleet(fleet);

            Console.WriteLine(isValidFleet 
                ? "Fleet looks great! All Validation Criteria Met" 
                : "Validation Criteria Not Met");

            logger.LogDebug("All done!");
        }

        static bool IsValidFleet(Fleet fleet) {
            //Note: This could all be refactored

            var fleetContext = new ValidationContext(fleet, null, null);
            var fleetValidationResults = new List<ValidationResult>();
            var isValidFleet = Validator.TryValidateObject(fleet, fleetContext, fleetValidationResults, true);

            if (!isValidFleet) {
                foreach (var fleetValidationResult in fleetValidationResults) {
                    Console.WriteLine(fleetValidationResult.ErrorMessage);
                }
                return false;
            }

            foreach(var vehicle in fleet.Vehicles) {
                var vehicleContext = new ValidationContext(vehicle, null, null);
                var vehicleValidationResults = new List<ValidationResult>();
                var isValidVehicle = Validator.TryValidateObject(vehicle, vehicleContext, vehicleValidationResults, true);
                if (!isValidVehicle) {
                    foreach (var vehicleValidationResult in vehicleValidationResults) {
                        Console.WriteLine(vehicleValidationResult.ErrorMessage);
                    }
                    return false;
                }
            }
            return true;
        }
    }

    public interface IFleetService {
        Fleet GetFleet();
    }

    public class FleetService : IFleetService {
        public Fleet GetFleet() {
            Console.WriteLine("Getting Fleet");
            var result = new Fleet();
            var path = "fleet1.xml";
            var xs = new XmlSerializer(typeof(Fleet));

            try {
                using (var fs = File.OpenRead(path)) {
                    result = (Fleet)xs.Deserialize(fs);
                }
            } catch (InvalidOperationException e) {
                Console.WriteLine($"Malformed XML: {e}");
            }

            return result;
        }
    }

    #region Interfaces

    public interface IFleet {
        [Required]
        [StringLength(100, MinimumLength=1)]
        string NickName { get; set;}
        [Required]
        Vehicle[] Vehicles { get; set;}
    }

    public interface IVehicle {
        [Required]
        string Manufacturer { get; set;}
        [Required]
        string Model { get; set; }
        [Required]
        string Year { get; set; }
    }

    #endregion

    [XmlRoot("Fleet")]
    public class Fleet : IFleet {
        [Required]
        [StringLength(100, MinimumLength=1)]
        [XmlElement("NickName")]
        public string NickName { get; set; }

        [Required]
        [XmlArray]
        [XmlArrayItem(typeof(Bus), ElementName = "Bus"),
        XmlArrayItem(typeof(Truck), ElementName = "Truck"),
        XmlArrayItem(typeof(Motorcycle), ElementName = "Motorcycle"),
        XmlArrayItem(typeof(Boat), ElementName = "Boat")]
        public Vehicle[] Vehicles { get; set; }
    }

    public abstract class Vehicle : IVehicle {
        [Required]
        [XmlElement("Make")]
         public string Manufacturer { get; set; }
        
        [Required]
        [XmlElement("Model")]
         public string Model { get; set; }

        [Required]
        [XmlElement("Year")]
         public string Year { get; set; }
    }

    public class Bus : Vehicle {

    }

    public class Truck : Vehicle {

    }

    public class Motorcycle : Vehicle {}

    public class Boat : Vehicle {
        [XmlElement("Type")]
        public string Type;
    }

    #region Validation Attributes 

    #endregion
}