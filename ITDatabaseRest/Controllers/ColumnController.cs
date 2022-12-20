using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITDatabaseRest.Controllers
{
    [Route("api/Tables")]
    [ApiController]
    public class ColumnController : ControllerBase
    {
        private readonly Manager _manager = Manager.Instance;

        /// <summary>
        /// Returns all attributes from table with specified name
        /// </summary>
        /// <response code="200">_All columns from table with specified name returned_</response>
        /// <response code="400">_Table with specified name or database is not created yet _</response>
        [HttpGet]
        [Route("{tableName}/Columns")]
        public ActionResult<CustomResponse<IEnumerable<Column>>> GetAll(string tableName)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var table = _manager.Database.tables.Find((table) => table.name == tableName);
            if (table != null)
            {
                var relatedActions = new Dictionary<String, String>
            {
                { "getTable",$"/Tables/{tableName}"},
                { "deleteTable",$"/Tables/{tableName}"},
                };

                var response = new CustomResponse<IEnumerable<Column>>(
                  table.columns, relatedActions);
                return Ok(response);
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }
        /// <summary>
        /// Returns attribute with specified name from table with specified name
        /// </summary>
        /// <response code="200">_Column with specified name  from table with specified name returned_</response>
        /// <response code="400">_Table with specified name or database is not created yet _</response>
        [HttpGet]
        [Route("{tableName}/Columns/{name}")]
        public ActionResult<CustomResponse<Column>> Get(string tableName, string name)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var table = _manager.Database.tables.Find((table) => table.name == tableName);
            if (table != null)
            {
                var column = table.columns.Find((column) => column.name == name);
                var relatedActions = new Dictionary<String, String>
            {
                { "getColumns",$"/Tables/{tableName}/Columns"},
                { "deleteColumn",$"/Tables/{tableName}/Rows/{name}"},
                };
                var response = new CustomResponse<Column>(
                column, relatedActions);
                return column == null ? BadRequest("Column does not exist") : Ok(response);
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
          
        }
        /// <summary>
        /// Returns table where values of attribute with specified name from table with specified name matches with specified regex pattern
        /// </summary>
        /// <response code="200">_Returned table which matches search pattern_</response>
        /// <response code="400">_Table with specified name or database is not created yet,or specified attribute does not exist _</response>
        [HttpGet]
        [Route("{tableName}/Columns/{name}/Search")]
        public ActionResult<CustomResponse<Table>> Search(string tableName,string name,[FromBody] string pattern)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var table = _manager.Database.tables.Find((table) => table.name == tableName);
            if (table != null)
            {
                var columnIndx = table.columns.FindIndex((column) => column.name == name);
                var result= _manager.Search(_manager.Database.tables.IndexOf(table), columnIndx, pattern);
                var relatedActions = new Dictionary<String, String>
            {
                { "getTable",$"/Tables/{tableName}"},
                { "deleteTable",$"/Tables/{tableName}"},
                 { "getColumn",$"/Tables/{tableName}/Columns/{name}"},
                };

                var response = new CustomResponse<Table>(
                  result, relatedActions);
                return columnIndx ==-1? BadRequest("Column does not exist") : Ok(response);

            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }

        }

        /// <summary>
        /// Creates attribute with specified name and type in specified table 
        /// </summary>
        /// <response code="200">_New attribute created_</response>
        /// <response code="400">_Table with specified name or database is not created yet, or specified type is not supported _</response>
        [HttpPost]
        [Route("{tableName}/Columns/{name}")]
        public IActionResult Post(string tableName, string name,[FromBody] string type)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
           int tableIndx = _manager.Database.tables.FindIndex((table) => table.name == tableName);
            if (tableIndx != -1)
            {
                var column = Manager.ColumnFromString(name, type);
                if (column == null)
                {
                    return BadRequest("Type is not valid");
                }
                _manager.AddColumn(tableIndx, column);
                return Ok("Added successfully");
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }

        }



        /// <summary>
        /// Removed attribute from table with specified name 
        /// </summary>
        /// <response code="200">_Attribute removed_</response>
        /// <response code="400">_Table with specified name or database is not created yet, or specified attribute does not exists _</response>      
        [HttpDelete]
        [Route("{tableName}/Columns/{name}")]
        public ActionResult Delete(string tableName, string name)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            int tableIndx = _manager.Database.tables.FindIndex((table) => table.name == tableName);
            if (tableIndx != -1)
            {
                var columnIndx = _manager.GetTable(tableIndx).columns.FindIndex((column) =>column.name==name);
                if (columnIndx == -1)
                {
                    return BadRequest("Column does not exist");
                }
                _manager.DeleteColumn(tableIndx,columnIndx);
                return Ok("Deleted successfully");
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }
        
    }
}
