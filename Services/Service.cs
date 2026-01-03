using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TestLambda.Models;
using MySqlConnector;
using Npgsql;

namespace TestLambda.Services
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class Service
    {
        private readonly string _mySqlConn;
        private readonly string _pgConn;

        public Service()
        {
            // Read connection strings from environment if present, else from default appsettings fallback
            _mySqlConn = Environment.GetEnvironmentVariable("MYSQL__CONNECTIONSTRING") ?? "Server=localhost;Database=testdb;User=root;Password=YourPassword;";
            _pgConn = Environment.GetEnvironmentVariable("POSTGRES__CONNECTIONSTRING") ?? "Host=localhost;Database=testdb;Username=postgres;Password=YourPassword";

            Console.WriteLine("Service initialized");
        }

        public ValidationResult ValidateRequest(Request req)
        {
            var res = new ValidationResult { IsValid = true };
            if (string.IsNullOrWhiteSpace(req.TeamName))
            {
                res.IsValid = false;
                res.Errors.Add("TeamName is required");
            }

            if (req.TeamName.Length > 100)
            {
                res.IsValid = false;
                res.Errors.Add("TeamName exceeds maximum length of 100");
            }

            if (req.Players == null || req.Players.Count == 0)
            {
                res.IsValid = false;
                res.Errors.Add("At least one player with goals must be provided");
            }
            else
            {
                foreach (var p in req.Players)
                {
                    if (p.PlayerId <= 0)
                    {
                        res.IsValid = false;
                        res.Errors.Add($"Invalid PlayerId: {p.PlayerId}");
                    }

                    if (p.Goals < 0)
                    {
                        res.IsValid = false;
                        res.Errors.Add($"Goals cannot be negative for PlayerId: {p.PlayerId}");
                    }
                }
            }

            return res;
        }

        // Demonstrates using NpgsqlDataSourceBuilder and opening a connection with enum mappings
        public async Task<NpgsqlConnection> OpenPostgresConnectionAsync()
        {
            try
            {
                Console.WriteLine("Initializing Postgres data source builder");
                var builder = new NpgsqlDataSourceBuilder(_pgConn);
                // If you have enum types to map, example:
                // builder.AddEnum<Models.TeamCategory>("team_category"); // requires matching PG enum
                var datasource = builder.Build();
                var conn = await datasource.OpenConnectionAsync();
                Console.WriteLine("Postgres connection opened");
                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening Postgres connection: {ex}");
                throw;
            }
        }

        // Main processing logic: create team and add player goal records using MySQL downstream
        public async Task<Response> ProcessTeamAsync(Request req)
        {
            var resp = new Response { Success = false };
            try
            {
                // Validate again defensively
                var v = ValidateRequest(req);
                if (!v.IsValid)
                {
                    resp.Errors = v.Errors;
                    return resp;
                }

                // Create team in MySQL and record player goals
                using var conn = new MySqlConnection(_mySqlConn);
                await conn.OpenAsync();
                using var tx = await conn.BeginTransactionAsync();
                try
                {
                    Console.WriteLine("Inserting team record");
                    // Insert team and get inserted id
                    var insertTeamCmd = conn.CreateCommand();
                    insertTeamCmd.Transaction = tx;
                    insertTeamCmd.CommandText = "INSERT INTO teams (name, category) VALUES (@name, @category); SELECT LAST_INSERT_ID();";
                    insertTeamCmd.Parameters.Add(new MySqlParameter("@name", req.TeamName));
                    insertTeamCmd.Parameters.Add(new MySqlParameter("@category", req.Category.ToString()));
                    var obj = await insertTeamCmd.ExecuteScalarAsync();
                    var teamId = Convert.ToInt32(obj);

                    Console.WriteLine($"Inserted team with id {teamId}");

                    // For each player, optionally verify player exists in player table then insert goal record
                    foreach (var p in req.Players)
                    {
                        Console.WriteLine($"Processing player {p.PlayerId} goals {p.Goals}");

                        // Check player exists
                        var chkCmd = conn.CreateCommand();
                        chkCmd.Transaction = tx;
                        chkCmd.CommandText = "SELECT COUNT(1) FROM player WHERE id = @pid";
                        chkCmd.Parameters.Add(new MySqlParameter("@pid", p.PlayerId));
                        var cntObj = await chkCmd.ExecuteScalarAsync();
                        var cnt = Convert.ToInt32(cntObj);
                        if (cnt == 0)
                        {
                            // Rollback and return error
                            await tx.RollbackAsync();
                            resp.Errors.Add($"Player with id {p.PlayerId} does not exist");
                            return resp;
                        }

                        // Insert into team_player_goals (example table)
                        var insCmd = conn.CreateCommand();
                        insCmd.Transaction = tx;
                        insCmd.CommandText = "INSERT INTO team_player_goals (team_id, player_id, goals) VALUES (@tid, @pid, @goals)";
                        insCmd.Parameters.Add(new MySqlParameter("@tid", teamId));
                        insCmd.Parameters.Add(new MySqlParameter("@pid", p.PlayerId));
                        insCmd.Parameters.Add(new MySqlParameter("@goals", p.Goals));
                        await insCmd.ExecuteNonQueryAsync();
                    }

                    await tx.CommitAsync();
                    resp.Success = true;
                    resp.TeamId = teamId;
                    resp.Message = "Team created and player goals recorded";
                    return resp;
                }
                catch (Exception exInner)
                {
                    Console.WriteLine($"DB operation failed: {exInner}");
                    try
                    {
                        await tx.RollbackAsync();
                    }
                    catch (Exception rbEx)
                    {
                        Console.WriteLine($"Rollback failed: {rbEx}");
                    }

                    resp.Errors.Add(exInner.Message);
                    return resp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProcessTeamAsync fatal: {ex}");
                resp.Errors.Add(ex.Message);
                return resp;
            }
        }
    }
}
