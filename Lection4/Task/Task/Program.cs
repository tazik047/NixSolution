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
            {
                print(i, employees[i]);
            }
            var sortedEmps = sortEmployeess(employees);
            Serialize(sortedEmps);
        }

        static List<Employee> Deserialize()
        {
            List<Employee> employees;
            using (var f = File.OpenRead("my.xml"))
            {
                // Создаем объект для десериализации списка Employee с рутовой нодой: Employees.
                XmlSerializer xml = new XmlSerializer(typeof(List<Employee>),
                    new XmlRootAttribute("Employees"));
                employees = (List<Employee>)xml.Deserialize(f);
            }
            return employees;
        }

        static void print(int index, Employee e)
        {
            // Получаем значение закрытого поля EmployeeID
            string id = (string)getValueFromPrivateField("EmployeeID", e);
            // Получаем значение закрытого поля addres
            string address = (string)getValueFromPrivateField("address", e);

            Console.WriteLine("{0}, Last Name: {1}, First Name: {2}, Age: {3}, Department: {4}, Address: {5}, id: {6}",
                index, e.LastName, e.FirstName, e.Age, e.Department, address, id);
        }

        static List<Employee> sortEmployeess(IEnumerable<Employee> emps)
        {
            //Выбираем всех сотрудников в возрасте 25-35 и сортируем их по закрытом поля EmployeeID
            return emps.Where(e => e.Age > 24 && e.Age < 36)
                .OrderBy(
                    e => getValueFromPrivateField("EmployeeID", e))
                .ToList();
        }

        static object getValueFromPrivateField(string name, object target)
        {
            return target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(target);
        }

        static void Serialize(List<Employee> list)
        {
            using (var f = File.Create("my_new.xml"))
            {
                // Создаем объект для сериализации списка Employee с рутовой нодой: Employees.
                XmlSerializer xml = new XmlSerializer(typeof(List<Employee>),
                    new XmlRootAttribute("Employees"));
                xml.Serialize(f, list);
            }
        }
    }
}
