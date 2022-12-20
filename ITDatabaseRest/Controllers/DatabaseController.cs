using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITDatabaseRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    { 
       

        private readonly ILogger<DatabaseController> _logger;
        private readonly Manager _manager = Manager.Instance;

        public DatabaseController(ILogger<DatabaseController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Returns  database in case it was created before
        /// </summary>
        /// <response code="400">_Database is not created yet_</response>

        [HttpGet]
        public ActionResult<CustomResponse<Database>> Get()
        {
            if (_manager.Database!=null)
            { var relatedActions = new Dictionary<String, String>
            {
                { "deleteDatabase","/Database" } };

                var response = new CustomResponse<Database>(
                    _manager.Database, relatedActions);
                return Ok(response);

            }
            else
            {
                return BadRequest("Database is not created yet");
            }
        }

        /// <summary>
        /// Creates database with specified name
        /// </summary>
        /// <response code="200">_Created database with specified name_</response>
        /// <response code="400">_Specified name is incorrect _</response>
        [HttpPost]
        public  IActionResult Create(String name) {
            if (_manager.Database != null)
            {
                return BadRequest("Database already exists");
            }
            
            bool res =_manager.CreateDB(name);
            if (res)
            {   
                return Ok("Created successful");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
        /// <summary>
        /// Clears database
        /// </summary>
        /// <response code="200">_Cleared database_</response>
        /// <response code="400">_Something went wrong _</response>
        [HttpDelete]
        public IActionResult Delete()
        {

            if (_manager.DeleteDB()) {
                return Ok("Successful deleted");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
            
        }
    }
}
