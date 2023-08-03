
namespace Repository;

using Domain;
using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Npgsql.PostgresTypes.PostgresCompositeType;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

public abstract class AbstractRepository<T> : IRepository<T> where T : PersistentObject
{
    private string _connectionString = "Server=localhost;Port=5432;Database=systemmetrixdb;UserId=postgres;Password=123456";
    abstract protected T Map(DbDataReader reader);

    public string GetColumnNames(T entity)
    {
        var columns = entity.GetType()
        .GetProperties()
        .Where(p => p.CustomAttributes.Any(ca => ca.AttributeType.Name.Equals(nameof(ColumnAttribute))))
        .Select(p =>
        {
            var columnAttr = p.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.Name.Equals(nameof(ColumnAttribute)));
            string columnName = columnAttr.ConstructorArguments[0].Value.ToString();
            return columnName;
        })
        .ToList();

        return string.Join(",", columns);
    }

    protected string GetTableName()
    {
        return this.GetType()
            .GetInterfaces()
            .Where(intType => intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IRepository<>))
            .Select(t => t.GetGenericArguments()[0])
            .ToList()
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
            }).First();
    }

    public string MapEntity(T entity)
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

            if (property.PropertyType == typeof(Guid))
            {
                continue;
            }
            var value = property.GetValue(entity);
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(string))
            {
                result = result + "'" + value + "'";
            }
            else
            {
                result = result + (value ?? "null");
            }

            if (properties.Last() != property)
                result = result + ",";
        }
        return result;
    }

    public string MapUpdeteEntity(T entity)
    {
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
                sql += $"{ress[a]}, ";
            else
                sql += $"{ress[a]} ";

            a++;
        }
        return sql;
    }

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
            var attributes = property.CustomAttributes;

            Type propertytype = property.GetType();
            var type = propertytype.GetTypeInfo();

            if (property.PropertyType == typeof(Guid))
            {
                continue;
            }

            var value = property.GetValue(entity);
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(string))
            {
                result += "'" + value + "'";
            }
            else
            {
                result += (value ?? "null");
            }

            if (properties.Last() != property)
                result = result + ",";
        }
        return result;
    }

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
                    metrics.Add(Map(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return metrics.AsQueryable();
    }

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
                    metrics.Add(Map(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return metrics.AsQueryable();
    }

    public void Create(T item)
    {
        var tableName = GetTableName();
        var columns = GetColumnNames(item);
        var values = MapEntityAnnotations(item);
        var sql = $"INSERT INTO public.{tableName}({columns}) VALUES ({values})";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            connection.Open();

            // Bind the parameter values
            foreach (var property in item.GetType().GetProperties())
            {
                string columnName = property.GetCustomAttribute<ColumnAttribute>()?.Name;
                if (!string.IsNullOrEmpty(columnName))
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = columnName;
                    parameter.Value = property.GetValue(item) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }

            command.ExecuteNonQuery();
        }
    }

    public void Delete(T entity)
    {
        var tableName = GetTableName();
        var sql = $"DELETE From public.{tableName} WHERE id = '{entity.Id}'";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            DbCommand command = (DbCommand)connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            connection.Open();
            DbDataReader reader = command.ExecuteReader();
        }
    }
    //
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

                metrics.Add(Map(reader));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return (IQueryable<T>)metrics.FirstOrDefault();
    }
    //
    public void Update(T entity, Guid Id)
    {
        var tableName = GetTableName();

        string result = MapUpdeteEntity(entity);
        var sql = $"UPDATE  public.{tableName} set {result} WHERE Id = '{Id}'";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            DbCommand command = (DbCommand)connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            connection.Open();
            DbDataReader reader = command.ExecuteReader();

        }
    }
    //
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

                metrics.Add(Map(reader));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }
        }

        return (IQueryable<T>)metrics.FirstOrDefault();
    }
    //
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
            DbDataReader reader = await command.ExecuteReaderAsync();

        }
    }

}
