using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GolfScorekeeper
{
    public class Courses
    {
        private Dictionary<string, int[]> courses = new Dictionary<string, int[]>()
        {
        };

        public int GetNineOrEighteen(string courseName)
        {
            return courses[courseName].Length;
        }
        
        public int GetCoursePar(string courseName)
        {
            int holeCount = GetNineOrEighteen(courseName);
            int par = 0;
            for (int i=0; i < holeCount; i++)
            {
                par += courses[courseName][i];
            }
            return par;
        }

        public int GetHolePar(string courseName, int holeNumber)
        {
            return courses[courseName][holeNumber-1];
        }
        //Unused
        public int GetCourseCount()
        {
            return courses.Count;
        }

        public List<string> GetCourseList()
        {
            return new List<string>(courses.Keys);
        }
        //What score do you have on this course after X holes?
        public int CalculateCurrentScore(string courseName, int holeNumber, int currentScore)
        {
            int totalPar = 0;
            for (int i=1; i <= holeNumber; i++)
            {
                totalPar += GetHolePar(courseName, i);
            }

            return currentScore - totalPar;
        }
        //Return 1 if replacing existing course (to notify user)
        public int AddNewCourse(string courseName, int[] parList)
        {
            if (!courses.ContainsKey(courseName) )
            {   
                courses.Add(courseName, parList);
                return 0;
            }
            else
            {
                courses[courseName] = parList;
                return 1;
            }
        }

        public void RemoveCourse(string courseName)
        {
            if (courses.ContainsKey(courseName))
            {
                courses.Remove(courseName);
            }
        }
    }
}
