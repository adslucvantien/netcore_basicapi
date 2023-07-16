namespace T1.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }


        public IFormFile? ImagePath { get; set; }

    }



}
