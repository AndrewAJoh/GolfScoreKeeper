using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using SQLitePCL;
using System.IO;

namespace GolfScorekeeper
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParList { get; set; }
    }
}
