using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Firestore;


namespace HymnsApp
{
    public class HymnsAttendance2 : IHymnsAttendance
    {
        private static readonly string PROJECT_ID = "hymnsapp-47352";

        private readonly FirestoreDb db;

        private readonly int NUM_STUDENT_FIELDS = 6;
        private readonly int NUM_TEACHER_FIELDS = 3;

        public readonly string[] StudentFields = { "studentName", "studentPhone", "grade", "parentName", "parentPhone", "birthday" };
        public readonly string[] TeacherFields = { "teacherName", "phone" , "birthday" };

        public static IDictionary<string, string> Classes;

        List<KeyValuePair<string, string>> studentList;
        List<KeyValuePair<string, string>> teacherList;

        // studentId -> studentSnapshot
        private readonly IDictionary<string, DocumentSnapshot> Students;
        private readonly IDictionary<string, DocumentSnapshot> Teachers;

        private string CurrentClass;

        public static string[] OrderedClasses;

        public HymnsAttendance2()
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
            unordered.Sort((a, b) => int.Parse(b.Split('.')[0]) - int.Parse(a.Split('.')[0]));
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

        public List<KeyValuePair<string, string>> StudentsOfGrade(string className)
        {
            if (className == CurrentClass)
            {
                return studentList;
            }
            CurrentClass = className;
            studentList = new List<KeyValuePair<string, string>>();
            DocumentReference classRef = db.Collection("classes").Document(Classes[className]);

            var w = classRef.GetSnapshotAsync();
            w.Wait();
            DocumentSnapshot classSnap = w.Result; // get the fields in the doc

            string[] studentIds = classSnap.GetValue<string[]>("students"); // extract the specific field that we want

            // loop through each id and get the students name from the students collection in the db.
            Students.Clear();
            for (int i = 0; i < studentIds.Length; i++)
            {
                DocumentReference student = db.Collection("students").Document(studentIds[i]);

                w = student.GetSnapshotAsync();
                w.Wait();
                DocumentSnapshot snapshot = w.Result;

                Students.Add(studentIds[i], snapshot);
                string name = snapshot.GetValue<string>("studentName");

                studentList.Add(new KeyValuePair<string, string>(studentIds[i], name));
            }

            studentList.Sort((a, b) => a.Value.CompareTo(b.Value));
            return studentList;
        }
        public void TakeAttendance(List<string> studentIds, DateTime date)
        {
            string d = date.ToString("MM/dd/yyyy");

            CurrentClass = "";  // Make sure that takeAttendance is the last call that happens!!!!!
            // 1. get the DocumentReference for each studentId
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
        }

        public string[] GetStudent(string studentId)
        {
            // studentname, studentphone, grade, parentname, parentphone, birthday
            string[] studentInfo = new string[NUM_STUDENT_FIELDS];
            DocumentSnapshot s = Students[studentId];
            for (int i = 0; i < studentInfo.Length; i++)
            {
                studentInfo[i] = s.GetValue<string>(StudentFields[i]);
            }
            return studentInfo;
        }

        public void AddStudent(string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday)
        {
            Dictionary<string, object> student = new Dictionary<string, object> 
            {
                { "studentName", studentName },
                { "studentPhone", studentPhone },
                { "grade", grade },
                { "parentName", parentName },
                { "parentPhone", parentPhone},
                { "birthday", birthday.ToString("MM/dd/yyyy").Substring(0, 5) }
            };
            // check if there is a student with this name and grade or birthday?
            var w = db.Collection("students").AddAsync(student);
            w.Wait();
            DocumentReference dr = w.Result;
            string studentId = dr.Id;
            DocumentReference middle = db.Document("classes/" + Classes[CurrentClass]);
            middle.UpdateAsync("students", FieldValue.ArrayUnion(studentId)).Wait();

            var wa = dr.GetSnapshotAsync();
            wa.Wait();
            Students.Add(studentId, wa.Result);
            studentList.Add(new KeyValuePair<string, string>(studentId, studentName));
            studentList.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        public string GetStudentPhone(string studentId)
        {
            DocumentSnapshot s = Students[studentId];
            return s.GetValue<string>("studentPhone");
        }

        public string GetStudentGrade(string studentId)
        {
            DocumentSnapshot s = Students[studentId];
            return s.GetValue<string>("grade");
        }

        public string GetParentName(string studentId)
        {
            DocumentSnapshot s = Students[studentId];
            return s.GetValue<string>("parentName");
        }

        public string GetParentPhone(string studentId)
        {
            DocumentSnapshot s = Students[studentId];
            return s.GetValue<string>("parentPhone");
        }

        public string GetBirthday(string studentId)
        {
            DocumentSnapshot s = Students[studentId];
            return s.GetValue<string>("birthday");
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

        public void RemoveStudent(string studentId)
        {
            throw new NotImplementedException();
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
                DocumentReference curClass = db.Document("classes/" + Classes[CurrentClass]);
                curClass.UpdateAsync("students", FieldValue.ArrayRemove(studentId)).Wait();

                //add student from current class
                DocumentReference newClass = db.Document("classes/" + Classes[newClassName]);
                newClass.UpdateAsync("students", FieldValue.ArrayUnion(studentId)).Wait();

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
            string week = DateTime.Now.ToString("MM/dd/yyyy").Substring(0, 5);
            string start = DateTime.Now.AddDays(-7).ToString("MM/dd/yyyy").Substring(0, 5);
            var birthdays = db.Collection("students")
                .WhereLessThanOrEqualTo("birthday", week)
                .WhereGreaterThan("birthday", start)
                .Select("studentName");

            var w = birthdays.GetSnapshotAsync();
            w.Wait();
            var res = w.Result;

            var docs = res.GetEnumerator();
            List<string> students = new List<string>();
            while (docs.MoveNext())
            {
                students.Add(docs.Current.GetValue<string>("studentName"));
            }
            return students.ToArray();
        }

        public List<KeyValuePair<string, string>> TeachersOfGrade(string className)
        {
            //if (className == CurrentClass)
            //{
            //    return teacherList;
            //}
            //CurrentClass = className;
            teacherList = new List<KeyValuePair<string, string>>();
            DocumentReference classRef = db.Collection("classes").Document(Classes[className]);

            var w = classRef.GetSnapshotAsync();
            w.Wait();
            DocumentSnapshot classSnap = w.Result; // get the fields in the doc

            string[] teacherIds = classSnap.GetValue<string[]>("teachers"); // extract the specific field that we want

            // loop through each id and get the students name from the students collection in the db.
            Teachers.Clear();
            for (int i = 0; i < teacherIds.Length; i++)
            {
                DocumentReference teacher = db.Collection("teachers").Document(teacherIds[i]);

                w = teacher.GetSnapshotAsync();
                w.Wait();
                DocumentSnapshot snapshot = w.Result;

                Teachers.Add(teacherIds[i], snapshot);
                string name = snapshot.GetValue<string>("teacherName");

                teacherList.Add(new KeyValuePair<string, string>(teacherIds[i], name));
            }

            teacherList.Sort((a, b) => a.Value.CompareTo(b.Value));
            return teacherList;
        }

        public void TakeTeacherAttendance(List<string> teacherIds, DateTime date)
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

        public string[] GetTeacher(string teacherId)
        {
            string[] teacherInfo = new string[NUM_TEACHER_FIELDS];
            DocumentSnapshot s = Teachers[teacherId];
            for (int i = 0; i < teacherInfo.Length; i++)
            {
                teacherInfo[i] = s.GetValue<string>(TeacherFields[i]);
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
                { "birthday", newBirthday.ToString("MM/dd/yyyy").Substring(0, 5)}
            };

            dr.SetAsync(update, SetOptions.MergeAll).Wait();
            var w = dr.GetSnapshotAsync();
            w.Wait();
            Teachers[teacherId] = w.Result;

            if (CurrentClass != newClassName)
            {
                //Remove student from current class
                DocumentReference curClass = db.Document("classes/" + Classes[CurrentClass]);
                curClass.UpdateAsync("teachers", FieldValue.ArrayRemove(teacherId)).Wait();

                //add student from current class
                DocumentReference newClass = db.Document("classes/" + Classes[newClassName]);
                newClass.UpdateAsync("teachers", FieldValue.ArrayUnion(teacherId)).Wait();
            }
        }
    }
}
