// namespace – групиране на класове
namespace CSharpBasics;

// using директиви – за включване на пространства от имена
using System.Collections.Generic; 


// class – основен контейнер за код
class Program
{
    // Main метод – входна точка
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        
        // Типове данни
        
        int age = 25;
        double price = 19.99;
        decimal totalPrice = 100.00M;
        string name = "Emiliyan";
        bool isActive = true;
        char gender = 'm';
        
        
        // Управляващи конструкции
        
        int number = 5;

        // if-else-if-else
        if (number > 0)
        {
            Console.WriteLine("Положително число");
        }
        else if (number < 0)
        {
            Console.WriteLine("Неположително число");
        }
        else
        {
            Console.WriteLine("Нула");
        }

        // switch
        switch (number)
        {
            case 1:
            {
                Console.WriteLine("Едно"); 
                break;
            }
            case 5:
            {
                Console.WriteLine("Пет"); 
                break;
            }
            default:
            {
                Console.WriteLine("Друго число"); 
                break;
            }
        }
        
        // Цикли

        // for
        for (int i = 0; i < 3; i++)
            Console.WriteLine($"i = {i}");

        // while
        int counter = 0;
        while (counter < 3)
        {
            Console.WriteLine($"counter = {counter}");
            counter++;
        }
        
        // Структури от данни
        
        // Масив
        int[] numbers = { 1, 2, 3, 4, 5, 6 };
        Console.WriteLine(numbers[1]); // 2

        // Списък
        List<string> fruits = new List<string> { "ябълка", "банан" };
        fruits.Add("портокал");
        
        foreach (var fruit in fruits)
            Console.WriteLine(fruit);

        // Речник
        Dictionary<int, string> students = new Dictionary<int, string>();
        students[205] = "Иван";
        students[101] = "Мария";
        
        Console.WriteLine(students[205]);
        
        
        // Методи
        int result = Add(3, 5);
        Console.WriteLine($"Сума: {result}");
        PrintMessage();
        PrintMessage("Добър ден");
        
        // Класове и обекти
        
        var student = new Person(name="Emiliyan", age=36);
        student.SayHello();
        
        // Работа с изключения

        try
        {
            student.SayGoodbye();
        }
        catch (System.NotImplementedException e)
        {
            Console.WriteLine("Catch block");
            Console.WriteLine(e.Message);
        }

        finally
        {
            Console.WriteLine("Finally");
        }
        
        // LINQ
        
        // Избиране на четните числа
        
        // var evens = numbers.Where(n => n % 2 == 0);

        var evens = from n in numbers
            where n % 2 == 0
            select n;
        

        Console.WriteLine("evens");
        foreach (var num in evens)
            Console.WriteLine(num); 
        
        
    }
    
    static int Add(int a, int b)
    {
        return a + b;
    }

    static void PrintMessage(string msg = "Здравей")
    {
        Console.WriteLine(msg);
    }
}