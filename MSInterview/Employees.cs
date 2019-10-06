using System;
using System.Collections.Generic;
using System.Text;

namespace MSInterview
{
    //Too close to the fire. Having Employees and Employee class. Employees was originally called Company
    public class Employees
    {
        Employee ceo;//Will be the root node of the tree
        int indexOfCeo = -1;//Used for building the tree. Start instead of having to search for it before search.

        public Employees(string inputString)
        {
            if (inputString == "" || inputString == null)
            {
                throw new FormatException("Empty string entered.\n");
            }
            //You can nest all these methods. They are separated for clarity
            string[] lines = BreakIntoLines(inputString);
            List<Employee> tempEmps = CreateEmployeesFromLines(lines);//tempEmps is temporaryEmployee List
            lines = new string[0];//Freeing up space.
            BuildTree(tempEmps);
            StartSetCumulativeSalaries();
        }

        private string[] BreakIntoLines(string inputString)
        {
            char[] separators = new char[] { '\n', '\r' };
            string[] lines = inputString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                throw new FormatException("No data found in the string entered.\n");
            }
            return lines;
        }

        private List<Employee> CreateEmployeesFromLines(string[] lines)
        {
            int countLines = lines.Length;
            List<Employee> tempEmps = new List<Employee>(countLines);

            //HashSet is for ensuring Employee Ids are unique. If you encounter duplicates, reject entry
            HashSet<string> addedEmployeeIds = new HashSet<string>();
            //List errors is for recording errors in lines/CSV when creating Employee 
            StringBuilder errorStrBuilder = new StringBuilder();

            int ceoFound = 0;

            for (int i = 0; i < countLines; i++)
            {
                try
                {
                    Employee emp = new Employee(lines[i]);
                    if (addedEmployeeIds.Contains(emp.EmpId))
                    {
                        errorStrBuilder.Append((i + 1) + ". '" + emp.EmpId + "' has already been added.\n");
                    }
                    else
                    {
                        if (emp.ManagerId == "")
                        {
                            if (ceoFound == 0)
                            {
                                ++ceoFound;
                                this.ceo = emp;
                                this.indexOfCeo = tempEmps.Count;//Before insertion... 
                                tempEmps.Add(emp);
                                addedEmployeeIds.Add(emp.EmpId);
                            }
                            else
                            {
                                ++ceoFound;
                            }
                        }
                        else
                        {
                            tempEmps.Add(emp);
                            addedEmployeeIds.Add(emp.EmpId);
                        }
                    }
                }
                catch (FormatException fe)
                {
                    errorStrBuilder.Append((i + 1) + ". " + fe.Message + "\n");
                }
            }

            if (errorStrBuilder.Length > 0)
            {
                throw new FormatException(errorStrBuilder.ToString());
            }

            if (ceoFound != 1)
            {
                if (ceoFound == 0)
                    throw new FormatException("You have no CEO. You have to enter one.\n");
                else
                    throw new FormatException("You can only have one CEO and you have " + ceoFound + ".\n");
            }

            if (tempEmps.Count == 0)
            {
                throw new FormatException("No employees were created successfully.\n");//Overcautious much??????
            }

            if (lines.Length > tempEmps.Count)
            {
                throw new FormatException("Not all lines were successfully added.\n");//Cause only the paranoid survive.
            }
            //Are those all the possible errors?
            return tempEmps;
        }

        private void BuildTree(List<Employee> tempEmps)
        {
            //Using a BFS like way of inserting into tree
            Queue<Employee> queueToBuild = new Queue<Employee>();
            queueToBuild.Enqueue(ceo);

            //Is it better to remove indeces added while looping (cause how long it takes is an issue) or is it better to just record
            //all indeces added but have to iterate though the whole loop over and over
            //Iterating seems easier cause removing involves having to shift all the values to your left so will not do that even though will be going through 
            //an employee we may have added already.
            //Shown both
            //You can use either but not both. Just comment and uncomment where highlighted.

            //1. Where we remove elements immediately after adding them.
            tempEmps.RemoveAt(this.indexOfCeo);
            while (queueToBuild.Count > 0)
            {
                Employee emp = queueToBuild.Dequeue();
                int countSubordinates = queueToBuild.Count;

                List<int> indecesToRemove = new List<int>();

                for (int i = 0; i < tempEmps.Count; i++)
                {
                    if (tempEmps[i].ManagerId == emp.EmpId)
                    {
                        queueToBuild.Enqueue(tempEmps[i]);
                        emp.AddSubordinate(tempEmps[i]);
                        indecesToRemove.Add(i);
                    }
                }

                for (int j = indecesToRemove.Count - 1; j >= 0; j--)
                {
                    tempEmps.RemoveAt(indecesToRemove[j]);
                }

                if (queueToBuild.Count > countSubordinates)
                {
                    emp.HasSubordinates = true;
                }
                else
                {
                    emp.HasSubordinates = false;
                    emp.CumulativeSalary = emp.Salary;
                }
            }

            if (tempEmps.Count > 0)
            {
                StringBuilder strNotAdded = new StringBuilder();
                strNotAdded.Append("The following employees were not added: ");
                for (int i = 0; i < tempEmps.Count; i++)
                {
                    strNotAdded.Append(tempEmps[i].EmpId + ", ");
                }
                strNotAdded.Remove(strNotAdded.Length - 2, 2);
                strNotAdded.Append(".\n");
                throw new FormatException(strNotAdded.ToString());
            }

            //end of version 1


            // 2 of code that does the same thing
            /*
			List<int> indecesAdded = new List<int>();//Comment this out if you opt to change
			indecesAdded.Add(this.indexOfCeo);//Comment this too
			while(queueToBuild.Count>0)
			{
				Employee emp = queueToBuild.Dequeue();
				int countSubordinates = queueToBuild.Count();

				for(int i=0;i<tempEmps.Count;i++)
				{
					if(tempEmps[i].ManagerId==emp.EmpId)
					{
						queueToBuild.Enqueue(tempEmps[i]);
						emp.AddSubordinate(tempEmps[i]);
						indecesAdded.Add(i);
					}
				}
				if(queueToBuild.Count>countSubordinates)
				{
					emp.HasSubordinates=true;
				}
				else
				{
					emp.HasSubordinates=false;
					emp.CumulativeSalary=emp.Salary;
				}
			}

			if(tempEmps.Count>indecesAdded.Count)
			{
				//Find the ones not added
				StringBuilder strBuild = new StringBuilder();

				int howMany=0;
				int offset = 0;
				//Before the first one was entered. Yikes if indecesAdded length is 0
				int firstIndex = indecesAdded[0];
				for(;offset<firstIndex;offset++)
				{
					strBuild.Append(tempEmps[offset].EmpId+", ");
					++howMany;
				}

				//The gaps in between
				for(int i=0;i<indecesAdded.Count-1;i++)
				{
					int currentIndex=indecesAdded[i];
					int nextIndex=indecesAdded[i+1];
					if((currentIndex+1)<nextIndex)
					{
						offset=currentIndex+1;
						for(;offset<nextIndex;offset++)
						{
							strBuild.Append(tempEmps[offset].EmpId+", ");
							++howMany;
						}
					}
				}

				//After last added index, there are could be others
				if((tempEmps.Count-indecesAdded.Count)>howMany)
				{
					offset=indecesAdded[indecesAdded.Count-1]+1;
					for(;offset<tempEmps.Count;offset++)
					{
						strBuild.Append(tempEmps[offset].EmpId+", ");
						++howMany;
					}
				}

				strBuild.Remove(strBuild.Length-2,2);
				strBuild.Append(".");
				throw new FormatException("The following "+howMany+" employee(s) were not added: "+strBuild.ToString());
			}

			//end of version 2. Code calls
			*/
        }

        private void StartSetCumulativeSalaries()
        {
            SetCumulativeSalaries(this.ceo);
        }

        //Recursive code. Using DFS approach to calculate all their salaries.
        private long SetCumulativeSalaries(Employee emp)
        {
            //The -1 check is to speed up the process. No need to calculate salary again if it was calculated on another previous call.
            if (emp.HasSubordinates && emp.CumulativeSalary == -1)
            {
                long totalSalary = emp.Salary;
                for (int i = 0; i < emp.subordinates.Count; i++)
                {
                    totalSalary += SetCumulativeSalaries(emp.subordinates[i]);
                }
                emp.CumulativeSalary = totalSalary;
                return totalSalary;
            }
            else
            {
                //Remember, when building Employees if an employee had no subordinates we set CumulativeSalary to Salary
                //And if we have already calculated CumulativeSalary no need to do it again, that's why by default it is set to -1 
                //to speed this up this recursion
                return emp.CumulativeSalary;
            }
        }

        public string FindSalary(string enteredId)
        {
            if (enteredId == "" || enteredId == null)
            {
                return "You have not entered an employee ID.";
            }

            //You can change which one you want to use. BFS or DFS. Both implemented.

            Employee emp = SearchBreadth(enteredId);
            //Employee emp = CallSearchDepth(enteredId);
            if (emp == null)
            {
                return "'" + enteredId + "' does not belong to any employee.";
            }
            else
            {
                return emp.CumulativeSalary + "";
            }
        }

        //1. First search technique. Uses Breadth First approach. Implemented using a Queue.
        private Employee SearchBreadth(string enteredId)
        {
            Queue<Employee> searchQueue = new Queue<Employee>();
            searchQueue.Enqueue(this.ceo);//Start from the root

            while (searchQueue.Count > 0)
            {
                Employee emp = searchQueue.Dequeue();
                if (emp.EmpId == enteredId)
                {
                    return emp;
                }
                for (int i = 0; i < emp.subordinates.Count; i++)
                    searchQueue.Enqueue(emp.subordinates[i]);
            }

            return null;
        }

        //2. Second search technique. Uses Depth First Search. Implemented using recursion. I wonder, is it possible to use iteration?
        private Employee CallSearchDepth(string enteredId)
        {
            //I know it's repititive to call this if method here but if it is the CEO and it has like 50,000 employees what then?
            if (this.ceo.EmpId == enteredId)
            {
                return this.ceo;
            }
            return SearchDepth(this.ceo, enteredId);
        }

        private Employee SearchDepth(Employee emp, string enteredId)
        {
            if (emp.HasSubordinates)
            {
                for (int i = 0; i < emp.subordinates.Count; i++)
                {
                    Employee possibleEntry = SearchDepth(emp.subordinates[i], enteredId);
                    if (possibleEntry != null)
                    {
                        return possibleEntry;
                    }
                }
                //Do not be tempted to place this before the for loop, cause if you do so, you will not have searched through the whole tree
                //rather you would have just checked the current node and not the children which would produce the wrong results.
                if (emp.EmpId == enteredId)
                {
                    return emp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (emp.EmpId == enteredId)
                {
                    return emp;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    class Employee
    {
        private readonly string empId = "";//Employee ID
        private readonly string managerId = "";
        private readonly long salary = 0;
        private long cumulativeSalary = -1;
        private bool hasSubordinates = false;
        internal List<Employee> subordinates = new List<Employee>();//Need to access it in recursion for calculating cumulative salaries

        public string EmpId { get { return this.empId; } }
        public string ManagerId { get { return this.managerId; } }
        public long Salary { get { return this.salary; } }
        public long CumulativeSalary { get { return this.cumulativeSalary; } set { this.cumulativeSalary = value; } }
        public List<Employee> Subordinates { get { return this.Subordinates; } set { this.subordinates = value; } }
        public bool HasSubordinates { get { return this.hasSubordinates; } set { this.hasSubordinates = value; } }

        public Employee(string empStr)
        {
            if (empStr == "" || empStr == null)
            {
                throw new FormatException("Enter data for employee.");
            }
            else
            {
                string[] fields = empStr.Split(new char[] { ',' });//Do not use StringSplitOptions.RemoveEmptyEntries cause you will not be able to detect CEO
                ValidateFields(fields);//If any error is encountered, will throw exception.
                this.empId = fields[0].Trim();
                this.managerId = fields[1].Trim();
                this.salary = ValidateSalary(fields[2]);//
            }
        }

        private void ValidateFields(string[] fields)
        {
            if (fields.Length != 3)
            {
                throw new FormatException("Use CSV format 'EmployeeId,ManagerId,Salary'.");
            }
            if (fields[0] == "" || fields[0] == null)
            {
                throw new FormatException("Specify employee ID.");
            }
            if (fields[0] == fields[1])
            {
                throw new FormatException("An employee cannot report to themselves.");
            }

        }

        private long ValidateSalary(string possSalary)//possibleSalary
        {
            long tempSalary;
            try
            {
                tempSalary = long.Parse(possSalary.Trim());
            }
            catch (FormatException)
            {
                throw new FormatException("Use numbers to specify salary.");
            }

            if (tempSalary <= 0)
            {
                throw new FormatException("Salary has to be greater than 0.");
            }

            return tempSalary;
        }

        public void AddSubordinate(Employee sub)//subordinate
        {
            subordinates.Add(sub);
        }

        public override string ToString()
        {
            return "Employee ID: " + this.empId + " Manager ID: " + this.managerId + " Salary: " + this.salary + " Count subordinates: " + subordinates.Count + " Cumulative salary: " + cumulativeSalary + ".";
        }
    }
}
