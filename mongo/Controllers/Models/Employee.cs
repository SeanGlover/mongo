﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Controllers.Models
{
    public class Employee
    {
        public ObjectId Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string PhotoFileName { get; set; }
    }
}
