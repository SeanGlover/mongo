using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mongo.Controllers.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _env = environment;
        }
        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var dbList = dbClient.GetDatabase("testDb").GetCollection<Employee>("Employee").AsQueryable();
            return new JsonResult(dbList);
        }
        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            int lastEmployeeId = dbClient.GetDatabase("testDb").GetCollection<Employee>("Employee").AsQueryable().Count();
            emp.EmployeeId = lastEmployeeId + 1;
            dbClient.GetDatabase("testDb").GetCollection<Employee>("Employee").InsertOne(emp);
            return new JsonResult("Added successfully");
        }
        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var filter = Builders<Employee>.Filter.Eq("EmployeeId", emp.EmployeeId);
            var update = Builders<Employee>.Update
            .Set("Name", emp.Name)
            .Set("Department", emp.Department)
            .Set("DateOfJoining", emp.DateOfJoining)
            .Set("PhotoFileName", emp.PhotoFileName);

            dbClient.GetDatabase("testDb").GetCollection<Employee>("Employee").UpdateOne(filter, update);
            return new JsonResult("Updated successfully");
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCnxn"));
            var filter = Builders<Employee>.Filter.Eq("EmployeeId", id);
            dbClient.GetDatabase("testDb").GetCollection<Employee>("Employee").DeleteOne(filter);
            return new JsonResult("Deleted successfully");
        }
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                var filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                using (var stream = new FileStream(physicalPath, FileMode.Create)) { postedFile.CopyTo(stream); }
                return new JsonResult(filename);
            }
            catch { return new JsonResult("anonymous.png"); }
        }
    }
}
