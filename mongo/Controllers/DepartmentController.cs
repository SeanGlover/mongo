using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mongo.Controllers.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration) { _configuration = configuration; }
        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var dbList = dbClient.GetDatabase("testDb").GetCollection<Department>("Department").AsQueryable();
            return new JsonResult(dbList);
        }
        public JsonResult Get(int departmentId)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var dbList = dbClient.GetDatabase("testDb").GetCollection<Department>("Department").AsQueryable().Where(d => d.DepartmentId == departmentId);
            return new JsonResult(dbList);
        }
        [HttpPost]
        public JsonResult Post(Department dep)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            int lastDepartmentId = dbClient.GetDatabase("testDb").GetCollection<Department>("Department").AsQueryable().Count();
            dep.DepartmentId = lastDepartmentId + 1;
            dbClient.GetDatabase("testDb").GetCollection<Department>("Department").InsertOne(dep);
            return new JsonResult("Added successfully");
        }
        [HttpPut]
        public JsonResult Put(Department dep)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var filter = Builders<Department>.Filter.Eq("DepartmentId", dep.DepartmentId);
            var update = Builders<Department>.Update.Set("DepartmentName", dep.DepartmentName);
            dbClient.GetDatabase("testDb").GetCollection<Department>("Department").UpdateOne(filter, update);
            return new JsonResult("Updated successfully");
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var filter = Builders<Department>.Filter.Eq("DepartmentId", id);
            dbClient.GetDatabase("testDb").GetCollection<Department>("Department").DeleteOne(filter);
            return new JsonResult("Deleted successfully");
        }
    }
}
