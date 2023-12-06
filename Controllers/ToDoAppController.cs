using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace PullaposkiToDoApp.Controllers
{
	[Route("api/[controller]"), ApiController]
	public class ToDoAppController : Controller
	{
		IConfiguration _configuration;

		public ToDoAppController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// [HttpGet, Route("GetNotes")]: These attributes indicate that the following method
		// should respond to HTTP GET requests at the path “api/ToDoAppController/GetNotes”.
		[HttpGet, Route("GetNotes")]
		public JsonResult GetNotes()
		{
			// defines a SQL query to select all records from the Notes table in the database.
			var query = "select * from dbo.Notes";

			// creates a new DataTable to store the results of the query.
			var table = new DataTable();

			// It retrieves the connection string for the database from the configuration.
			var sqlDataSource = _configuration.GetConnectionString("ToDoAppDBCon");

			SqlDataReader myReader;

			// It opens a new SQL connection using the connection string
			using (SqlConnection myConnection = new SqlConnection(sqlDataSource))
			{
				myConnection.Open();

				// and creates a new SQL command with the query.
				using (SqlCommand myCommand = new SqlCommand(query, myConnection))
				{
					// It executes the command and loads the results into the DataTable.
					myReader = myCommand.ExecuteReader();
					table.Load(myReader);
					myReader.Close();
					myConnection.Close();
				}
			}

			// Finally, it returns the DataTable as a JsonResult.
			return new JsonResult(table);
		}

		// This method is called when a POST request is made to “api/ToDoAppController/AddNotes”.
		// It takes a string named newNotes from the form data of the request.
		[HttpPost, Route("AddNotes")]
		public JsonResult AddNotes([FromForm] string newNotes)
		{
			// defines a SQL query to insert a new record into the Notes table in the database.
			var query = "insert into dbo.Notes values(@newNotes)";
			var table = new DataTable();
			var sqlDataSource = _configuration.GetConnectionString("ToDoAppDBCon");

			SqlDataReader myReader;

			using (SqlConnection myConnection = new SqlConnection(sqlDataSource))
			{
				myConnection.Open();

				using (SqlCommand myCommand = new SqlCommand(query, myConnection))
				{
					// adds the newNotes string to the command parameters
					myCommand.Parameters.AddWithValue("@newNotes", newNotes);

					// executes the command, which inserts the new note into the database
					// and returns a SqlDataReader containing the results of the query
					myReader = myCommand.ExecuteReader();

					table.Load(myReader);
					myReader.Close();
					myConnection.Close();
				}
			}

			return new JsonResult("Added succesfully!");
		}

		[HttpDelete, Route("DeleteNotes")]
		//This method is called when a DELETE request is made to “api/ToDoAppController/DeleteNotes”.
		//It takes an integer id as a parameter, which should be included in the body of the DELETE request.
		public JsonResult DeleteNotes(int id)
		{
			// defines a SQL query to delete a record from the Notes table in the database
			// where the Id matches the provided id
			string query = "delete from dbo.Notes where id=@id";

			var table = new DataTable();
			var sqlDataSource = _configuration.GetConnectionString("ToDoAppDBCon");
			
			SqlDataReader myReader;


			// In this code, myConnection is an IDisposable object.
			// The using statement ensures that myConnection.Dispose() is called
			// whether the code inside the block runs successfully or if an exception is thrown.

			// When you create a SqlConnection object, it opens a connection to the database.
			// This is an expensive operation in terms of system resources.
			// If the connection is not closed when it’s no longer needed,
			// it can consume system resources and impact performance.
			// By implementing IDisposable, the SqlConnection class provides a way to ensure
			// that the connection is properly closed and the resources are released.
			using (SqlConnection myConnection = new SqlConnection(sqlDataSource))
			{
				myConnection.Open();
				using(SqlCommand myCommand = new SqlCommand(query, myConnection))
				{
					// adds the id to the command parameters
					myCommand.Parameters.AddWithValue("@id", id);
						
					myReader = myCommand.ExecuteReader();
					table.Load(myReader);
					myReader.Close();
					myConnection.Close();
				}
			}

			return new JsonResult("Deleted succesfully");
		}
	}
}
