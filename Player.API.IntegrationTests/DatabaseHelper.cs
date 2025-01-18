using System.Data;
using MySql.Data.MySqlClient;
using Player.Domain.Interfaces;

namespace Player.API.IntegrationTests;

public class DatabaseHelper : IAsyncLifetime
{
    private readonly MySqlConnection _connection;

    private readonly Dictionary<string, DatabaseRecord> _records = [];

    public DatabaseHelper(string connectionString)
    {
        _connection = new MySqlConnection(connectionString);
    }

    public void TrackRecord(long id, string tableName)
    {
        string key = $"{id}_{tableName}";
        if (!_records.ContainsKey(key))
        {
            _records.Add(key, new DatabaseRecord(id, tableName));
        }
    }

    public void TrackRecord<TEntity>(TEntity value)
        where TEntity : IBaseEntity
    {
        TrackRecord(value.Id, value.GetType().Name);
    }

    public async Task DeleteRecordAsync(long id, string tableName)
    {
        MySqlCommand cmd = _connection.CreateCommand();
        cmd.CommandText = $"DELETE FROM {tableName} WHERE Id = @Id";
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task CleanTablesAsync()
    {
        List<DatabaseRecord> records = _records.Reverse()
            .Select(kvp => kvp.Value)
            .ToList();
        foreach ( DatabaseRecord databaseRecord in records)
        {
            await DeleteRecordAsync(databaseRecord.Id, databaseRecord.TableName);
        }
    }

    private record DatabaseRecord(long Id, string TableName);

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _connection.OpenAsync();
    }
}