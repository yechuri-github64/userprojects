using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreaterailcardLambda.Models;
using Npgsql;
using System.Text.RegularExpressions;

namespace CreaterailcardLambda.Services
{
    public class Service
    {
        private readonly string _connectionString;

        public Service(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Response> CreateRailcardAsync(Request request)
        {
            ValidateRequest(request);

            // Map enums and create connection using NpgsqlDataSourceBuilder for enum mapping
            var builder = new NpgsqlDataSourceBuilder(_connectionString);
            builder.MapEnum<CreaterailcardLambda.Models.RailcardType>("railcard_type_enum");
            builder.MapEnum<CreaterailcardLambda.Models.CardholderType>("cardholder_type_enum");
            await using var dataSource = builder.Build();

            await using var conn = await dataSource.OpenConnectionAsync();
            //await conn.OpenAsync();

            await using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Insert railcard
                var insertRailcardSql = @"INSERT INTO public.railcards
(railcard_type, railcard_valid_from, railcard_valid_to, railcard_name, railcard_number, railcard_requested_date, railcard_transaction_reference, railcard_usable_to)
VALUES (@railcard_type, @railcard_valid_from, @railcard_valid_to, @railcard_name, @railcard_number, @railcard_requested_date, @railcard_transaction_reference, @railcard_usable_to)
RETURNING id";

                await using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = insertRailcardSql;
                cmd.Parameters.AddWithValue("railcard_type", request.RailcardType);
                cmd.Parameters.AddWithValue("railcard_valid_from", request.RailcardValidFrom);
                cmd.Parameters.AddWithValue("railcard_valid_to", request.RailcardValidTo);
                cmd.Parameters.AddWithValue("railcard_name", (object?)request.RailcardName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("railcard_number", request.RailcardNumber);
                cmd.Parameters.AddWithValue("railcard_requested_date", request.RailcardRequestedDate);
                cmd.Parameters.AddWithValue("railcard_transaction_reference", request.RailcardTransactionReference);
                cmd.Parameters.AddWithValue("railcard_usable_to", (object?)request.RailcardUsableTo ?? DBNull.Value);

                var idObj = await cmd.ExecuteScalarAsync();
                var railcardIdInt = Convert.ToInt32(idObj);

                // Insert cardholders
                var insertCardholderSql = @"INSERT INTO public.railcard_cardholders
(railcard_id, cardholder_title, cardholder_forename, cardholder_surname, cardholder_type, cardholder_photo_name, cardholder_photo_rrs_key, cardholder_photo_url, cardholder_photo_key)
VALUES (@railcard_id, @cardholder_title, @cardholder_forename, @cardholder_surname, @cardholder_type, @cardholder_photo_name, @cardholder_photo_rrs_key, @cardholder_photo_url, @cardholder_photo_key)";

                foreach (var ch in request.Cardholders)
                {
                    await using var cmdCh = conn.CreateCommand();
                    cmdCh.Transaction = tx;
                    cmdCh.CommandText = insertCardholderSql;
                    cmdCh.Parameters.AddWithValue("railcard_id", railcardIdInt);
                    cmdCh.Parameters.AddWithValue("cardholder_title", ch.CardholderTitle);
                    cmdCh.Parameters.AddWithValue("cardholder_forename", ch.CardholderForename);
                    cmdCh.Parameters.AddWithValue("cardholder_surname", ch.CardholderSurname);
                    cmdCh.Parameters.AddWithValue("cardholder_type", ch.CardholderType);
                    cmdCh.Parameters.AddWithValue("cardholder_photo_name", ch.CardholderPhotoName);
                    cmdCh.Parameters.AddWithValue("cardholder_photo_rrs_key", (object?)ch.CardholderPhotoRRSKey ?? DBNull.Value);
                    cmdCh.Parameters.AddWithValue("cardholder_photo_url", (object?)ch.CardholderPhotoURL ?? DBNull.Value);
                    cmdCh.Parameters.AddWithValue("cardholder_photo_key", (object?)ch.CardholderPhotoKey ?? DBNull.Value);

                    await cmdCh.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();

                // Return a UUID and a short token as response (railcardId sample uses UUID)
                var response = new Response
                {
                    RailcardId = Guid.NewGuid().ToString(),
                    Token = GenerateToken(6)
                };

                return response;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private void ValidateRequest(Request request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // railcard number length
            if (string.IsNullOrWhiteSpace(request.RailcardNumber)) throw new ArgumentException("railcardNumber is required");
            if (request.RailcardNumber.Length < 11 || request.RailcardNumber.Length > 22) throw new ArgumentException("railcardNumber length must be between 11 and 22");
            if (!Regex.IsMatch(request.RailcardNumber, "^[A-Za-z0-9]+$")) throw new ArgumentException("railcardNumber has invalid characters");

            // transaction reference
            if (string.IsNullOrWhiteSpace(request.RailcardTransactionReference)) throw new ArgumentException("railcardTransactionReference is required");
            if (request.RailcardTransactionReference.Length != 15 || !Regex.IsMatch(request.RailcardTransactionReference, "^\\d{2}[A-Z0-9]{4}\\d{4}\\d{5}$")) throw new ArgumentException("railcardTransactionReference invalid format");

            // railcard usable to required for SixteenToSeventeen
            if (request.RailcardType == Models.RailcardType.SixteenToSeventeen && !request.RailcardUsableTo.HasValue)
                throw new ArgumentException("railcardUsableTo is required when railcardType is SixteenToSeventeen");

            // cardholders count and rules
            if (request.Cardholders == null) throw new ArgumentException("cardholders are required");
            if (request.Cardholders.Count < 1 || request.Cardholders.Count > 2) throw new ArgumentException("cardholders must be 1 or 2 items");

            var primaryCount = 0;
            foreach (var ch in request.Cardholders)
            {
                if (ch.CardholderType == Models.CardholderType.Primary) primaryCount++;

                if (string.IsNullOrWhiteSpace(ch.CardholderTitle) || ch.CardholderTitle.Length > 15) throw new ArgumentException("cardholderTitle invalid");
                if (string.IsNullOrWhiteSpace(ch.CardholderForename) || ch.CardholderForename.Length > 100) throw new ArgumentException("cardholderForename invalid");
                if (string.IsNullOrWhiteSpace(ch.CardholderSurname) || ch.CardholderSurname.Length > 100) throw new ArgumentException("cardholderSurname invalid");
                if (string.IsNullOrWhiteSpace(ch.CardholderPhotoName) || ch.CardholderPhotoName.Length > 100) throw new ArgumentException("cardholderPhotoName invalid");

                // one of the photo fields must be provided
                var provided = 0;
                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoRRSKey)) provided++;
                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoURL)) provided++;
                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoKey)) provided++;
                if (provided != 1) throw new ArgumentException("Each cardholder must provide exactly one photo field (cardholderPhotoRRSKey, cardholderPhotoURL or cardholderPhotoKey)");

                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoRRSKey))
                {
                    if (ch.CardholderPhotoRRSKey.Length < 39 || ch.CardholderPhotoRRSKey.Length > 42) throw new ArgumentException("cardholderPhotoRRSKey length invalid");
                    if (!Regex.IsMatch(ch.CardholderPhotoRRSKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$")) throw new ArgumentException("cardholderPhotoRRSKey invalid pattern");
                }

                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoKey))
                {
                    if (ch.CardholderPhotoKey.Length < 39 || ch.CardholderPhotoKey.Length > 42) throw new ArgumentException("cardholderPhotoKey length invalid");
                    if (!Regex.IsMatch(ch.CardholderPhotoKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$")) throw new ArgumentException("cardholderPhotoKey invalid pattern");
                }

                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoURL))
                {
                    if (ch.CardholderPhotoURL.Length < 20 || ch.CardholderPhotoURL.Length > 2048) throw new ArgumentException("cardholderPhotoURL length invalid");
                    if (!Regex.IsMatch(ch.CardholderPhotoURL, "^[Hh][Tt][Tt][Pp][Ss]:\\/\\/[A-Za-z0-9._~:\\/\\?#@!$&'()*+,;=%-]+$")) throw new ArgumentException("cardholderPhotoURL invalid pattern");
                }
            }

            if (primaryCount != 1) throw new ArgumentException("Exactly one Primary cardholder is required");
        }

        private static string GenerateToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var data = new byte[length];
            rng.GetBytes(data);
            var result = new char[length];
            for (int i = 0; i < length; i++) result[i] = chars[data[i] % chars.Length];
            return new string(result);
        }
    }
}
