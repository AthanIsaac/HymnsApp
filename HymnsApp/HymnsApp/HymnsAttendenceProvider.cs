using System;
using System.Collections.Generic;
using System.IO;

namespace HymnsApp
{
    //class MainClass
    //{
    //    public static void Main(string[] args)
    //    {
    //        HymnsAttendance attendance = new HymnsAttendance();

    //        Console.WriteLine("Enter your grade ");
    //        int G;
    //        Int32.TryParse(Console.ReadLine(), out G);
    //        attendance.StudentsNames(G).ForEach((obj) => Console.WriteLine(obj));
    //        Console.WriteLine("Who showed up today to class?");
    //        List<string> students = new List<string>();

    //        string s = Console.ReadLine();
    //        while(s != "")
    //        {
    //            students.Add(s);
    //            s = Console.ReadLine();
    //        }

    //        attendance.attendedToday(students);
    //        attendance.students.ForEach((obj) => Console.WriteLine(obj));
    //    }
    //}



    // think about exposing internal representation to users for student and date?????????????????

    internal class Student
    {
        public string Name 
        {
            get; set;
        }
        public int Grade
        {
            get; set;
        }
        public string Phone
        {
            get; set;
        }

        readonly HashSet<Date> Attendance;

        public Student(string name, int grade, string phone, HashSet<Date> attendance)
        {
            Name = name;
            Grade = grade;
            Phone = phone; // can be empty
            Attendance = attendance;
        }

        public void AddDate(int year, int month, int day)
        {
            Date date = new Date(month, day, year);
            Attendance.Add(date);
        }
        public void AddDate(Date date)
        {
            Attendance.Add(date);
        }

        public bool HasDate(Date date)
        {
            return Attendance.Contains(date);
        }

        public void RemoveDate(Date date)
        {
            Attendance.Remove(date);
        }

        public int DatesCount(int year)
        {
            int count = 0;
            foreach (Date d in Attendance)
            {
                if (d.year == year)
                {
                    count++;
                }
            }
            return count;
        }

        public override string ToString()
        {
            string s = Name + "," + Grade + "," + Phone + ",";
            if (Attendance.Count == 0)
            {
                return s;
            }

            foreach (Date d in Attendance)
            {
                s += d + ";";
            }

            return s.Remove(s.Length - 1); // remove extra ; on the last date
        }
    }


    internal class Date
    {
        public int year;
        public int month;
        public int day;
        

        public Date(int m, int d, int y)
        {
            year = y; month = m; day = d;
        }

        public Date(string date) // mm/dd/yy
        {
            month = int.Parse("" + date[0] + date[1]);
            day = int.Parse("" + date[3] + date[4]);
            year = int.Parse("" + date[6] + date[7]);
        }

        public override string ToString()
        {
            if (month < 10 && day < 10)
                return "0" + month + "/0" + day + "/" + year;

            if (month < 10)
                return "0" + month + "/" + day + "/" + year;

            if (day < 10)
                return "" + month + "/0" + day + "/" + year;

            return "" + month + "/" + day + "/" + year;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is Date other &&
                day == other.day &&
                month == other.month &&
                year == other.year;
        }

        public override int GetHashCode()
        {
            return day * 37 * 71 * 11 * month * year;
        }
    }

    public class HymnsAttendance
    {
        private readonly List<Student> Students;
        private readonly string Path;
        public static string[] Classes = { 
            "Kindergarten",
            "1st Grade",
            "2nd Grade",
            "3rd Grade",
            "4th Grade",
            "5th Grade", 
            "6th Grade",
            "7th Grade",
            "8th Grade",
            "9th Grade",
            "10th Grade",
            "11th Grade",
            "12th Grade"
        };

        public HymnsAttendance(string path)
        {
            Students = new List<Student>();
            Path = path;
            ReadStudents();
        }

        /**
         * file format:
         * [name],[grade],[phone#],[d1;d2;...;dn;]
         * Update needed here
         **/
        private void ReadStudents()
        {
            if (!File.Exists(Path))
            {
                string fakefile = 
                    "mena girgis,7,,\n" +
                    "nabu bruk,7,,\n" +
                    "youssef girgis,7,,\n" +
                    "pheobe hanna,7,,\n" +
                    "elaria hanna,7,,\n" +
                    "mena habib,7,,\n" +
                    "tony shafik,7,,\n" +
                    "jonathan bedair,7,,\n" +
                    "sandra wagdy,7,,";

                fakefile = fakefile.ToLower();
                using (StreamWriter file = new StreamWriter(Path))
                {
                    foreach (string s in fakefile.Split('\n'))
                    {
                        file.WriteLine(s);
                    }
                }
            }
            using (TextReader reader = new StreamReader(Path))
            {
                string l = reader.ReadLine();
                while (l != null)
                {
                    string[] studentInfo = l.Split(',');
                    HashSet<Date> dates;
                    if (studentInfo[3] == "") // if there are no dates for a student
                    {
                        dates = new HashSet<Date>();
                    }
                    else
                    {
                        dates = new HashSet<Date>(new List<string>(studentInfo[3].Split(';'))
                                    .ConvertAll((input) => new Date(input)));
                    }
                    Students.Add(new Student(
                        studentInfo[0],
                        int.Parse(studentInfo[1]),
                        studentInfo[2],
                        dates));

                    l = reader.ReadLine();
                }
            }
        }
        public void WriteStudents()
        {
            // this method should override the file that was read from (replacing the previous content) 
            // with the content of 'students'.
            // format should be:
            // [name],[grade],[phone#],[d1;d2;...;dn;]
            // WOW!!!!! format can be applied by calling the tostring of the student!!!
            using (StreamWriter file = new StreamWriter(Path))
            {
                foreach (Student s in Students)
                {
                    file.WriteLine(s.ToString());
                }
            }
        }

        public List<string> StudentsOfGrade(int grade)
        {
            List<string> students = new List<string>();
            foreach (Student s in Students)
            {
                if (s.Grade == grade)
                {
                    students.Add(s.Name);
                }
            }
            return students;
        }

        public void NewGrade()
        {
            Students.ForEach((obj) => ++obj.Grade);
        }

        public void TakeAttendance(List<string> names, int grade)
        {
            Date today = new Date(DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year % 100);
            List<Student> inGrade = Students.FindAll(s => s.Grade == grade);
            foreach (Student s in inGrade) 
            {
                if (names.Contains(s.Name))
                {
                    s.AddDate(today);
                }
                else
                {
                    s.RemoveDate(today);
                }
            }
        }

        public void TakeAttendance(List<string> names, int grade, DateTime date)
        {
            Date today = new Date(date.Month, date.Day, date.Year % 100);
            List<Student> inGrade = Students.FindAll(s => s.Grade == grade);
            foreach (Student s in inGrade)
            {
                if (names.Contains(s.Name))
                {
                    s.AddDate(today);
                }
                else
                {
                    s.RemoveDate(today);
                }
            }
        }

        public bool AttendedToday(string student)
        {
            Date today = new Date(DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year % 100);
            return Students.Exists(s => s.Name == student && s.HasDate(today));
        }

        public bool AttendedToday(string student, DateTime date)
        {
            Date today = new Date(date.Month, date.Day, date.Year % 100);
            return Students.Exists(s => s.Name == student && s.HasDate(today));
        }

        public void AddSudent(string name, string number, int grade)
        {
            Students.Add(new Student(name.ToLower().Trim(), grade, number, new HashSet<Date>()));
        }

        public string GetNumber(string name, int grade)
        {
            return Students.Find(s => s.Name == name && s.Grade == grade).Phone;
        }
        public string GetDatesForYear(string name, int grade)
        {
            return Students.Find(s => s.Name == name && s.Grade == grade).DatesCount(DateTime.Now.Year % 100).ToString();
        }

        public void RemoveStudent(string name, int grade)
        {
            Students.Remove(Students.Find(s => s.Name == name && s.Grade == grade));
        }
        public void EditStudent(string oldname, int oldgrade, string newname, string newphone, int newgrade)
        {
            Student student = Students.Find(s => s.Name == oldname && s.Grade == oldgrade);
            student.Name = newname.ToLower().Trim();
            student.Grade = newgrade;
            student.Phone = newphone ?? "";
        }
    }
}
