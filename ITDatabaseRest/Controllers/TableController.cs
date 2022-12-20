using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ITDatabaseRest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ILogger<TableController> _logger;
        private readonly Manager _manager = Manager.Instance;
        public TableController(ILogger<TableController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        ///Returns table with specified name
        /// </summary>
        /// <response code="200">_Returned table with specified name_</response>
        /// <response code="400">_Database is not created yet or table with this name does not exist _</response>
        [HttpGet]
        [Route("{tableName}")]
        public ActionResult<CustomResponse<Table>> Get(String tableName)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var table = _manager.Database.tables.Find((table) => table.name == tableName);
            if (table!= null)
            {
                var relatedActions = new Dictionary<String, String>
            {
                { "deleteTable",$"/Tables/{tableName}" },
                { "getRows",$"/Tables/{tableName}/Rows"},
                { "getColmns",$"/Tables/{tableName}/Columns" }};

                var response = new CustomResponse<Table>(
                    table, relatedActions);
                return Ok(response);


            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }

        /// <summary>
        /// Creates table with specified name
        /// </summary>
        /// <response code="200">_Created table with specified name_</response>
        /// <response code="400">_Table with specified name or database is not created yet _</response>
        [HttpPost]
        public IActionResult Create(String tableName)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }

            bool res = _manager.AddTable(tableName);
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
        /// Removes table with specified name
        /// </summary>
        /// <response code="200">_Removed table with specified name_</response>
        /// <response code="400">_Table with specified name or database is not created yet _</response>
        [HttpDelete]
        public IActionResult Delete(string tableName)
        {

            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var tableIndx = _manager.Database.tables.FindIndex((table)=>table.name==tableName);
            if (tableIndx!=-1)
            {
               _manager.DeleteTable(tableIndx);
                return Ok("Created successful");
            }
            else
            {
                return BadRequest("Table does not exist");
            }

        }
    }

}

