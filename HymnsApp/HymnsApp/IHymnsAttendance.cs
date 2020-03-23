using System;
using System.Collections.Generic;

namespace HymnsApp
{
    public interface IHymnsAttendance
    {
        /* StudentsOfGrade will always be the first method called to get the ids of students
         * Every method that follows will assume that the class is the one passed into StudentsOfGrade
         */
        List<KeyValuePair<string, string>> StudentsOfGrade(string className);
        // ImageOfStudent(string studentName);

        void TakeAttendance(List<string> studentIds, DateTime date);
        bool AttendedToday(string studentId, DateTime date);

        // [studentname, studentphone, grade, parentName, parentPhone, birthday, photo?]
        string[] GetStudent(string studentId);
        void AddStudent(string studentName, string studentPhone, int grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
        string GetStudentPhone(string studentId);
        int GetStudentGrade(string studentId);
        string GetParentName(string studentId);
        string GetParentPhone(string studentId);

        string GetBirthday(string studentId);

        string GetDatesForYear(string studentId);

        void RemoveStudent(string studentId);
        void EditStudent(string studentId, string newClassName, string newStudentPhone, 
            int newGrade, string newParentName, string newParentPhone, DateTime newBirthday);

        string[] WeeklyBirthdays();
    }
}