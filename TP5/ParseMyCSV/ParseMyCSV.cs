using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParseMyCSV
{
    public enum Houses
    {
        None,
        Gryffindor,
        Hufflepuff,
        Ravenclaw,
        Slytherin,
    }

    public class Student
    {
        public string Name;
        public Houses House;
        public int Points;

        public Student(string name, Houses house, int points)
        {
            Name = name;
            House = house;
            Points = points;
        }

        public static void AssignHouse(Student student)
        {
            List<string> Gryffindor = new List<string>(){"Harry Potter", "Hermione Granger", "Ron Weasley"};            
            List<string> Slytherin = new List<string>(){"Draco Malfoy"};
            List<string> Hufflepuff = new List<string>(){"Ernie Macmillan"};
            List<string> Ravenclaw = new List<string>(){"Terry Boot"};

            if (Gryffindor.Contains(student.Name))
            {
                student.House = Houses.Gryffindor;
                return;
            }
            if (Slytherin.Contains(student.Name))
            {
                student.House = Houses.Slytherin;
                return;
            }
            if (Hufflepuff.Contains(student.Name))
            {
                student.House = Houses.Hufflepuff;
                return;
            }
            if (Ravenclaw.Contains(student.Name))
            {
                student.House = Houses.Ravenclaw;
                return;
            }

            student.House = Houses.None;

        }
    }

    class ParseMyCSV
    {
        public static Dictionary<string, Student> CreateStudentsInfoFromFormat(string path)
        {
            path = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), path);
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error : \"{path}\" does not exist");
                return null;
            }

            StreamReader sr = File.OpenText(path);
            List<string> filesLine=new List<string>();
            string contents;
            while ((contents = sr.ReadLine()) != null)
            {
               filesLine.Add(contents);
            }
            sr.Close();
            if (filesLine.Count == 0)
            {
                return null;
            }
            Dictionary<string, Student> dict = new Dictionary<string, Student>();
            foreach (string line in filesLine)
            {
                
                string[] data = line.Split(',');
                if(data.Length!=3)
                    continue;
                Student student = new Student(data[0],(Houses)int.Parse(data[1]),int.Parse(data[2]));
                if (student.House == Houses.None) Student.AssignHouse(student);
                dict.Add(student.Name,student);
            }

            return dict;
        }

        public static void SaveStudents(Dictionary<string, Student> students, string dest)
        {
            if (students == null) return;
            string destpath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), dest);

            StreamWriter sw = new StreamWriter(File.Open(destpath, FileMode.Create));
            foreach (KeyValuePair<string, Student> kvp in students)
            {
                sw.WriteLine($"{kvp.Value.Name}, { (int)kvp.Value.House}, {kvp.Value.Points}");
            }
            sw.Close();

        }

        public static void AddPoints(Dictionary<string, Student> students, string student, int point)
        {
            if (!students.ContainsKey(student))
            {
                Console.WriteLine($"Error: \"{student}\" does not exist");
                return;
            }

            students[student].Points += point;
        }

        public static void GivePoints(Dictionary<string, Student> students)
        {
            List<string> HarryTeam = new List<string>(){"Harry Potter", "Hermione Granger", "Ron Weasley"};  
            Random r = new Random();
            foreach (var student in students)
            {
                student.Value.Points=r.Next(0, 99);
                if (HarryTeam.Contains(student.Key))
                {
                    AddPoints(students,student.Key,99999);
                }
            }
            
        }

        public static void WinnerOfTheHouseCup(Dictionary<string, Student> students)
        {
            int[] list = new int[5];
            foreach (var student in students)
            {
                if (student.Value.House != Houses.None)
                {
                    list[(int) student.Value.House] += student.Value.Points;
                }   
            }

            int maxValue = list.Max();
            int maxIndex = list.ToList().IndexOf(maxValue);

            Console.WriteLine($"Winner of the House Cup is : {((Houses)maxIndex).ToString()}");

        }

        public static void ListofHouses(Dictionary<string, Student> students, string dest)
        {
            string destpath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), dest);
            StreamWriter sw = new StreamWriter(File.Open(destpath, FileMode.Create));

            sw.WriteLine("House Gryffindor : ");
            foreach (KeyValuePair<string, Student> kvp in students.Where(x => x.Value.House == Houses.Gryffindor))
            {
                sw.WriteLine(kvp.Key);
            }
            sw.WriteLine("\n\nHouse Hufflepuff :");
            foreach (KeyValuePair<string, Student> kvp in students.Where(x => x.Value.House == Houses.Hufflepuff))
            {
                sw.WriteLine(kvp.Key);
            }
            sw.WriteLine("\n\nHouse Ravenclaw :");
            foreach (KeyValuePair<string, Student> kvp in students.Where(x => x.Value.House == Houses.Ravenclaw))
            {
                sw.WriteLine(kvp.Key);
            }
            sw.WriteLine("\n\nHouse Slytherin :");
            foreach (KeyValuePair<string, Student> kvp in students.Where(x => x.Value.House == Houses.Slytherin))
            {
                sw.WriteLine(kvp.Key);
            }
            sw.Close();
            
        }

        public static void Update(Dictionary<string, Student> students, string path)
        {
            path = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), path);
            StreamReader sr = File.OpenText(path);
            string contents;
            while ((contents = sr.ReadLine()) != null)
            {
                string[] data = contents.Split('/');
                if(data.Length<2) 
                    continue;
                switch (data[0])
                {
                    case "RenameStudent":
                    {
                        foreach (var student in students)
                        {
                            if (student.Key == data[1])
                            {
                                student.Value.Name = data[2];
                                Student newStudent = new Student(data[2], student.Value.House, student.Value.Points);
                                students.Remove(student.Key);
                                students.Add(data[2],newStudent);
                                break;
                            }
                        }

                        break;
                    }
                    case "ChangeHouse":
                    {
                        foreach (var student in students)
                        {
                            if (student.Key == data[1])
                            {
                                student.Value.House = (Houses) int.Parse(data[2]);
                            }
                        }
                        break;
                    }
                    case "AddStudent":
                    {
                        Student newStudent = new Student(data[1],(Houses) int.Parse(data[2]), int.Parse(data[2]));
                        students.Add(data[1],newStudent);
                        break;
                    }
                    case "RemoveStudent":
                    {
                        students.Remove(data[1]);
                        break;
                    }
                }
            }

            sr.Close();
        }


        static void Main(string[] args)
        {
            Dictionary<string,Student>students=CreateStudentsInfoFromFormat("student.csv");
            GivePoints(students);
            WinnerOfTheHouseCup(students);
            ListofHouses(students,"HousesList");
            Update(students,"change");
            SaveStudents(students,"student.csv");

        }
    }
}