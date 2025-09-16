namespace CSharpBasics;

public class Student
{
    private readonly string _name;
    private int _age;

    public Student(string name, int age)
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