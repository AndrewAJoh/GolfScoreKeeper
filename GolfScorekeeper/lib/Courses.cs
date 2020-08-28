using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GolfScorekeeper
{
    public class Courses
    {
        private Dictionary<string, int[]> courses = new Dictionary<string, int[]>()
        {
            { "Hiawatha", new int[] {5, 5, 3, 4, 4, 4, 5, 3, 4, 5, 4, 3, 5, 4, 3, 4, 4, 4} },
            { "Sample2", new int[] {1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2, 3, 4, 5, 6, 4, 3} }
        };

        public int getCoursePar(string courseName)
        {
            int par = 0;
            for (int i=0; i < 18; i++)
            {
                par += courses[courseName][i];
            }
            return par;
        }

        public int getHolePar(string courseName, int holeNumber)
        {
            return courses[courseName][holeNumber-1];
        }
        //Unused
        public int getCourseCount()
        {
            return courses.Count;
        }

        public List<string> getCourseList()
        {
            return new List<string>(courses.Keys);
        }
        //What score do you have on this course after X holes?
        public int CalculateCurrentScore(string courseName, int holeNumber, int currentScore)
        {
            int totalPar = 0;
            for (int i=1; i <= holeNumber; i++)
            {
                totalPar += getHolePar(courseName, i);
            }

            return currentScore - totalPar;
        }

    }
}
