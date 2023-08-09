using Domain;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Repository;
public abstract class AbstractRepository<T> : IRepository<T> where T : PersistentObject
{
    // The connection string for the database.
    private string _connectionString = "Server=localhost;Port=5432;Database=systemmetrixdb;UserId=postgres;Password=123456";

    // An abstract method that will be implemented in subclasses.
    // It is responsible for mapping data from a DataReader to an object of type T.
    abstract protected T Map(DbDataReader reader);

    // Method to retrieve column names (fields) of a T entity.
    public string GetColumnNames(T entity)
    {
        // Get the properties of the entity and extract their names.
        var columns = entity.GetType().GetProperties()
            .Select(e => e.Name)
            .Where(name => name != "id") // Exclude the "id" field.
            .ToList();

        // Combine the names into a string, separated by commas.
        return string.Join(",", columns);
    }

    // Method to get the table name based on the TableAttribute.
    protected string GetTableName()
    {
        return this.GetType()
            .GetInterfaces()
            .Where(intType => intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IRepository<>))
            .Select(t => t.GetGenericArguments()[0])
            .Select(type =>
            {
                TableAttribute? tableAttribute = null;
                var attrs = type.GetCustomAttributes(true);
                if (attrs != null)
                {
                    var tableAttr = attrs.FirstOrDefault(attr => attr.GetType() == typeof(TableAttribute));
                    tableAttribute = tableAttr as TableAttribute;
                }
                return tableAttribute!.Name ?? throw new Exception();
            })
            .First();
    }

    // Method to map entity annotations (attributes) to a string.
    public string MapEntityAnnotations(T entity)
    {
        var properties = entity.GetType().GetProperties();
        string result = "";

        foreach (var property in properties)
        {
            if (property == null)
            {
                return null;
            }
            Type propertytype = property.GetType();

            var type = propertytype.GetTypeInfo();

            // Skip the property if it is of type Guid.
            if (property.PropertyType == typeof(Guid))
            {
                continue;
            }

            try
            {
                var value = property.GetValue(entity);


                // Get the value of the property.
                if (value != null)
                    value = value.ToString().Replace(",", ".");


                // Convert the value to a string and add it to the result.
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(string))
                {
                    result = result + "'" + value + "'";
                }
                else
                {
                    result = result + (value ?? "null");
                }

                // Add a comma if it's not the last property.
                if (properties.Last() != property)
                    result = result + ",";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return result;
    }

    // Method to map fields and values for the UPDATE operation.
    public string MapUpdeteEntity(T entity)
    {
        // Get field names and their values.
        string result = MapEntityAnnotations(entity);
        string columns = GetColumnNames(entity);

        var colum = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var ress = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var a = 0;
        string sql = "";
        while (a < colum.Length)
        {
            sql += $"{colum[a]} = ";
           
            if (a != colum.Length - 1)
            {
                sql += $"{ress[a]}, ";
            }
            else
                sql += $"{ress[a]} ";

            a++;
        }
        return sql;
    }
    // Method to map property names and values for UPDATE operation.
    public string MapUpdateValues(T entity)
    {
        var properties = entity.GetType().GetProperties();
        string result = "";

        foreach (var property in properties)
        {
            if (property == null)
            {
                return null; // Returns null if a property is null (this might be unnecessary).
            }
            var attributes = property.CustomAttributes;

            Type propertytype = property.GetType(); // Gets the type of the property, not the value.
            var type = propertytype.GetTypeInfo();   // Gets TypeInfo of the property type (not needed).

            if (property.PropertyType == typeof(Guid)) // Skips properties of type Guid.
            {
                continue;
            }

            var value = property.GetValue(entity); // Gets the value of the property.
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(string))
            {
                result += $"{property.Name} = '{value}',"; // Formats value as a string if it's DateTime or string.
            }
            else
            {
                result += $"{property.Name} = {value},"; // Uses the value as is for other property types.
            }
        }
        return result.TrimEnd(','); // Trims the trailing comma before returning.
    }

    // Method to retrieve all entities from the database.
    public virtual IQueryable<T> GetAll()
    {
        IList<T> metrics = new List<T>();
        var tableName = GetTableName();
        var sql = $"SELECT * FROM public.{tableName} ORDER BY id ASC";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                DbCommand command = (DbCommand)connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                connection.Open();

                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    metrics.Add(Map(reader)); // Maps data from the reader to an entity using the Map method.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return metrics.AsQueryable(); // Returns the list of entities as an IQueryable.
    }

    // Method to retrieve all entities asynchronously from the database.
    public async Task<IQueryable<T>> GetAllAsync()
    {
        IList<T> metrics = new List<T>();
        var tableName = GetTableName();
        var sql = $"SELECT * FROM public.{tableName} ORDER BY id ASC";

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                DbCommand command = (DbCommand)connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                connection.Open();

                DbDataReader reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    metrics.Add(Map(reader)); // Maps data from the reader to an entity using the Map method.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return metrics.AsQueryable(); // Returns the list of entities as an IQueryable.
    }

    // Method to create a new entity in the database.
    public void Create(T item)
    {
        var tableName = GetTableName(); // Get the table name for the entity.
        var columns = GetColumnNames(item); // Get the column names for the entity.
        var values = MapEntityAnnotations(item); // Get the values to be inserted into columns.
        var sql = $"INSERT INTO public.{tableName}({columns}) VALUES ({values})"; // Construct the SQL INSERT statement.

        using (var connection = new NpgsqlConnection(_connectionString)) // Establish a connection to the database.
        {
            DbCommand command = connection.CreateCommand(); // Create a database command.
            command.CommandText = sql; // Set the SQL command text.
            command.CommandType = CommandType.Text; // Set the command type to text.

            connection.Open(); // Open the database connection.

            // Bind the parameter values for each property of the entity.
            foreach (var property in item.GetType()
                .GetProperties()
                .ToList())
            {
                string columnName = property.Name; // Get the column name from ColumnAttribute.
                if (!string.IsNullOrEmpty(columnName))
                {
                    var parameter = command.CreateParameter(); // Create a parameter for the command.
                    parameter.ParameterName = columnName; // Set the parameter name.
                    parameter.Value = property.GetValue(item) ?? DBNull.Value; // Set the parameter value.
                    command.Parameters.Add(parameter); // Add the parameter to the command.
                }
            }

            command.ExecuteNonQuery(); // Execute the SQL command to insert the entity.
        }
    }
    // Method to Delete a new entity in the database.
    public void Delete(T entity)
    {
        var tableName = GetTableName();
        var sql = $"DELETE From public.{tableName} WHERE id = '{entity.id}'";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            DbCommand command = (DbCommand)connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            connection.Open();
            DbDataReader reader = command.ExecuteReader(); // Executes the DELETE SQL command.
        }
    }

    // Method to retrieve a single entity from the database.
    public virtual IQueryable<T> Get()
    {
        IList<T> metrics = new List<T>();
        var tableName = GetTableName();

        var sql = $"SELECT * FROM public.{tableName} ORDER BY id ASC";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                DbCommand command = (DbCommand)connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                connection.Open();

                DbDataReader reader = command.ExecuteReader();
                reader.Read();

                metrics.Add(Map(reader)); // Maps data from the reader to a single entity using the Map method.

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return (IQueryable<T>)metrics.FirstOrDefault(); // Returns the first entity from the list.
    }

    // Method to update an entity in the database.
    public void Update(T entity, string ip)
    {
        var tableName = GetTableName();

        string result = MapUpdeteEntity(entity); // Retrieves the UPDATE SQL statement.
        var sql = $"UPDATE  public.{tableName} set {result} WHERE id = '{ip}'";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                DbCommand command = (DbCommand)connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                connection.Open();
                DbDataReader reader = command.ExecuteReader(); // Executes the UPDATE SQL command.
                connection.Close(); // Closes the connection.
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception.Message: " + ex.Message);
        }
    }

    // Method to retrieve a single entity asynchronously from the database.
    public async Task<IQueryable<T>> GetAsync()
    {
        IList<T> metrics = new List<T>();
        var tableName = GetTableName();

        var sql = $"SELECT * FROM public.{tableName} ORDER BY id ASC";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                DbCommand command = (DbCommand)connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                connection.Open();

                DbDataReader reader = await command.ExecuteReaderAsync();
                reader.Read();

                metrics.Add(Map(reader)); // Maps data from the reader to a single entity using the Map method.

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return (IQueryable<T>)metrics.FirstOrDefault(); // Returns the first entity from the list.
    }

    // Method to create a new entity asynchronously in the database.
    public async Task CreateAsync(T item)
    {
        var tableName = GetTableName();
        string result = MapEntityAnnotations(item);
        string columns = GetColumnNames(item);
        var sql = $"INSERT INTO public.{tableName}({columns}) values ({result})";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            DbCommand command = (DbCommand)connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            connection.Open();
            DbDataReader reader = await command.ExecuteReaderAsync(); // Executes the INSERT SQL command asynchronously.
        }
    }
}
