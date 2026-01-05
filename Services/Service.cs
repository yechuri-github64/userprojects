using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlDataSource = Npgsql.NpgsqlDataSource;
using NpgsqlDataSourceBuilder = Npgsql.NpgsqlDataSourceBuilder;
using DemotestprojLambda.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

#nullable enable

namespace DemotestprojLambda.Services
{
    public class Service
    {
        private readonly string _connString;
        private static NpgsqlDataSource? _dataSource;

        public Service()
        {
            // Load connection string from appsettings or environment
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            _connString = config.GetConnectionString("PostgreSql") ?? Environment.GetEnvironmentVariable("PostgreSql") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_connString))
            {
                Console.WriteLine("PostgreSql connection string not provided");
            }

            EnsureDataSource();
        }

        private void EnsureDataSource()
        {
            if (_dataSource != null) return;
            var builder = new NpgsqlDataSourceBuilder(_connString);
            // Use builtin pooling
            _dataSource = builder.Build();
        }

        public static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            // Register the factory for exact-case enum conversion
            options.Converters.Add(new ExactEnumStringConverterFactory());
            return options;
        }

        public string[] ValidateRequest(Request req)
        {
            var errors = new List<string>();

            // travelcardType is enum; no validation needed beyond parsing

            // travelcardRequestedDate must be in the past
            if (req.travelcardRequestedDate > DateTimeOffset.UtcNow)
                errors.Add("travelcardRequestedDate must be in the past");

            // travelcardValidFrom must be before travelcardValidTo
            if (req.travelcardValidFrom >= req.travelcardValidTo)
                errors.Add("travelcardValidFrom must be earlier than travelcardValidTo");

            // travelcardValidTo must be in the future
            if (req.travelcardValidTo <= DateTimeOffset.UtcNow)
                errors.Add("travelcardValidTo must be in the future");

            // travelcardUsableTo required if SixteenToSeventeen
            if (req.travelcardType == TravelcardType.SixteenToSeventeen)
            {
                if (!req.travelcardUsableTo.HasValue)
                    errors.Add("travelcardUsableTo is required for SixteenToSeventeen type");
                else if (req.travelcardUsableTo <= DateTimeOffset.UtcNow)
                    errors.Add("travelcardUsableTo must be in the future");
            }
            else
            {
                if (req.travelcardUsableTo.HasValue)
                    errors.Add("travelcardUsableTo must be null unless travelcardType is SixteenToSeventeen");
            }

            // travelcardName optional but must match pattern ^[A-Za-z0-9 ]*$ and max 255
            if (!string.IsNullOrEmpty(req.travelcardName))
            {
                if (req.travelcardName.Length > 255) errors.Add("travelcardName must be 255 characters or fewer");
                if (!Regex.IsMatch(req.travelcardName, "^[A-Za-z0-9 ]*$")) errors.Add("travelcardName contains invalid characters");
            }

            // travelcardNumber length and pattern
            if (string.IsNullOrWhiteSpace(req.travelcardNumber)) errors.Add("travelcardNumber is required");
            else
            {
                if (req.travelcardNumber.Length < 11 || req.travelcardNumber.Length > 22) errors.Add("travelcardNumber must be between 11 and 22 characters");
                if (!Regex.IsMatch(req.travelcardNumber, "^[A-Za-z0-9]+$")) errors.Add("travelcardNumber contains invalid characters");
            }

            // travelcardTransactionReference pattern ^\d{2}[A-Z0-9]{4}\d{4}\d{5}$ and length 15
            if (string.IsNullOrWhiteSpace(req.travelcardTransactionReference)) errors.Add("travelcardTransactionReference is required");
            else
            {
                if (req.travelcardTransactionReference.Length != 15) errors.Add("travelcardTransactionReference must be 15 characters");
                if (!Regex.IsMatch(req.travelcardTransactionReference, "^\\d{2}[A-Z0-9]{4}\\d{4}\\d{5}$")) errors.Add("travelcardTransactionReference does not match required pattern");
            }

            // cardholders: exactly 1 or 2; exactly one Primary; optional one Secondary
            if (req.cardholders == null) errors.Add("cardholders is required");
            else
            {
                if (req.cardholders.Count < 1 || req.cardholders.Count > 2) errors.Add("cardholders must contain 1 or 2 items");
                int primaryCount = 0;
                foreach (var ch in req.cardholders)
                {
                    if (ch.cardholderType == CardholderType.Primary) primaryCount++;

                    // title length 1-15 and pattern
                    if (string.IsNullOrWhiteSpace(ch.cardholderTitle) || ch.cardholderTitle.Length > 15) errors.Add("cardholderTitle is required and max 15 chars");
                    else if (!Regex.IsMatch(ch.cardholderTitle, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-žºª .'’\\-]+$")) errors.Add("cardholderTitle contains invalid characters");

                    // forename/surname length and pattern
                    if (string.IsNullOrWhiteSpace(ch.cardholderForename) || ch.cardholderForename.Length > 100) errors.Add("cardholderForename is required and max 100 chars");
                    else if (!Regex.IsMatch(ch.cardholderForename, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-ž .'’\\-]+$")) errors.Add("cardholderForename contains invalid characters");

                    if (string.IsNullOrWhiteSpace(ch.cardholderSurname) || ch.cardholderSurname.Length > 100) errors.Add("cardholderSurname is required and max 100 chars");
                    else if (!Regex.IsMatch(ch.cardholderSurname, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-ž .'’\\-]+$")) errors.Add("cardholderSurname contains invalid characters");

                    // photo name pattern
                    if (string.IsNullOrWhiteSpace(ch.cardholderPhotoName) || ch.cardholderPhotoName.Length > 100) errors.Add("cardholderPhotoName is required and max 100 chars");
                    else if (!Regex.IsMatch(ch.cardholderPhotoName, "^(?!.*[×÷ˇ˘μ])[A-Za-z0-9À-ž _.\\-()\\[\]','&+#]+$")) errors.Add("cardholderPhotoName contains invalid characters");

                    // Exactly one of photo fields must be provided
                    int photoProvided = 0;
                    if (!string.IsNullOrWhiteSpace(ch.cardholderPhotoRRSKey))
                    {
                        photoProvided++;
                        if (ch.cardholderPhotoRRSKey.Length < 39 || ch.cardholderPhotoRRSKey.Length > 42) errors.Add("cardholderPhotoRRSKey must be between 39 and 42 characters");
                        if (!Regex.IsMatch(ch.cardholderPhotoRRSKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$")) errors.Add("cardholderPhotoRRSKey does not match required pattern");
                    }
                    if (!string.IsNullOrWhiteSpace(ch.cardholderPhotoURL))
                    {
                        photoProvided++;
                        if (ch.cardholderPhotoURL.Length < 20 || ch.cardholderPhotoURL.Length > 2048) errors.Add("cardholderPhotoURL must be between 20 and 2048 characters");
                        if (!Regex.IsMatch(ch.cardholderPhotoURL, "^(https?://)[A-Za-z0-9._~:/?#@!$&'()*+,;=%-]+$")) errors.Add("cardholderPhotoURL does not match required pattern");
                    }
                    if (!string.IsNullOrWhiteSpace(ch.cardholderPhotoKey))
                    {
                        photoProvided++;
                        if (ch.cardholderPhotoKey.Length < 39 || ch.cardholderPhotoKey.Length > 42) errors.Add("cardholderPhotoKey must be between 39 and 42 characters");
                        if (!Regex.IsMatch(ch.cardholderPhotoKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$")) errors.Add("cardholderPhotoKey does not match required pattern");
                    }

                    if (photoProvided != 1) errors.Add("Each cardholder must provide exactly one of cardholderPhotoRRSKey, cardholderPhotoURL, or cardholderPhotoKey");
                }

                if (primaryCount != 1) errors.Add("Exactly one Primary cardholder is required");

                // Check secondary allowed for travelcard type
                if (req.cardholders.Count == 2)
                {
                    var allowedSecondary = new HashSet<TravelcardType>
                    {
                        TravelcardType.TwoTogether,
                        TravelcardType.Family,
                        TravelcardType.Network,
                        TravelcardType.DevonandCornwall,
                        TravelcardType.Barcklays
                    };

                    if (!allowedSecondary.Contains(req.travelcardType))
                        errors.Add("Secondary cardholder is not allowed for the specified travelcardType");
                }
            }

            return errors.ToArray();
        }

        public async Task<(string travelcardId, string token)> CreateTravelcardAsync(Request req)
        {
            if (_dataSource == null) throw new InvalidOperationException("Database datasource not initialized");

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Insert travelcard
                var insertTravelcardCmd = conn.CreateCommand();
                insertTravelcardCmd.CommandText = @"
                    INSERT INTO public.travelcards
                    (travelcard_type, travelcard_valid_from, travelcard_valid_to, travelcard_name, travelcard_number, travelcard_requested_date, travelcard_transaction_reference, travelcard_usable_to)
                    VALUES (@type, @validFrom, @validTo, @name, @number, @requestedDate, @txRef, @usableTo)
                    RETURNING id";

                insertTravelcardCmd.Parameters.AddWithValue("@type", NpgsqlTypes.NpgsqlDbType.Text, req.travelcardType.ToString());
                insertTravelcardCmd.Parameters.AddWithValue("@validFrom", req.travelcardValidFrom.UtcDateTime);
                insertTravelcardCmd.Parameters.AddWithValue("@validTo", req.travelcardValidTo.UtcDateTime);
                insertTravelcardCmd.Parameters.AddWithValue("@name", (object?)req.travelcardName ?? DBNull.Value);
                insertTravelcardCmd.Parameters.AddWithValue("@number", req.travelcardNumber);
                insertTravelcardCmd.Parameters.AddWithValue("@requestedDate", req.travelcardRequestedDate.UtcDateTime);
                insertTravelcardCmd.Parameters.AddWithValue("@txRef", req.travelcardTransactionReference);
                insertTravelcardCmd.Parameters.AddWithValue("@usableTo", (object?)req.travelcardUsableTo?.UtcDateTime ?? DBNull.Value);

                var travelcardIdObj = await insertTravelcardCmd.ExecuteScalarAsync();
                if (travelcardIdObj == null) throw new Exception("Failed to insert travelcard");
                var travelcardId = Convert.ToInt32(travelcardIdObj);

                // Insert cardholders
                foreach (var ch in req.cardholders)
                {
                    var insertChCmd = conn.CreateCommand();
                    insertChCmd.CommandText = @"
                        INSERT INTO public.cardholders
                        (travelcard_id, cardholder_title, cardholder_forename, cardholder_surname, cardholder_type, cardholder_photo_name, cardholder_photo_rrs_key, cardholder_photo_url, cardholder_photo_key)
                        VALUES (@travelcardId, @title, @forename, @surname, @type, @photoName, @rrsKey, @url, @key)";

                    insertChCmd.Parameters.AddWithValue("@travelcardId", travelcardId);
                    insertChCmd.Parameters.AddWithValue("@title", ch.cardholderTitle);
                    insertChCmd.Parameters.AddWithValue("@forename", ch.cardholderForename);
                    insertChCmd.Parameters.AddWithValue("@surname", ch.cardholderSurname);
                    insertChCmd.Parameters.AddWithValue("@type", NpgsqlTypes.NpgsqlDbType.Text, ch.cardholderType.ToString());
                    insertChCmd.Parameters.AddWithValue("@photoName", ch.cardholderPhotoName);
                    insertChCmd.Parameters.AddWithValue("@rrsKey", (object?)ch.cardholderPhotoRRSKey ?? DBNull.Value);
                    insertChCmd.Parameters.AddWithValue("@url", (object?)ch.cardholderPhotoURL ?? DBNull.Value);
                    insertChCmd.Parameters.AddWithValue("@key", (object?)ch.cardholderPhotoKey ?? DBNull.Value);

                    await insertChCmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();

                // Build response
                var guid = Guid.NewGuid().ToString();
                var token = GenerateToken(6);
                Console.WriteLine($"Created travelcard id={travelcardId}, guid={guid}");
                return (guid, token);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                Console.WriteLine($"DB error: {ex}");
                throw;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        private static string GenerateToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rnd = new Random();
            var arr = new char[length];
            for (int i = 0; i < length; i++) arr[i] = chars[rnd.Next(chars.Length)];
            return new string(arr);
        }
    }
}
