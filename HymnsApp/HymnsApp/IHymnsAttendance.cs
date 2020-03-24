using System;
using System.Collections.Generic;

namespace HymnsApp
{
    public interface IHymnsAttendance
    {
        /* StudentsOfGrade will always be the first method called to get the ids of students
         * Every method that follows will assume that the class is the one passed into StudentsOfGrade
         */

        // OrderedClasses, an array of all the class names
        List<KeyValuePair<string, string>> StudentsOfGrade(string className);
        // ImageOfStudent(string studentName);

        void TakeAttendance(List<string> studentIds, DateTime date);
        bool AttendedToday(string studentId, DateTime date);

        // [studentname, studentphone, grade, parentName, parentPhone, birthday, photo?]
        string[] GetStudent(string studentId);

        // Mandatory: studentName, grade, parentName, birthday
        void AddStudent(string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
        string GetStudentPhone(string studentId);
        string GetStudentGrade(string studentId);
        string GetParentName(string studentId);

        string GetParentPhone(string studentId);

        string GetBirthday(string studentId);

        int GetDatesForYear(string studentId);

        void RemoveStudent(string studentId);

        void EditStudent(string studentId, string newClassName, string newStudentName, string newStudentPhone, 
            string newGrade, string newParentName, string newParentPhone, DateTime newBirthday);

        //for students and teachers
        string[] WeeklyBirthdays();


        //Methods for teachers

        List<KeyValuePair<string, string>> teachersOfGrade(string className);

        void TakeTeacherAttendance(List<string> teacherIds, DateTime date);

        string[] GetTeacher(string teacherId);

        void EditStudent(string teacherId, string newClassName, string newTeacherName, string newTeacherPhone, DateTime newBirthday);


    }
}