using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace CalorieCalculator.API
{
    //INPUTS: sex age height weight
    //OUTPUT: person's ideal body weight and daily recommended caloric intake

    #region Enums
    
    public enum The_sex {
        Male,
        Female
    }

    #endregion

    //TODO: Rename back to Calc
    public class Calc
    {
        #region Properties

        private static Patient _currentPatient;
        public static string DISTANCE_FROM_IDEAL_WEIGHT { get; set; }
        public static string IDEAL_WEIGHT { get; set; }
        public static string CALORIES { get; set; }

        #endregion

        #region Utilities & Validation

        private static void Clear() {
            _currentPatient = null;
            DISTANCE_FROM_IDEAL_WEIGHT = String.Empty;
            IDEAL_WEIGHT = String.Empty;
            CALORIES = String.Empty;
        }
        private static PatientPhysicalDataModel PreparePatientPhysicalModel(string heightFeet, string heightInches, string weight, string age) {
            var model = new PatientPhysicalDataModel();
            model.Age = age;
            model.HeightFeet = heightFeet;
            model.HeightInches = heightInches;
            return model;
        } 

        private static PatientPersonalDataModel PreparePatientPersonalModel(string firstName, string lastName, string ssnPart1, string ssnPart2, string ssnPart3) {{
            var model = new PatientPersonalDataModel();
            model.FirstName = firstName;
            model.LastName = lastName;
            model.SsnPart1 = ssnPart1;
            model.SsnPart2 = ssnPart2;
            model.SsnPart3 = ssnPart3;
            return model;
        }}
        
        private static bool IsPatientPhysicalDataValid(PatientPhysicalDataModel model) {
            double result;
            if (!double.TryParse(model.HeightFeet, out result)) {
                Console.WriteLine("Feet must be a numeric value.");
                return false;
            }
            if (!double.TryParse(model.HeightInches, out result)) {
                Console.WriteLine("Inches must be a numeric value.");
                return false;
            }
            if (!double.TryParse(model.Weight, out result)) {
                Console.WriteLine("Weight must be a numeric value.");
                return false;
            }
            if (!double.TryParse(model.Age, out result)) {
                Console.WriteLine("Age must be a numeric value.");
                return false;
            }
            if (!(Convert.ToDouble(model.HeightFeet) >= 5)) {
                Console.WriteLine("Height has to be equal to or greater than 5 feet!");
                return false;
            }
            return true;
        }

        private static bool IsPatientPersonalDataValid(PatientPersonalDataModel model) {
            int result;
            if ((!int.TryParse(model.SsnPart1, out result)) |
                (!int.TryParse(model.SsnPart2, out result)) |
                (!int.TryParse(model.SsnPart3, out result))) {
                Console.WriteLine("You must enter valid SSN.");
                return false;
            }
            if (model.FirstName.Trim().Length < 1) {
                Console.WriteLine("You must enter patient’s first name.");
                return false;
            }
            if (model.LastName.Trim().Length < 1) {
                Console.WriteLine("You must enter patient’s last name.");
                return false;
            }
            return true;
        }

        #endregion

        #region Inner Classes

        public class PatientPhysicalDataModel {
            public string HeightFeet { get; set; }
            public string HeightInches { get; set; }
            public string Weight { get; set; }
            public string Age { get; set; }
        }

        public class PatientPersonalDataModel {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string SsnPart1 { get; set; }
            public string SsnPart2 { get; set; }
            public string SsnPart3 { get; set; }
        }

        public abstract class Patient {
            protected string firstName;
            protected string lastName;
            protected double heightInches;
            protected double weight;
            protected double age;
            protected string socialSecurityNumber;

            public string FirstName {
                get { return firstName; }
                set { firstName = value; }
            }
            public string LastName {
                get { return lastName; }
                set { lastName = value; }
            }
            public double HeightInches {
                get { return heightInches; }
                set { heightInches = value; }
            }
            public double Weight {
                get { return weight; }
                set { weight = value; }
            }
            public double Age {
                get { return age; } 
                set { age = value; }
            }
            public string SocialSecurityNumber {
                get { return socialSecurityNumber; }
                set { socialSecurityNumber = value; }
            }

            public abstract double DailyRecommendedCalories();
            public abstract double IdealWeight();
            public double DistanceFromIdealWeight() {
                return weight - IdealWeight();
            }
        }

        public class MalePatient : Patient {
            public override double DailyRecommendedCalories() {
                return (66 + (6.3 * weight) + (12.9 * heightInches) - (6.8 * age));
            }

            public override double IdealWeight() {
                return (50 + (2.3 * heightInches - 60) * 2.2046);
            }
        }

        public class FemalePatient : Patient {
            public override double DailyRecommendedCalories() {
                return (655  + (4.3 * weight) + (4.7 * heightInches) - (4.7 * age));
            }

            public override double IdealWeight() {
                return (45.5 + (2.3 * heightInches - 60) * 2.2046);
            }
        }

        public static class PatientReportIO {

            #region Properties

            private const string XML_FILE_LOCATION = @"\PatientsHistory.xml"; 
            private static Patient _currentPatient;
            private static XmlDocument _currentPatientDocument = null;

            #endregion

            #region Utilities
            
            private static string GetAssemblyDirectory() {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }

            private static string GetHistory() {
                return File.ReadAllText(GetAssemblyDirectory() + XML_FILE_LOCATION);
            }

            private static void LoadPatientDocument() {
                try {
                    _currentPatientDocument.Load(GetAssemblyDirectory() + XML_FILE_LOCATION);
                }
                catch (FileNotFoundException ee) {
                    return;
                }
            }

            private static void SavePatientDocument() {
                _currentPatientDocument.Save(GetAssemblyDirectory() + XML_FILE_LOCATION);
            }

            #endregion

            #region Methods

            private static XmlNode MapPatientMeasurementData(XmlNode measurementNode) {
                measurementNode.Attributes["date"].Value = DateTime.Now.ToString();
                measurementNode["height"].FirstChild.Value = (Convert.ToInt32(_currentPatient.HeightInches)).ToString();
                measurementNode["weight"].FirstChild.Value = _currentPatient.Weight.ToString();
                measurementNode["age"].FirstChild.Value = _currentPatient.Age.ToString();
                measurementNode["dailyCaloriesRecommended"].FirstChild.Value = CALORIES;
                measurementNode["idealBodyWeight"].FirstChild.Value = IDEAL_WEIGHT;
                measurementNode["distanceFromIdealWeight"].FirstChild.Value = DISTANCE_FROM_IDEAL_WEIGHT;
                return measurementNode;
            }

            private static XmlNode MapPatientPersonalData(XmlNode patientNode) {
                patientNode.Attributes["ssn"].Value = _currentPatient.SocialSecurityNumber;
                patientNode.Attributes["firstName"].Value = _currentPatient.FirstName;
                patientNode.Attributes["lastName"].Value = _currentPatient.LastName;
                return patientNode;
            }

            private static void AddNewPatientHistoryNode() {
                _currentPatientDocument.LoadXml(
                    $@"<PatientsHistory>
                        <patient 
                            ssn=""{_currentPatient.SocialSecurityNumber}"" 
                            firstName=""{_currentPatient.FirstName}""
                            lastName=""{_currentPatient.LastName}""
                        >
                            <measurement date=""{DateTime.Now}"">
                                <height>{_currentPatient.HeightInches.ToString()}</height>
                                <weight>{_currentPatient.Weight}</weight>
                                <age>{_currentPatient.Age}</age>
                                <dailyCaloriesRecommended>{CALORIES}</dailyCaloriesRecommended>
                                <idealBodyWeight>{IDEAL_WEIGHT}</idealBodyWeight>
                                <distanceFromIdealWeight>{DISTANCE_FROM_IDEAL_WEIGHT}</distanceFromIdealWeight>
                            </measurement>
                        </patient>
                    </PatientsHistory>"
                );
            }

            private static XmlNode SearchForPatientNodeBySocialSecurityNumber(string socialSecurityNumber) {
                XmlNode patientNode = null;
                foreach (XmlNode node in _currentPatientDocument.FirstChild.ChildNodes) {
                    foreach (XmlAttribute attrib in node.Attributes) {
                        if ((attrib.Name == "ssn") & (attrib.Value == socialSecurityNumber)) {
                            patientNode = node;
                        }
                    }
                }
                return patientNode;
            }

            public static void Save(Patient patient) {
                _currentPatient = patient;

                if (_currentPatient == null) {
                    Console.WriteLine("No Patient Data To Save");
                    return;
                }

                LoadPatientDocument();

                if (_currentPatientDocument == null) {
                    AddNewPatientHistoryNode();
                } else {
                    XmlNode patientNode = SearchForPatientNodeBySocialSecurityNumber(patient.SocialSecurityNumber);
                    if (patientNode == null) {
                        var measurementNode = _currentPatientDocument.DocumentElement.FirstChild["measurement"].CloneNode(true);
                        var newPatientNode = _currentPatientDocument.DocumentElement.FirstChild.CloneNode(false);
                        patientNode.AppendChild(MapPatientMeasurementData(measurementNode));
                        _currentPatientDocument.FirstChild.AppendChild(MapPatientPersonalData(newPatientNode));
                    } else {
                        var measurementNode = patientNode.FirstChild.CloneNode(true);
                        patientNode.AppendChild(MapPatientMeasurementData(patientNode));
                    }
                }
                SavePatientDocument();
            }

            #endregion

        }

        #endregion

        #region API Methods

        public static void Calculate(string heightFeet, string heightInches, string weight, string age, The_sex sex) {
            Clear();

            if (!IsPatientPhysicalDataValid(PreparePatientPhysicalModel(heightFeet, heightInches, weight, age))) {
                return;
            }

            if (sex == The_sex.Male) {
                _currentPatient = new MalePatient();
            } else {
                _currentPatient = new FemalePatient();
            }

            _currentPatient.HeightInches = (Convert.ToDouble(heightFeet) * 12) + Convert.ToDouble(heightInches);
            _currentPatient.Weight = (Convert.ToDouble(weight));
            _currentPatient.Age = (Convert.ToDouble(age));

            CALORIES = _currentPatient.DailyRecommendedCalories().ToString();
            IDEAL_WEIGHT = _currentPatient.IdealWeight().ToString();
            DISTANCE_FROM_IDEAL_WEIGHT = _currentPatient.DistanceFromIdealWeight().ToString();
        }

        public static void Save(string patientSsnPart1, string patientSsnPart2, string patientSsnPart3, string patientFirstName, string patientLastName, string heightFeet, string heightInches, string weight, string age) {
            if (!IsPatientPhysicalDataValid(PreparePatientPhysicalModel(heightFeet, heightInches, weight, age)) || !IsPatientPersonalDataValid(PreparePatientPersonalModel(patientFirstName, patientLastName, patientSsnPart1, patientSsnPart2, patientSsnPart3))) {
                return;
            }

            PatientReportIO.Save(_currentPatient);
        }

        #endregion
        
    }
}