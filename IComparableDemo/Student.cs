using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IComparableDemo
{
    class Student:IComparable
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public double Average { get; set; }

        public int CompareTo(object obj)
        {
            Student otherStudent = obj as Student;
            if (obj!=null)
            {
                return this.Name.CompareTo(otherStudent.Name);
            }
            return -1;
        }

        public override string ToString()
        {
            return $"{Name} {Family} [{Average}]";
        }
    }
}
