using System;
using System.Collections.Generic;
using System.IO;

using Google.Cloud.Firestore;


namespace HymnsApp
{
    public class HymnsAttendance : IHymnsAttendance
    {
        private static readonly string PROJECT_ID = "hymnsapp-47352";

        private readonly FirestoreDb db;

        private readonly int NUM_STUDENT_FIELDS = 6;
        private readonly int NUM_TEACHER_FIELDS = 3;

        // studentInfo[Attendance.StudentInfo.GRADE]
        public enum StudentInfo { STUDENT_NAME, STUDENT_PHONE, GRADE, PARENT_NAME, PARENT_PHONE, BIRTHDAY };
        private readonly string[] StudentFields = { "studentName", "studentPhone", "grade", "parentName", "parentPhone", "birthday" };

        public enum TeacherInfo { TEACHER_NAME, TEACHER_PHONE, BIRTHDAY };
        private readonly string[] TeacherFields = { "teacherName", "teacherPhone", "birthday" };

        private readonly IDictionary<string, string> Classes;

        private List<KeyValuePair<string, string>> studentList;
        private List<KeyValuePair<string, string>> teacherList;

        // studentId -> studentSnapshot
        private readonly IDictionary<string, DocumentSnapshot> Students;
        private readonly IDictionary<string, DocumentSnapshot> Teachers;

        private string CurrentClass;

        public static string[] OrderedClasses;

        public HymnsAttendance()
        {
            Students = new Dictionary<string, DocumentSnapshot>();
            Teachers = new Dictionary<string, DocumentSnapshot>();

            Classes = new Dictionary<string, string>();

            SetEnvironmentVariables();
            db = FirestoreDb.Create(PROJECT_ID);
            GetClasses();
        }

        private void GetClasses()
        {
            var docs = db.Collection("classes").ListDocumentsAsync().GetEnumerator();
            List<string> unordered = new List<string>();

            while (true)
            {
                var n = docs.MoveNext();
                n.Wait();
                if (!n.Result)
                {
                    break;
                }
                string cur = docs.Current.Id;
                Classes.Add(cur.Split('.')[1], cur);
                unordered.Add(cur);
            }
            unordered.Sort((a, b) => int.Parse(a.Split('.')[0]) - int.Parse(b.Split('.')[0]));
            OrderedClasses = unordered.ConvertAll(a => a.Split('.')[1]).ToArray();
        }
        private void SetEnvironmentVariables()
        {
            string jsonCred =
                @"{  
                'type': 'service_account', 'project_id': 'hymnsapp-47352',
                'private_key_id': 'c77f0a0bf42582ab067216779bf21c7ff44746f8',
                'private_key': '-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCsVqhSIp8YRzov\nqdLuSr61fGbPxdHYkogtSaqRzLkZ2c+Z4jK0/Tq+gEyPwU65imVsM6tmoAojoBMa\nNYzj+V6kevUU49bJS0SbT2s9xHsoUeC4Eouh9k89euYSKOMUJY1WJ/WzW+YhiT4j\n2RBadxv7WW6Pj9zwXP4kIG9KSBsj3cAyy1QTxJOp60+FUQ0tblOcXiwZeJ14Vd8o\nvUgxW63+rotpcDHI5y9tK8G+TFw1z6Ktbtvf1+YqMoQrDqd+MXfOY0XRwZp+etkR\npw4BUaWB8v0iyDzCtTTARYeB0YuEWXRxkB7yEOdUFkkwJ7LlqdxWpOtPHsZyyifh\n18E6mjaDAgMBAAECggEAOfJZXmStwegRNH2RovYW8ccGes41JPEAQNoINEX15y9J\nkqBwFFMhMXTKSXviEJjsSVmGRFeDkT79rU5cNTtgC0Ycqg2B5uUcCGzHzlkGYCRH\nvxzdPnQnaLCuM2T0FY4a2+FLyCcaViRjdIJD8WcjTXxgpHzm+RsLiPIu+XcNSR11\nhekVHqCnD1H8nnzub1W7oifSfmOYP9X8QqbTbMSzAjAnZm9vWPYqTUv9T4pReHKp\nX7Nn7FQoC+uqfgtGPTFVCnBB+QnWSz3ieNIpB0Mwhry+sW40fxeP/3dOeF7mgtcR\noh5Q8bM5x1daYnKDRaOnWScniDjISpgkvi5lHPnO4QKBgQDS09HRFSUl1qfqXs+W\n24M2fErRne7p9L29AZ2QxvizzHE9Da2K2Q2Jqgq9pavFuP6uVcPoFY21FtYJRuBs\nNLG5rCyWQ6nEfIhSWaBPXn0K0vzbd4WCWAJax4jP4ru03+BisRucDlWr8017q3wv\nI7UXZ7i84JVrz625od6HymZu5QKBgQDRQ6q1+ZVfO/JaaRQP/cKmoSyIRpXAq+dc\n1heDg5JVDo+J2w252Gl6mS/bHZqU4B6tN4B7zysNho/wq+K3K0yB6XWZvu9Gow4E\nJ6N26s8fkEbg9VKNR6j2y0BvgTCGXpxtVEZgx+TryuhJ/SedUGW5ldutQQoRgGSy\nw+kDxf9RRwKBgFweqeNN9flenehOnS4xpFe+X4LQG4Cmq/FWL17/UdReVGx0+Ytd\n5AhvAFp1dWTjvIS4fO1/3XdvDv7mtVEShUW38ZOG5Tsxnu88skt74e9E2a+bvf6C\nLiU0YpdTXuldmVIGSSYbLVwA565N8+k1FP+xFouRJjkBectO60kyKyfxAoGAXi39\nC28rSbQaC5SVAelsDEAnYaGazh7Pvplf9cFmPz+RXDKpB8YdGp8MyamWI4CGbeC3\nw4DmWG0CJQfGvjcPdLxUQACNgZXqvfX0/JUK6KKvM5lVMN5abc/lzQkwhJjZ/95Y\n2j+8iF5Gkt4LUSKY1aBbQKsv09ab+1Rub1NR6AsCgYEAiaJLHmV5o4be5JlSMYUP\nQAupHQlajaAzBrhPgP1YEBC36/lbtgmw/HjD4CxxVYiHnfECISDk/DjZILyX72tW\nd+XpUN23ILX/kqUNAZKZXJrHp6ivLKHFO4yfok1dBf7Wv3TfPDJgZgRzw278IA0r\n8XcHzVxE6znzDksOV8IOjEw=\n-----END PRIVATE KEY-----\n',
                'client_email': 'connecthymnsapp@hymnsapp-47352.iam.gserviceaccount.com',
                'client_id': '110366193664730016820',
                'auth_uri': 'https://accounts.google.com/o/oauth2/auth',
                'token_uri': 'https://oauth2.googleapis.com/token',
                'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs',
                'client_x509_cert_url': 'https://www.googleapis.com/robot/v1/metadata/x509/connecthymnsapp%40hymnsapp-47352.iam.gserviceaccount.com' 
            }";

            // connect to database
            var fileName = @"HymnsApp-c77f0a0bf425.json";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, fileName);

            if (!File.Exists(path))
            {
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.WriteLine(jsonCred.Replace('\'', '\"'));
                }
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }

        public IList<KeyValuePair<string, string>> StudentsOfGrade(string className)
        {
            // if the class has not changed return the same one as before
            if (className == CurrentClass)
            {
                return studentList;
            }

            // We have a new class so reset the global student variables
            CurrentClass = className;
            studentList = new List<KeyValuePair<string, string>>();  // has to be a new list since someone else has access
            Students.Clear();

            // classes -> {students}, {teachers}
            DocumentReference classRef = db.Collection("classes").Document(Classes[className]);

            var w = classRef.GetSnapshotAsync();
            w.Wait();
            DocumentSnapshot classSnap = w.Result; // get the fields in the doc

            string[] studentIds = classSnap.GetValue<string[]>("students"); // extract the specific field that we want

            // loop through each id and get the students name from the students collection in the db.
            foreach (string studentId in studentIds)
            {
                DocumentReference student = db.Collection("students").Document(studentId);

                w = student.GetSnapshotAsync();
                w.Wait();
                DocumentSnapshot snapshot = w.Result;

                Students.Add(studentId, snapshot);
                string name = snapshot.GetValue<string>("studentName");

                studentList.Add(new KeyValuePair<string, string>(studentId, name));
            }

            studentList.Sort((a, b) => a.Value.CompareTo(b.Value));
            return studentList;
        }

        public void TakeAttendance(IList<string> studentIds, DateTime date)
        {
            string d = date.ToString("MM/dd/yyyy");

            // Make sure that takeAttendance is the last call that happens!!!!!

            // go through all students in this class
            // if they are in the list, add their attendance
            // if not, remove their attendance
            foreach (string studentId in Students.Keys)
            {
                DocumentReference dr = db.Collection("students").Document(studentId);

                DocumentSnapshot snapshot = Students[studentId];

                try
                {
                    snapshot.GetValue<string[]>("attended");
                    if (studentIds.Contains(studentId))
                    {
                        dr.UpdateAsync("attended", FieldValue.ArrayUnion(d)).Wait();
                    }
                    else
                    {
                        dr.UpdateAsync("attended", FieldValue.ArrayRemove(d)).Wait();
                    }
                }
                catch
                {
                    if (studentIds.Contains(studentId))
                    {
                        Dictionary<string, object> update = new Dictionary<string, object>
                    {
                        { "attended", new string[] { d } }
                    };
                        dr.SetAsync(update, SetOptions.MergeAll).Wait();
                    }
                }
            }
            CurrentClass = "";
        }

        public string[] GetStudentInfo(string studentId)
        {
            // studentname, studentphone, grade, parentname, parentphone, birthday
            string[] studentInfo = new string[NUM_STUDENT_FIELDS];
            DocumentSnapshot s = Students[studentId];
            for (int i = 0; i < studentInfo.Length; i++)
            {
                try
                {
                    studentInfo[i] = s.GetValue<string>(StudentFields[i]);
                }
                catch
                {
                    studentInfo[i] = "";
                }
            }
            return studentInfo;
        }

        public void AddStudent(string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday)
        {
            // make sure that no fields are null
            studentName = studentName ?? "";
            studentPhone = studentPhone ?? "";
            grade = grade ?? "";
            parentName = parentName ?? "";
            parentPhone = parentPhone ?? "";

            string bday = birthday.ToString("MM/dd/yyyy").Substring(0, 5);

            Dictionary<string, object> student = new Dictionary<string, object>
            {
                { "studentName", studentName },
                { "studentPhone", studentPhone },
                { "grade", grade },
                { "parentName", parentName },
                { "parentPhone", parentPhone},
                { "birthday", bday }
            };

            // check if there is a student with this name and birthday
            var q = db.Collection("students")
                .WhereEqualTo("studentName", studentName)
                .WhereEqualTo("birthday", bday)
                .GetSnapshotAsync();

            q.Wait();
            if (q.Result.Count != 0)
            {
                return;
            }

            // add the student
            var w = db.Collection("students").AddAsync(student);
            w.Wait();
            DocumentReference dr = w.Result;
            string studentId = dr.Id;

            // add them to their class
            DocumentReference classDoc = db.Collection("classes").Document(Classes[CurrentClass]);
            try
            {
                classDoc.UpdateAsync("students", FieldValue.ArrayUnion(studentId)).Wait();
            }
            catch
            {
                Dictionary<string, object> fields = new Dictionary<string, object>
                {
                    { "students", new string[] { studentId } }
                };
                classDoc.SetAsync(fields).Wait();
            }


            var wa = dr.GetSnapshotAsync();
            wa.Wait();

            // Update students lists
            Students.Add(studentId, wa.Result);
            studentList.Add(new KeyValuePair<string, string>(studentId, studentName));
            studentList.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        public int GetDatesForYear(string studentId)
        {
            int year = DateTime.Now.Year;
            DocumentSnapshot s = Students[studentId];

            try
            {
                string[] attended = s.GetValue<string[]>("attended");
                return new List<string>(attended).FindAll(a => int.Parse(a.Split('/')[2]) == year).Count;
            }
            catch
            {
                return 0;
            }
        }

        public void EditStudent(string studentId, string newClassName, string newStudentName, string newStudentPhone, string newGrade,
                                string newParentName, string newParentPhone, DateTime newBirthday)
        {
            DocumentReference dr = db.Collection("students").Document(studentId);

            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "studentName", newStudentName},
                { "studentPhone", newStudentPhone },
                { "grade", newGrade },
                { "parentName", newParentName },
                { "parentPhone", newParentPhone},
                { "birthday", newBirthday.ToString("MM/dd/yyyy").Substring(0, 5) }
            };

            dr.SetAsync(update, SetOptions.MergeAll).Wait();
            var w = dr.GetSnapshotAsync();
            w.Wait();
            Students[studentId] = w.Result;

            if (CurrentClass != newClassName)
            {
                //Remove student from current class
                DocumentReference curClass = db.Collection("classes").Document(Classes[CurrentClass]);
                curClass.UpdateAsync("students", FieldValue.ArrayRemove(studentId)).Wait();

                //add student to new class
                DocumentReference newClass = db.Collection("classes").Document(Classes[newClassName]);
                try
                {
                    newClass.UpdateAsync("students", FieldValue.ArrayUnion(studentId)).Wait();
                }
                catch
                {
                    Dictionary<string, object> fields = new Dictionary<string, object>
                    {
                        { "students", new string[] { studentId } }
                    };
                    newClass.SetAsync(fields).Wait();
                }
            }
        }

        public bool AttendedToday(string studentId, DateTime date)
        {
            string d = date.ToString("MM/dd/yyyy");

            DocumentSnapshot s = Students[studentId];
            try
            {
                string[] attended = s.GetValue<string[]>("attended");

                foreach (string a in attended)
                {
                    if (a == d) return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string[] WeeklyBirthdays()
        {
            string today = DateTime.Now.ToString("MM/dd/yyyy").Substring(0, 5);
            string start = DateTime.Now.AddDays(-7).ToString("MM/dd/yyyy").Substring(0, 5);
            var birthdays = db.Collection("students")
                .WhereLessThanOrEqualTo("birthday", today)
                .WhereGreaterThan("birthday", start)
                .Select("studentName");

            var w = birthdays.GetSnapshotAsync();
            w.Wait();
            var docs = w.Result.GetEnumerator();

            List<string> students = new List<string>();
            while (docs.MoveNext())
            {
                students.Add(docs.Current.GetValue<string>("studentName"));
            }
            return students.ToArray();
        }

        public IList<KeyValuePair<string, string>> TeachersOfGrade(string className)
        {
            // reset teacher lists
            teacherList = new List<KeyValuePair<string, string>>();
            Teachers.Clear();
            DocumentReference classRef = db.Collection("classes").Document(Classes[className]);

            var w = classRef.GetSnapshotAsync();
            w.Wait();
            DocumentSnapshot classSnap = w.Result; // get the fields in the doc

            string[] teacherIds = classSnap.GetValue<string[]>("teachers"); // extract the specific field that we want

            // get teacher names for each id
            foreach (string teacherId in teacherIds)
            {
                DocumentReference teacher = db.Collection("teachers").Document(teacherId);

                w = teacher.GetSnapshotAsync();
                w.Wait();
                DocumentSnapshot snapshot = w.Result;

                Teachers.Add(teacherId, snapshot);
                string name = snapshot.GetValue<string>("teacherName");

                teacherList.Add(new KeyValuePair<string, string>(teacherId, name));
            }

            teacherList.Sort((a, b) => a.Value.CompareTo(b.Value));
            return teacherList;
        }

        public void TakeTeacherAttendance(IList<string> teacherIds, DateTime date)
        {
            string d = date.ToString("MM/dd/yyyy");
            foreach (string teacherId in Teachers.Keys)
            {
                DocumentReference dr = db.Collection("teachers").Document(teacherId);

                DocumentSnapshot snapshot = Teachers[teacherId];

                try
                {
                    snapshot.GetValue<string[]>("attended");
                    if (teacherIds.Contains(teacherId))
                    {
                        dr.UpdateAsync("attended", FieldValue.ArrayUnion(d)).Wait();
                    }
                    else
                    {
                        dr.UpdateAsync("attended", FieldValue.ArrayRemove(d)).Wait();
                    }
                }
                catch
                {
                    if (teacherIds.Contains(teacherId))
                    {
                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "attended", new string[] { d } }
                        };
                        dr.SetAsync(update, SetOptions.MergeAll).Wait();
                    }
                }
            }
        }

        public int TeacherGetDatesForYear(string teacherId)
        {
            int year = DateTime.Now.Year;
            DocumentSnapshot s = Teachers[teacherId];

            try
            {
                string[] attended = s.GetValue<string[]>("attended");
                return new List<string>(attended).FindAll(a => int.Parse(a.Split('/')[2]) == year).Count;
            }
            catch
            {
                return 0;
            }
        }

        public bool TeacherAttendedToday(string teacherId, DateTime date)
        {
            string d = date.ToString("MM/dd/yyyy");

            DocumentSnapshot s = Teachers[teacherId];
            try
            {
                string[] attended = s.GetValue<string[]>("attended");

                foreach (string a in attended)
                {
                    if (a == d) return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public string[] GetTeacherInfo(string teacherId)
        {
            string[] teacherInfo = new string[NUM_TEACHER_FIELDS];
            DocumentSnapshot s = Teachers[teacherId];
            for (int i = 0; i < teacherInfo.Length; i++)
            {
                try
                {
                    teacherInfo[i] = s.GetValue<string>(TeacherFields[i]);
                }
                catch
                {
                    teacherInfo[i] = "";
                }
            }

            return teacherInfo;
        }

        public void EditTeacher(string teacherId, string newClassName, string newTeacherName, string newTeacherPhone, DateTime newBirthday)
        {
            DocumentReference dr = db.Collection("teachers").Document(teacherId);

            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "teacherName", newTeacherName},
                { "phone", newTeacherPhone },
                { "birthday", newBirthday.ToString("MM/dd/yyyy").Substring(0, 5) }
            };

            dr.SetAsync(update, SetOptions.MergeAll).Wait();
            var w = dr.GetSnapshotAsync();
            w.Wait();
            Teachers[teacherId] = w.Result;

            if (CurrentClass != newClassName)
            {
                //Remove student from current class
                DocumentReference curClass = db.Collection("classes").Document(Classes[CurrentClass]);
                curClass.UpdateAsync("teachers", FieldValue.ArrayRemove(teacherId)).Wait();

                //add student from current class
                DocumentReference newClass = db.Collection("classes").Document(Classes[newClassName]);
                try
                {
                    newClass.UpdateAsync("teachers", FieldValue.ArrayUnion(teacherId)).Wait();
                }
                catch
                {
                    Dictionary<string, object> t = new Dictionary<string, object>
                    {
                        { "teachers", new string[] { teacherId } }
                    };
                    newClass.SetAsync(t, SetOptions.MergeAll).Wait();
                }
            }
        }

        public void AddTeacher(string teacherName, string teacherPhone, DateTime birthday)
        {
            teacherName = teacherName ?? "";
            teacherPhone = teacherPhone ?? "";

            string bday = birthday.ToString("MM/dd/yyyy").Substring(0, 5);
            Dictionary<string, object> teacher = new Dictionary<string, object>
            {
                { "teacherName", teacherName },
                { "phone", teacherPhone },
                { "birthday", bday }
            };

            // check if there is a teacher with this name and birthday?
            var w = db.Collection("teachers").AddAsync(teacher);
            w.Wait();
            DocumentReference dr = w.Result;
            string teacherId = dr.Id;
            DocumentReference classDoc = db.Collection("classes").Document(Classes[CurrentClass]);

            try
            {
                classDoc.UpdateAsync("teachers", FieldValue.ArrayUnion(teacherId)).Wait();
            }
            catch
            {
                Dictionary<string, object> t = new Dictionary<string, object>
                {
                    { "teachers", new string[] { teacherId } }
                };
                classDoc.SetAsync(t, SetOptions.MergeAll).Wait();
            }

            var wa = dr.GetSnapshotAsync();
            wa.Wait();
            Teachers.Add(teacherId, wa.Result);
            teacherList.Add(new KeyValuePair<string, string>(teacherId, teacherName));
            teacherList.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        public Stream GetStudentPhoto(string studentId)
        {
            var student = Students[studentId];
            if (student.ContainsField("photo"))
                return new MemoryStream(student.GetValue<byte[]>("photo"));
            else
                return null;
        }

        public void AddStudentPhoto(string studentId, Stream photo)
        {
            
            Dictionary<string, object> s = new Dictionary<string, object>();
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                s.Add("photo", memoryStream.ToArray());
                db.Collection("students").Document(studentId).SetAsync(s, SetOptions.MergeAll).Wait();
            }
            
        }

        public Stream GetTeacherPhoto(string teacherId)
        {
            var teacher = Teachers[teacherId];
            if (teacher.ContainsField("photo"))
                return new MemoryStream(teacher.GetValue<byte[]>("photo"));
            else
                return null;
        }

        public void AddTeacherPhoto(string teacherId, Stream photo)
        {
            Dictionary<string, object> t = new Dictionary<string, object>();
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                t.Add("photo", memoryStream.ToArray());
                db.Collection("teachers").Document(teacherId).SetAsync(t, SetOptions.MergeAll).Wait();
            }
            
        }


        public string[][] GetCurriculum()
        {
            var enumerator = db.Collection("curriculum").ListDocumentsAsync().GetEnumerator();

            List<string[]> curriculum = new List<string[]>();
            
            while(true)
            {
                var n = enumerator.MoveNext();
                n.Wait();

                if (!n.Result)
                {
                    break;
                }
                var w = enumerator.Current.GetSnapshotAsync();
                w.Wait();
                curriculum.Add(w.Result.GetValue<string[]>("hymns"));

            }
            return curriculum.ToArray();
        }
    }
}
