namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        Student student = new Student();
        student.Name = "Frede";
        student.Age = 24;
        Console.WriteLine($"{student.Name} har alderen {student.Age}");
    }
}