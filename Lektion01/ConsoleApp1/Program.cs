namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        Student student = new Student();
        student.Name = "Frede";
        student.Age = 24;
        Console.WriteLine($"{student.Name} har alderen {student.Age}");


        var car1 = new Car("Citroen", "C3", 2014);
        var car2 = new Car("Citroen", "C3", 2014);
        Console.WriteLine(car1 == car2);
        Console.WriteLine(car1);
        
    }
}