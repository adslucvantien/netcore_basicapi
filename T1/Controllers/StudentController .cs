using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T1.Models;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace T1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {

        [HttpGet("all")]
        public async Task<IEnumerable<Student>> GetStudents()
        {
            var students = new List<Student>();
            string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = "SELECT * FROM Student;";

                using (var command = new SqlCommand(commandText, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var student = new Student
                            {
                                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age"))
                            };

                            students.Add(student);
                        }
                    }
                }
            }

            return students;

            
        }



        [HttpGet("{studentId}")]
        public async Task<Student> GetStudent(int studentId)
        {
            Student student = null;
            string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = "SELECT * FROM Student WHERE StudentId = @StudentId;";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@StudentId", studentId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            student = new Student
                            {
                                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age"))
                            };
                        }
                    }
                }
            }

            return student;
        }



        [HttpGet("byCourse/{courseId}")]
        public async Task<IEnumerable<Student>> GetStudentsByCourse(int courseId)
        {
            var students = new List<Student>();
            string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = "SELECT s.* FROM Student s INNER JOIN StudentCourse sc ON s.StudentId = sc.StudentId WHERE sc.CourseId = @CourseId;";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", courseId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var student = new Student
                            {
                                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age"))
                            };

                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }


        [HttpPost("insert")]
        public async Task<IActionResult> InsertStudent(Student student)
        {
            string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = "INSERT INTO Student (Name, Age) VALUES (@Name, @Age);";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Name", student.Name);
                    command.Parameters.AddWithValue("@Age", student.Age);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }



        [HttpPut("update")]
        public async Task<IActionResult> UpdateStudent(Student student)
        {
            string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = "UPDATE Student SET Name = @Name, Age = @Age WHERE StudentId = @StudentId;";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Name", student.Name);
                    command.Parameters.AddWithValue("@Age", student.Age);
                    command.Parameters.AddWithValue("@StudentId", student.StudentId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }




    }
}
