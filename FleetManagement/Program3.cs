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
            Console.WriteLine("Getting Fleet 3");
            var result = new Fleet();
            var path = "fleet3.xml";
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

        [Required]
        [Vin]
        [XmlElement("VinNumber")]
        public string VehicleIdentificationNumber { get; set; }
    }

    public class LandVehicle : Vehicle {
        [Required]
        [LicensePlateNumber]
        [XmlElement("LicensePlateNumber")]
        public string LicensePlateNumber { get; set; }
    }

    public class SeaVehicle : Vehicle {
        [Required]
        [NauticalRegistrationNumber]
        [XmlElement("NauticalRegistrationNumber")]
        public string NauticalRegistrationNumber { get; set; }
    }

    public class Bus : LandVehicle {}

    public class Truck : LandVehicle {}

    public class Motorcycle : LandVehicle {}

    public class Boat : SeaVehicle {
        [XmlElement("Type")]
        public string Type;
    }

    #region Validation Attributes 

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]  
    public sealed class VinAttribute : ValidationAttribute  
    {  
        //NOTE: Derived from the Wikipedia page for VINs
        //https://en.wikipedia.org/wiki/Vehicle_identification_number
        //TODO: include offending vehicle information
        private const string _errorMessage = "Not a valid VIN";
        private const string _vinWrongSizeMessage = "Vin of incorrect length.";

        private int Transliterate(char c) {
             return "0123456789.ABCDEFGH..JKLMN.P.R..STUVWXYZ".IndexOf(c) % 10;
        }

        private char GetCheckDigit(string vin) {
            var map = "0123456789X";
            var weights = "8765432X098765432";
            var sum = 0;
            for (var i = 0; i < 17; ++i)
                sum += Transliterate(vin[i]) * map.IndexOf(weights[i]);
            return Convert.ToChar(map[sum % 11]);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            ValidationResult result = ValidationResult.Success;  

            string val = Convert.ToString(value);

            if (val.Length != 17) {
                result = new ValidationResult(_vinWrongSizeMessage);
            }

            if (GetCheckDigit(val) != val[8]) {
                result = new ValidationResult(_errorMessage);
            }

            return result;  
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]  
    public sealed class NauticalRegistrationNumberAttribute : ValidationAttribute  
    {
        private const string _errorMessage = "Not a valid nautical registration number";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            ValidationResult result = ValidationResult.Success;  

            if (false) { //TODO: condition for valid nautical registration number
                result = new ValidationResult(_errorMessage);
            }

            return result;  
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]  
    public sealed class LicensePlateNumberAttribute : ValidationAttribute  
    {
        private const string _errorMessage = "Not a valid license plate number";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            ValidationResult result = ValidationResult.Success;  

            if (false) { //TODO: condition for valid license plate number
                result = new ValidationResult(_errorMessage);
            }

            return result;  
        }
    }

    #endregion
}