using System;
using System.Collections.Generic;
using System.IO;

namespace HymnsApp
{
    public interface IHymnsAttendance
    {
        /* StudentsOfGrade will always be the first method called to get the ids of students
         * Every method that follows will assume that the class is the one passed into StudentsOfGrade
         */

        // OrderedClasses, an array of all the class names

        /// <summary>
        /// Retrieves a list of the students in a given class.
        /// Use the studentId to access and update the rest of the information for the student.
        /// </summary>
        /// <param name="className">The name of the class to get students for</param>
        /// <returns>A list of pairs (studentId, studentName)</returns>
        IList<KeyValuePair<string, string>> StudentsOfGrade(string className);
        // ImageOfStudent(string studentName);

        /// <summary>
        /// Takes attendance for a given list of students on a given date.
        /// Any students in the same class that are not included will be marked absent.
        /// </summary>
        /// <param name="studentIds">The students that attended</param>
        /// <param name="date">The date of attendance</param>
        void TakeAttendance(IList<string> studentIds, DateTime date);
        
        /// <summary>
        /// Checks if a student attended on a given date
        /// </summary>
        /// <param name="studentId">The id of the student to check</param>
        /// <param name="date">The date to check attendance on</param>
        /// <returns>Whether the student attended on the date</returns>
        bool AttendedToday(string studentId, DateTime date);

        /// <summary>
        /// Retrieves student information for the given student.
        /// This information includes: studentName, studentPhone, grade, parentName, parentPhone, birthday
        /// Use the StudentInfo enum to access different student fields
        /// </summary>
        /// <param name="studentId">The id of the student to get information for</param>
        /// <returns>An array of student information</returns>
        string[] GetStudentInfo(string studentId);

        // Mandatory: studentName, grade, parentName, birthday
        /// <summary>
        /// Adds a student to the current class.
        /// Important: If an optional field is not present, replace it wil ""
        /// </summary>
        /// <param name="studentName">(Mandatory) The student's name</param>
        /// <param name="studentPhone">The student's phone number</param>
        /// <param name="grade">(Mandatory) The student's grade</param>
        /// <param name="parentName">(Mandatory) The parent's name</param>
        /// <param name="parentPhone">The parent's phone</param>
        /// <param name="birthday">(Mandatory) The student's birthday</param>
        void AddStudent(string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday);

        /// <summary>
        /// Gets the number of days a student has attended this year.
        /// Uses the current calendar year.
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <returns>The number of days the student attended this year</returns>
        int GetDatesForYear(string studentId);

        //void RemoveStudent(string studentId);

        /// <summary>
        /// Edits the student information.
        /// If a field is different it will be changed.
        /// If a field has not been changed pass in the previous value.
        /// </summary>
        /// <param name="studentId">The id of the student to be editted</param>
        /// <param name="newClassName">The student's class</param>
        /// <param name="newStudentName">The student's name</param>
        /// <param name="newStudentPhone">The student's phone number</param>
        /// <param name="newGrade">The student's grade</param>
        /// <param name="newParentName">The parent's name</param>
        /// <param name="newParentPhone">The parent's phone</param>
        /// <param name="newBirthday">The student's birthday</param>
        void EditStudent(string studentId, string newClassName, string newStudentName, string newStudentPhone, 
            string newGrade, string newParentName, string newParentPhone, DateTime newBirthday);

        /// <summary>
        /// Gets the names of students who had birthdays in the past week.
        /// Birthdays are within [6 days ago, today]
        /// </summary>
        /// <returns>The names of the students with birthdays this week</returns>
        string[] WeeklyBirthdays();


        /// <summary>
        /// Retrieves a list of the teachers for a given class.
        /// Use the teacherId to access and update the rest of the information for the teacher.
        /// </summary>
        /// <param name="className">The class to get teachers for</param>
        /// <returns>A list of pairs (teacherId, teacherName)</returns>
        IList<KeyValuePair<string, string>> TeachersOfGrade(string className);

        /// <summary>
        /// Takes attendance for a given list of teachers on a given date.
        /// Any teachers in the same class that are not included will be marked absent.
        /// </summary>
        /// <param name="teacherIds">The teachers that attended</param>
        /// <param name="date">The date of attendance</param>
        void TakeTeacherAttendance(IList<string> teacherIds, DateTime date);

        /// <summary>
        /// Checks if a teachehr attended on a given date
        /// </summary>
        /// <param name="teacherId">The id of the teacher to check</param>
        /// <param name="date">The date to check attendance on</param>
        /// <returns>Whether the teacher attended on the date</returns>
        bool TeacherAttendedToday(string teacherId, DateTime date);

        /// <summary>
        /// Retrieves teacher information for the given teacher.
        /// This information includes: teacherName, teacherPhone, birthday
        /// Use the TeacherInfoIndices enum to access different teacher fields
        /// </summary>
        /// <param name="teacherId">The id of the teacher to get information for</param>
        /// <returns>The information of the teacher</returns>
        string[] GetTeacherInfo(string teacherId);

        /// <summary>
        /// Edits the teacher information.
        /// If a field is different it will be changed.
        /// If a field has not been changed pass in the previous value.
        /// </summary>
        /// <param name="teacherId">The id of the teacher to edit</param>
        /// <param name="newClassName">The teacher's class</param>
        /// <param name="newTeacherName">The teacher's name</param>
        /// <param name="newTeacherPhone">The teacher's phone number</param>
        /// <param name="newBirthday">The teacher's birthday</param>
        void EditTeacher(string teacherId, string newClassName, string newTeacherName, string newTeacherPhone, DateTime newBirthday);

        /// <summary>
        /// Adds a teacher to the current class.
        /// Important: If an optional field is not present, replace it wil ""
        /// </summary>
        /// <param name="teacherName">The teacher's name</param>
        /// <param name="teacherPhone">The teacher's phone number</param>
        /// <param name="birthday">The teacher's birthday</param>
        void AddTeacher(string teacherName, string teacherPhone, DateTime birthday);

        /// <summary>
        /// Returns a student photo using the student ID.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        Stream GetStudentPhoto(string studentId);

        /// <summary>
        /// adds a student photo to the the studentId
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="photo"></param>
        void AddStudentPhoto(string studentId, Stream photo);

        /// <summary>
        /// Returns a teacher photo using teacherId
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        Stream GetTeacherPhoto(string teacherId);

        /// <summary>
        /// adds a teacher photo to the teacherId
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="photo"></param>
        void AddTeacherPhoto(string teacherId, Stream photo);

       /// <summary>
       /// returns a 2D array of the curriculums, from kindergarten to middle school
       /// </summary>
       /// <returns></returns>
        string[][] GetCurriculum();
    }
}