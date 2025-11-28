using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using TestfromdocspostgresqlLambda.Models;

namespace TestfromdocspostgresqlLambda.Services
{
    public class Service
    {
        private readonly string _connectionString;

        public Service(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgreSql") ?? string.Empty;
            if (string.IsNullOrEmpty(_connectionString))
                Console.WriteLine("Warning: PostgreSql connection string not configured.");
        }

        public List<string> ValidateRequest(Request req)
        {
            var errors = new List<string>();

            // railcardType - enum validated by deserializer

            // ValidFrom must be no later than one calendar month from creation (requested date)
            if (req.railcardRequestedDate != default && req.railcardValidFrom > req.railcardRequestedDate.AddMonths(1))
            {
                errors.Add("railcardValidFrom must be no later than one calendar month from railcardRequestedDate.");
            }

            // date-time fields
            // railcardValidTo must be present and after validFrom
            if (req.railcardValidTo <= req.railcardValidFrom)
                errors.Add("railcardValidTo must be after railcardValidFrom.");

            // railcardName optional pattern: ^[A-Za-z0-9 ]*$, max 255
            if (!string.IsNullOrEmpty(req.railcardName))
            {
                if (req.railcardName.Length > 255) errors.Add("railcardName exceeds maximum length of 255.");
                if (!Regex.IsMatch(req.railcardName, "^[A-Za-z0-9 ]*$")) errors.Add("railcardName pattern invalid.");
            }

            // railcardNumber length 11-22 and pattern
            if (string.IsNullOrEmpty(req.railcardNumber)) errors.Add("railcardNumber is required.");
            else
            {
                if (req.railcardNumber.Length < 11 || req.railcardNumber.Length > 22) errors.Add("railcardNumber must be between 11 and 22 characters.");
                if (!Regex.IsMatch(req.railcardNumber, "^[A-Za-z0-9]+$")) errors.Add("railcardNumber pattern invalid.");
            }

            // railcardTransactionReference exactly 15 chars and pattern ^\d{2}[A-Z0-9]{4}\d{4}\d{5}$
            if (string.IsNullOrEmpty(req.railcardTransactionReference)) errors.Add("railcardTransactionReference is required.");
            else
            {
                if (req.railcardTransactionReference.Length != 15) errors.Add("railcardTransactionReference must be exactly 15 characters.");
                if (!Regex.IsMatch(req.railcardTransactionReference, "^\\d{2}[A-Z0-9]{4}\\d{4}\\d{5}$")) errors.Add("railcardTransactionReference pattern invalid.");
            }

            // railcardUsableTo required only if SixteenToSeventeen
            if (req.railcardType == RailcardType.SixteenToSeventeen && req.railcardUsableTo == null)
                errors.Add("railcardUsableTo is required for SixteenToSeventeen type.");

            // cardholders: 1 or 2 items only; exactly one Primary required; optional Secondary
            if (req.cardholders == null || req.cardholders.Count < 1 || req.cardholders.Count > 2)
                errors.Add("cardholders must contain 1 or 2 items.");
            else
            {
                int primaryCount = 0;
                foreach (var ch in req.cardholders)
                {
                    if (ch.cardholderType == CardholderType.Primary) primaryCount++;

                    // title
                    if (string.IsNullOrEmpty(ch.cardholderTitle) || ch.cardholderTitle.Length > 15)
                        errors.Add("cardholderTitle is required and max length 15.");
                    else if (!Regex.IsMatch(ch.cardholderTitle, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-žºª .'’\\-]+$"))
                        errors.Add("cardholderTitle contains invalid characters.");

                    // forename
                    if (string.IsNullOrEmpty(ch.cardholderForename) || ch.cardholderForename.Length > 100)
                        errors.Add("cardholderForename is required and max length 100.");
                    else if (!Regex.IsMatch(ch.cardholderForename, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-ž .'’\\-]+$"))
                        errors.Add("cardholderForename contains invalid characters.");

                    // surname
                    if (string.IsNullOrEmpty(ch.cardholderSurname) || ch.cardholderSurname.Length > 100)
                        errors.Add("cardholderSurname is required and max length 100.");
                    else if (!Regex.IsMatch(ch.cardholderSurname, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-ž .'’\\-]+$"))
                        errors.Add("cardholderSurname contains invalid characters.");

                    // photo name
                    if (string.IsNullOrEmpty(ch.cardholderPhotoName) || ch.cardholderPhotoName.Length > 100)
                        errors.Add("cardholderPhotoName is required and max length 100.");
                    else if (!Regex.IsMatch(ch.cardholderPhotoName, "^(?!.*[×÷ˇ˘μ])[A-Za-z0-9À-ž _.\\-()\\[\]',&+#]+$"))
                        errors.Add("cardholderPhotoName contains invalid characters.");

                    // image: one of three
                    int provided = 0;
                    if (!string.IsNullOrEmpty(ch.cardholderPhotoRRSKey))
                    {
                        provided++;
                        if (ch.cardholderPhotoRRSKey.Length < 39 || ch.cardholderPhotoRRSKey.Length > 42) errors.Add("cardholderPhotoRRSKey length invalid.");
                        if (!Regex.IsMatch(ch.cardholderPhotoRRSKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$")) errors.Add("cardholderPhotoRRSKey pattern invalid.");
                    }
                    if (!string.IsNullOrEmpty(ch.cardholderPhotoURL))
                    {
                        provided++;
                        if (ch.cardholderPhotoURL.Length < 20 || ch.cardholderPhotoURL.Length > 2048) errors.Add("cardholderPhotoURL length invalid.");
                        if (!Regex.IsMatch(ch.cardholderPhotoURL, "^(https?://)[A-Za-z0-9._~:/?#@!$&'()*+,;=%-]+$")) errors.Add("cardholderPhotoURL pattern invalid.");
                    }
                    if (!string.IsNullOrEmpty(ch.cardholderPhotoKey))
                    {
                        provided++;
                        if (ch.cardholderPhotoKey.Length < 39 || ch.cardholderPhotoKey.Length > 42) errors.Add("cardholderPhotoKey length invalid.");
                        if (!Regex.IsMatch(ch.cardholderPhotoKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$")) errors.Add("cardholderPhotoKey pattern invalid.");
                    }
                    if (provided == 0) errors.Add("Each cardholder must provide one of cardholderPhotoRRSKey, cardholderPhotoURL, or cardholderPhotoKey.");
                    if (provided > 1) errors.Add("Only one of cardholderPhotoRRSKey, cardholderPhotoURL, cardholderPhotoKey must be provided.");
                }
                if (primaryCount != 1) errors.Add("Exactly one Primary cardholder is required.");
            }

            return errors;
        }

        public async Task<(string railcardId, string token)> SaveAsync(Request req)
        {
            // Generate ids to return
            var railcardGuid = Guid.NewGuid().ToString();
            var token = GenerateToken();

            Console.WriteLine("Saving to PostgreSQL...");

            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("Connection string empty - skipping DB save (for safety in this environment). Returning generated values.");
                return (railcardGuid, token);
            }

            await using var dataSource = BuildDataSource();
            await using var conn = await dataSource.OpenConnectionAsync();

            // Begin transaction
            await using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Insert railcard and return id
                await using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"INSERT INTO railcards(railcard_type, railcard_valid_from, railcard_valid_to, railcard_name, railcard_number, railcard_requested_date, railcard_transaction_reference, railcard_usable_to)
                                        VALUES (@type, @validFrom, @validTo, @name, @number, @requestedDate, @transactionReference, @usableTo)
                                        RETURNING id";
                    cmd.Parameters.AddWithValue("@type", NpgsqlDbType.Unknown, req.railcardType.ToString());
                    cmd.Parameters.AddWithValue("@validFrom", req.railcardValidFrom);
                    cmd.Parameters.AddWithValue("@validTo", req.railcardValidTo);
                    cmd.Parameters.AddWithValue("@name", (object?)req.railcardName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@number", req.railcardNumber);
                    cmd.Parameters.AddWithValue("@requestedDate", req.railcardRequestedDate);
                    cmd.Parameters.AddWithValue("@transactionReference", req.railcardTransactionReference);
                    cmd.Parameters.AddWithValue("@usableTo", (object?)req.railcardUsableTo ?? DBNull.Value);

                    var insertedIdObj = await cmd.ExecuteScalarAsync();
                    var insertedId = Convert.ToInt32(insertedIdObj);

                    // Insert cardholders
                    foreach (var ch in req.cardholders)
                    {
                        await using var cmd2 = conn.CreateCommand();
                        cmd2.Transaction = tx;
                        cmd2.CommandText = @"INSERT INTO railcard_cardholders(railcard_id, cardholder_title, cardholder_forename, cardholder_surname, cardholder_type, cardholder_photo_name, cardholder_photo_rrs_key, cardholder_photo_url, cardholder_photo_key)
                                             VALUES (@rid, @title, @forename, @surname, @type, @photoName, @photoRRS, @photoURL, @photoKey)";
                        cmd2.Parameters.AddWithValue("@rid", insertedId);
                        cmd2.Parameters.AddWithValue("@title", ch.cardholderTitle);
                        cmd2.Parameters.AddWithValue("@forename", ch.cardholderForename);
                        cmd2.Parameters.AddWithValue("@surname", ch.cardholderSurname);
                        cmd2.Parameters.AddWithValue("@type", NpgsqlDbType.Unknown, ch.cardholderType.ToString());
                        cmd2.Parameters.AddWithValue("@photoName", ch.cardholderPhotoName);
                        cmd2.Parameters.AddWithValue("@photoRRS", (object?)ch.cardholderPhotoRRSKey ?? DBNull.Value);
                        cmd2.Parameters.AddWithValue("@photoURL", (object?)ch.cardholderPhotoURL ?? DBNull.Value);
                        cmd2.Parameters.AddWithValue("@photoKey", (object?)ch.cardholderPhotoKey ?? DBNull.Value);

                        await cmd2.ExecuteNonQueryAsync();
                    }
                }

                await tx.CommitAsync();
                Console.WriteLine("Database save committed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex}");
                try { await tx.RollbackAsync(); } catch { }
                throw;
            }

            return (railcardGuid, token);
        }

        private NpgsqlDataSource BuildDataSource()
        {
            var builder = new NpgsqlDataSourceBuilder(_connectionString);
            // Map enums to PostgreSQL enum types
            try
            {
                builder.MapEnum<RailcardType>("railcard_type_enum");
                builder.MapEnum<CardholderType>("cardholder_type_enum");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Enum mapping warning: {ex}");
            }
            return builder.Build();
        }

        private string GenerateToken()
        {
            // Short token
            var rng = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var buff = new char[6];
            for (int i = 0; i < buff.Length; i++) buff[i] = chars[rng.Next(chars.Length)];
            return new string(buff);
        }
    }
}
