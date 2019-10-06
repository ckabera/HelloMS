using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSInterview;

namespace MSTestInterview
{
    [TestClass]
    public class TestEmployeesDLL
    {
        //The prupose of this method is to simplify the call to the Employees.dll 
            //instead of having to write this code everytime
        public string BuildEmployees(string entry,string key)
        {
            try
            {
                Employees employees = new Employees(entry);
                return employees.FindSalary(key);
            }
            catch(FormatException fe)
            {
                return fe.Message;
            }
        }

        [TestMethod]
        //Testing whether it does the work it is supposed to
        public void TestIsWorking()
        {
            string entry = "Bruce,,1000\n"
                + "Alfred,Bruce,750\n"
                + "Selina,Bruce,700\n"
                + "Chase,Bruce,650\n"
                + "Dick,Alfred,500\n"
                + "Jason,Alfred,400\n"
                + "Damian,Dick,300\n"
                + "Jesse,Dick,300\n"
                + "Tim,Dick,300";
            //Bruce
            //Alfred,Selina,Chase
            //Dick,	Jason
            //Jesse,Damian,Tim

            string expectedBruce = "4900";
            string expectedAlfred = "2550";
            string expectedDick = "1400";
            string expectedJesse = "300";

            Assert.AreEqual(expectedBruce, BuildEmployees(entry, "Bruce"));
            Assert.AreEqual(expectedAlfred, BuildEmployees(entry, "Alfred"));
            Assert.AreEqual(expectedDick, BuildEmployees(entry, "Dick"));
            Assert.AreEqual(expectedJesse, BuildEmployees(entry, "Jesse"));
        }
        //2. Testing how it handles invalid input for salary
        [TestMethod]
        public void TestingIsSalaryValid()
        {
            //1. Does not accept less than  0 
            string entryLessThan0 = "Clark,,-200";
            string expectedLessThan0 = "1. Salary has to be greater than 0.\n";
            Assert.AreEqual(expectedLessThan0, BuildEmployees(entryLessThan0, "Clark"));

            //2. Does not accept equal to 0 
            string entryEqualTo0 = "Clark,,0";
            string expectedEqualTo0 = "1. Salary has to be greater than 0.\n";
            Assert.AreEqual(expectedEqualTo0, BuildEmployees(entryEqualTo0, "Clark"));

            //3. Does not accept floating point numbers
            string entryFloatingPoint = "Clark,,3.14159";
            string expectedFloating = "1. Use numbers to specify salary.\n";
            Assert.AreEqual(expectedFloating, BuildEmployees(entryFloatingPoint, "Clark"));

            //4. Does not accept strings or characters
            string entryString = "Clark,,Fortress";
            //Get the same error as when you try to use floating point numbers
            Assert.AreEqual(expectedFloating, BuildEmployees(entryString, "Clark"));
        }

        //3. Testing whether an employee has more than one manager.
        [TestMethod]
        public void TestingHasMoreThanOneManager()
        {
            //The code uses first-come-first-served. So only the first one is used.
            //The other entries with similar employee ID are labeled as duplicates.
            string entryMoreThanOne = "Diana,,5000\n"
                + "Steve,Diana,2000\n"
                + "Steve,Artemis,2000\n";
            string expectedMoreThanOne = "3. 'Steve' has already been added.\n";
            Assert.AreEqual(expectedMoreThanOne, BuildEmployees(entryMoreThanOne, "Diana"));
        }

        //4. Testing whether it can accept two or more CEOs or none.
        [TestMethod]
        public void TestingHasNumberOfCEOs()
        {
            //Testing more than 1 CEO being added.
            string entryMoreThanOneCEO = "Dawn,,4000\n"
                + "Hank,,4000\n"
                + "Donna,,4000";
            string expectedMoreThanOneCEO = "You can only have one CEO and you have 3.\n";
            Assert.AreEqual(expectedMoreThanOneCEO, BuildEmployees(entryMoreThanOneCEO, "Hank"));

            //Testing no CEO added.
            string entryNoCEO = "Gar,Dick,3000\n"
                + "Raven,Kory,3000\n"
                + "Jason,Dick,3000";
            string expectedNoCEO = "You have no CEO. You have to enter one.\n";
            Assert.AreEqual(expectedNoCEO, BuildEmployees(entryNoCEO, "Gar"));
        }

        //5. Testing no circular reference
        [TestMethod]
        public void TestingCircularReference()
        {
            //A circular reference will be caught as employees not connected to organization. They were not added at all.
            //It uses CEO who has no manager builds from him or her.
            //So check in list of employees not added. Also catches people who report to themselves.

            //Testing circular reference. Can contain 2 or more employees in circular reference.
            string entryCircular = "Barry,,20000\n"
                + "Clark,Bruce,10000\n"
                + "Bruce,Diana,10000\n"
                + "Diana,Clark,10000";
            string expectedCircular = "The following employees were not added: Clark, Bruce, Diana.\n";
            Assert.AreEqual(expectedCircular, BuildEmployees(entryCircular, "Clark"));

            //Testing self refential entries.
            string entrySelfReference = "Lex,Lex,50000";
            string expectedSelfReference = "1. An employee cannot report to themselves.\n";
            Assert.AreEqual(expectedSelfReference, BuildEmployees(entrySelfReference, "Lex"));
        }

        //6. Testing that all managers are also listed as employees
        [TestMethod]
        public void TestingManagerIsEmployee()
        {
            //Similar to circular reference error. 
            //It works top-to-bottom, so all managers must have been listed as employees to add them as managers
            //So if you try to add an employee who reports to a manager who has not been added, you'll get the name of the employee as not added
            string entryIsManagerEmployee = "Jonz,,5000\n"
                + "Clark,Jonz,4000\n"
                + "Bruce,Jonz,4000\n"
                + "Diana,Jonz,4000\n"
                + "Fate,Constantine,3000";
            string expectedIsManagerEmployee = "The following employees were not added: Fate.\n";
            Assert.AreEqual(expectedIsManagerEmployee, BuildEmployees(entryIsManagerEmployee, "Constantine"));
        }

        //7. Testing other errors
        [TestMethod]
        public void TestingOtherErrors()
        {
            //1. No data entered
            string expectedNoData = "Empty string entered.\n";
            Assert.AreEqual(expectedNoData, BuildEmployees("", "Diana"));

            //2. Not specifying employee ID for querying salary.
            string entryNoEmpID = "Clark,,4000";
            string expectedNoEmpID = "You have not entered an employee ID.";
            Assert.AreEqual(expectedNoEmpID, BuildEmployees(entryNoEmpID, ""));

            //3. 
        }
    }
}
