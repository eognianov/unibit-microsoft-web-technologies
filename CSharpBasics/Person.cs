namespace CSharpBasics;

public interface ILivingBeing
{
    string Name { get; }
    int Age { get; }
}


public class Person : ILivingBeing
{
    private readonly string _name;
    private int _age;

    public Person(string name, int age)
    {
        _name = name;
        _age = age;
    }
    public string Name => _name;
    
    public int Age
    {
        get
        {
            return _age;
        }
        private set
        {
            _age = value;
        }
    }

    public void SayHello()
    {
        Console.WriteLine($"Здравей! Аз съм {Name}, на {Age} години.");
    }

    public void SayGoodbye()
    {
        throw new System.NotImplementedException();
    }
}

class Student(string name, int age) : Person(name, age)
{
    public List<string> Courses { get; set; } = new List<string>();
}