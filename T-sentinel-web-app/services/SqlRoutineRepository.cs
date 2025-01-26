using Microsoft.Data.SqlClient;

public class SqlRoutineRepository : IRoutineRepository
{
    private readonly string _connectionString;

    public SqlRoutineRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InsertRoutine(Routine routine)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Routine (Commands, CreatedAt) VALUES (@Commands, @CreatedAt)";
                    command.Parameters.AddWithValue("@Commands", routine.Commands);
                    command.Parameters.AddWithValue("@CreatedAt", routine.CreatedAt);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            throw;
        }
    }

    public List<Routine> GetRoutines()
    {
        var routines = new List<Routine>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id, Commands, CreatedAt FROM Routine";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var routine = new Routine
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Commands = reader.GetString(reader.GetOrdinal("Commands")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };
                            routines.Add(routine);
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            throw;
        }

        return routines;
    }
}
