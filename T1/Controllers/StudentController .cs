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


        //[HttpPost("insert")]
        //public async Task<IActionResult> InsertStudent(Student student)
        //{
        //    string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var commandText = "INSERT INTO Student (Name, Age) VALUES (@Name, @Age);";

        //        using (var command = new SqlCommand(commandText, connection))
        //        {
        //            command.Parameters.AddWithValue("@Name", student.Name);
        //            command.Parameters.AddWithValue("@Age", student.Age);

        //            await command.ExecuteNonQueryAsync();
        //        }
        //    }

        //    return Ok();
        //}




        [HttpPost("insert")]
        public async Task<IActionResult> InsertStudent([FromForm] Student student)
        {
            string _connectionString = "Server=103.74.120.121;Database=students;User Id=tmp;Password=tmp@123;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var commandText = "INSERT INTO Student (Name, Age, ImagePath) VALUES (@Name, @Age, @ImagePath); SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Name", student.Name);
                    command.Parameters.AddWithValue("@Age", student.Age);

                    // Get the IFormFile from the request
                    var file = student.ImagePath; // Get the uploaded file from the student object

                    // Check if a file is uploaded
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);

                        // Generate a unique file name
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                        // Set the file path where the image will be saved on the server
                        var filePath = Path.Combine("uploads", uniqueFileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        command.Parameters.AddWithValue("@ImagePath", filePath);
                    }
                    else
                    {
                        // If no file is uploaded, set the image path to null or an appropriate default value in the database
                        command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    }

                    // Execute the SQL command and get the inserted student ID
                    int insertedStudentId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    // Set the ID, name, age, and image path in the student object
                    student.StudentId= insertedStudentId;
                   
                }
            }

            return Ok(student);
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
