using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Task
{
    class Program
    {
        static void Main(string[] args)
        {
            var employees = Deserialize();
            for (int i = 0; i < employees.Count; i++)
                Print(i, employees[i]);
            var sortedEmps = SortEmployees(employees);
            Serialize(sortedEmps);
        }

        static List<Employee> Deserialize()
        {
            try
            {
                using (var file = File.OpenRead("my.xml"))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<Employee>),
                        new XmlRootAttribute("Employees"));
                    return (List<Employee>)xml.Deserialize(file);
                }

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(
                    "Не был найден файл my.xml, в котором должен храниться список сотрудников в XML формате.");
                return new List<Employee>();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(@"Файл имеет неправильный формат.
Возможно вы не так назвали рутовую ноду(ее необходимо назвать Employee)
Возможно вы не так назвали или не указали один из элементов Employee.
Дополнительные сведения: {0}", ex.Message);
                return new List<Employee>();
            }
        }

        static void Print(int index, Employee e)
        {
            string id = getValueFromPrivateField<string>("EmployeeID", e);
            string address = getValueFromPrivateField<string>("address", e);

            Console.WriteLine("{0}, Last Name: {1}, First Name: {2}, Age: {3}, Department: {4}, Address: {5}, id: {6}",
                index, e.LastName, e.FirstName, e.Age, e.Department, address, id);
        }

        static List<Employee> SortEmployees(IEnumerable<Employee> emps)
        {
            return emps.Where(e => e.Age > 24 && e.Age < 36)
                .OrderBy(
                    e => getValueFromPrivateField<string>("EmployeeID", e))
                .ToList();
        }

        private static T getValueFromPrivateField<T>(string name, object target)
        {
            return (T)target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(target);
        }

        static void Serialize(List<Employee> list)
        {
            using (var f = File.Create("my_new.xml"))
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<Employee>),
                    new XmlRootAttribute("Employees"));
                xml.Serialize(f, list);
            }
        }
    }
}


// https://drive.google.com/folderview?id=0B-NEcS071j6Gfm9YVDVzdjNIOFlDVy1EXzFLQm54WEVaczNYMDBleDdnNUlCYVVyaVVpNEU&usp=sharing
