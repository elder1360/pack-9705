using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam
{
    class Program
    {
        static void Main(string[] args)
        {
            Exam exam = new Exam(10);
            
            Console.WriteLine();
            foreach (var q in exam)
            {
                Console.WriteLine(q);               
            }
            Console.ReadKey();
        }
    }
    internal class Exam:IEnumerable
    {
        public string Name { get; set; }
        public Question[] Questions = new Question[]
        {
            new Question(){Id=1, Q="What is your name"},
            new Question(){Id=2, Q="What is Programming language"},
            new Question(){Id=3, Q="What is C#"},
            new Question(){Id=4, Q="What is Perl"},
            new Question(){Id=5, Q="What is PHP"},
            new Question(){Id=6, Q="What is Python"},
            new Question(){Id=7, Q="What is Ruby"},
            new Question(){Id=8, Q="What is C"},
            new Question(){Id=9, Q="What is C++"},
            new Question(){Id=10, Q="What is Java"},
            new Question(){Id=11, Q="What is SQL"},
            new Question(){Id=12, Q="What is GO"},
            new Question(){Id=13, Q="What is Swift"},
            new Question(){Id=14, Q="What is B4A"},
            new Question(){Id=15, Q="What is .NET"},
            new Question(){Id=16, Q="What is F#"},
            new Question(){Id=17, Q="What is R#"},
            new Question(){Id=18, Q="What is VB"},
            new Question(){Id=19, Q="What is Basic"},
            new Question(){Id=20, Q="What is your name"},
            new Question(){Id=22, Q="What is Programming language"},
            new Question(){Id=23, Q="What is C#"},
            new Question(){Id=24, Q="What is Perl"},
            new Question(){Id=25, Q="What is PHP"},
            new Question(){Id=26, Q="What is Python"},
            new Question(){Id=27, Q="What is Ruby"},
            new Question(){Id=28, Q="What is C"},
            new Question(){Id=29, Q="What is C++"},
            new Question(){Id=30, Q="What is Java"},
            new Question(){Id=31, Q="What is SQL"},
            new Question(){Id=32, Q="What is GO"},
            new Question(){Id=33, Q="What is Swift"},
            new Question(){Id=34, Q="What is B4A"},
            new Question(){Id=35, Q="What is .NET"},
            new Question(){Id=36, Q="What is F#"},
            new Question(){Id=37, Q="What is R#"},
            new Question(){Id=38, Q="What is VB"},
            new Question(){Id=39, Q="What is Basic"},
            new Question(){Id=40, Q="What is your name"},
            new Question(){Id=41, Q="What is Programming language"},
            new Question(){Id=42, Q="What is C#"},
            new Question(){Id=43, Q="What is Perl"},
            new Question(){Id=44, Q="What is PHP"},
            new Question(){Id=45, Q="What is Python"},
            new Question(){Id=46, Q="What is Ruby"},
            new Question(){Id=47, Q="What is C"},
            new Question(){Id=48, Q="What is C++"},
            new Question(){Id=49, Q="What is Java"},
            new Question(){Id=50, Q="What is SQL"},
            new Question(){Id=51, Q="What is GO"},
            new Question(){Id=52, Q="What is Swift"},
            new Question(){Id=53, Q="What is B4A"},
            new Question(){Id=54, Q="What is .NET"},
            new Question(){Id=55, Q="What is F#"},
            new Question(){Id=56, Q="What is R#"},
            new Question(){Id=57, Q="What is VB"},
            new Question(){Id=58, Q="What is Basic"}
        };

        int _howManyQuestion;
        public Exam(int howManyQuestion)
        {
            _howManyQuestion = howManyQuestion;            
        }

        public IEnumerator GetEnumerator()
        {        
           return new ExamEnumerator(Questions,_howManyQuestion);
        }
    }
    internal class Question
    {
        public int Id { get; set; }
        public string Q { get; set; }

        public override string ToString()
        {
            return $"{Id}-{Q}?";
        }
    }
    internal class ExamEnumerator : IEnumerator
    {
        Random rnd = new Random();
        private int maxRandomNumber;
        private int[] numbers;

        private int _howManyQuestion;
        private Question[] _questions;
        
        public ExamEnumerator(Question[] questions,int howManyQuestion)
        {
            _questions = questions;
            _howManyQuestion = howManyQuestion;
            numbers = new int[_questions.Length];
            for (int i = 0; i < _questions.Length; i++)
            {
                numbers[i] = i + 1;
            }
            maxRandomNumber = _questions.Length - 1;
        }
        public object Current => _questions[Getnumber()];

        public bool MoveNext()
        {
            return (_howManyQuestion-- > 0);
        }

        public void Reset()
        {
            ;
        }
        private int Getnumber()
        {
            int number;
            int x = rnd.Next(maxRandomNumber);
            number = numbers[x];
            numbers[x] = numbers[maxRandomNumber--];
            
            return number;
        }

    }
}
