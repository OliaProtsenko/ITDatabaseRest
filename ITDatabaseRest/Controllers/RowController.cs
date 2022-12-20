using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITDatabaseRest.Controllers
{
    [Route("api/Rows")]
    [ApiController]
    public class RowController : ControllerBase
    {
        private readonly Manager _manager = Manager.Instance;

        /// <summary>
        /// Returns all rows from table with specified name
        /// </summary>
        /// <response code="200">_all rows from table with specified name returned_</response>
        /// <response code="400">_Table with specified name or database is not created yet _</response>
        [HttpGet]
        [Route("{tableName}/Rows")]
        public ActionResult<CustomResponse<IEnumerable<Row>>> GetAll(string tableName)
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

                var response = new CustomResponse<IEnumerable<Row>>(
                  table.rows, relatedActions);
                return Ok(response);
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }

        /// <summary>
        /// Returns row with specified id from table with specified name
        /// </summary>
        /// <response code="200">_Specified row from table with specified name returned_</response>
        /// <response code="400">_Table with specified name or database is not created yet _</response>
        [HttpGet]
        [Route("{tableName}/Rows/{id}")]
        public ActionResult<CustomResponse<Row>> Get(string tableName,int id)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var table = _manager.Database.tables.Find((table) => table.name == tableName);
            if (table != null)
            {
                if (id >= table.rows.Count)
                {
                    return BadRequest("Row does not exist");
                }
                var relatedActions = new Dictionary<String, String>
            {
                { "getRows",$"/Tables/{tableName}/Rows"},
                { "deleteRow",$"/Tables/{tableName}/Rows/{id}"},
                {"updateRow",$"/Tables/{tableName}/Rows/{id}" }
                };

                var row = table.rows[id];
                var response = new CustomResponse<Row>(
                row, relatedActions);
                return Ok(response);
                
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }

        /// <summary>
        /// Updates rows with specified index from table with specified name
        /// </summary>
        /// <response code="200">_row from table with specified name updated_</response>
        /// <response code="400">_Table with specified name or database is not created yet, or row with this index does not exist, or value has invalid type_</response>  
        [HttpPost]
        [Route("{tableName}/Rows")]
        public ActionResult Post(string tableName,[FromBody] Row row)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var tableIndx = _manager.Database.tables.FindIndex((table) => table.name == tableName);
            if (tableIndx != -1)
            {
                _manager.AddRow(tableIndx);
                var rowIndx = row.values.Count - 1;
                for (int i = 0; i < row.values.Count; i++)
                {
                    if (!_manager.ChangeCellValue(row.values[i], tableIndx, i, rowIndx))
                    {
                        _manager.DeleteRow(tableIndx, rowIndx);
                        return BadRequest(new { error = $"Value {row.values[i]} is of invalid type" });

                    }

                }
                return Ok("Row successfully added");
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }

        /// <summary>
        ///Added row to table with specified name
        /// </summary>
        /// <response code="200">_added row to table with specified name_</response>
        /// <response code="400">_Table with specified name or database is not created yet, or value has invalid type _</response>
        [HttpPut]
        [Route("{tableName}/Rows/{id}")]
        public ActionResult Put(string tableName,int id, [FromBody] Row row)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var tableIndx = _manager.Database.tables.FindIndex((table) => table.name == tableName);
            if (tableIndx != -1)
            {
                if (id < _manager.Database.tables[tableIndx].rows.Count)
                {
                    if (row.values.Count == _manager.Database.tables[tableIndx].columns.Count)
                    {
                        var currentRow = _manager.Database.tables[tableIndx].rows[id];

                        for (int i = 0; i < row.values.Count; i++)
                        {
                       
                            if (!_manager.ChangeCellValue(row.values[i], tableIndx, i, id))
                            {  
                                for(int j = i; j > 0; j--)
                                {
                                    _manager.ChangeCellValue(currentRow.values[j], tableIndx, j, id);
                                }

                                return BadRequest(new { error = $"Value {row.values[i]} is of invalid type" });

                            }
                        }
                        return Ok("Row successfully added");
                    }
                    else
                    {
                        return BadRequest("Numbers of the row's values and the table's columns don't match");
                    }
                   
                }
                else
                {
                    return BadRequest("Row does not exist");
                }
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }

        [HttpDelete]
        [Route("{tableName}/Rows/{id}")]
        public ActionResult Delete(string tableName,int id)
        {
            if (_manager.Database == null)
            {
                return BadRequest("Database doesn't exist");
            }
            var tableIndx = _manager.Database.tables.FindIndex((table) => table.name == tableName);
            if (tableIndx !=-1)
            {
                if (id >= _manager.Database.tables[tableIndx].rows.Count)
                {
                    return BadRequest("Row does not exist");
                }
              _manager.DeleteRow(tableIndx,id);
                return Ok("Deleted successfully");
            }
            else
            {
                return BadRequest("Table with name $tableName does not exist");
            }
        }
    }
}
