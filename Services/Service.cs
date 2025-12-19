using System.Text.Json;
using System.Text.RegularExpressions;
using CreatetravelcardLambda.Models;
using CreatetravelcardLambda.Models.Enums;
using Npgsql;
using System.Globalization;

namespace CreatetravelcardLambda.Services;

public class Service
{
    private readonly string _connectionString;
    private readonly NpgsqlDataSource _dataSource;

    public Service()
    {
        Console.WriteLine("Service initializing");
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        _connectionString = config.GetConnectionString("PostgreSql") ?? throw new InvalidOperationException("Missing PostgreSql connection string");

        var builder = new NpgsqlDataSourceBuilder(_connectionString);
        // register enums - names must match database enum names
        builder.MapEnum<TravelcardType>("travelcard_type_enum");
        builder.MapEnum<CardholderType>("cardholder_type_enum");
        _dataSource = builder.Build();
    }

    public APIGatewayProxyResponse CreateErrorResponse(int statusCode, string message, IEnumerable<string>? details = null)
    {
        var error = new
        {
            error = message,
            details = details ?? Array.Empty<string>()
        };
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(error),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }

    public IEnumerable<string> ValidateRequest(Request req)
    {
        var errors = new List<string>();

        // travelcardType must be in enum - deserializer handles unknowns typically

        // travelcardValidFrom must be no later than one calendar month from creation date/time. Business wording: it must no later that one calendar month from the Digital Travelcard creation date and time.
        // We interpret creation date = requested date
        var requested = req.TravelcardRequestedDate;
        var maxValidFrom = requested.AddMonths(1);
        if (req.TravelcardValidFrom > maxValidFrom)
            errors.Add("travelcardValidFrom must be no later than one calendar month after travelcardRequestedDate");

        // Check requested_date is in the past
        if (req.TravelcardRequestedDate > DateTimeOffset.UtcNow.AddMinutes(1))
            errors.Add("travelcardRequestedDate must be in the past");

        // Check valid_from date is earlier than valid_to date
        if (req.TravelcardValidFrom >= req.TravelcardValidTo)
            errors.Add("travelcardValidFrom must be earlier than travelcardValidTo");

        // valid_to in the future
        if (req.TravelcardValidTo <= DateTimeOffset.UtcNow)
            errors.Add("travelcardValidTo must be in the future");

        // If SixteenToSeventeen then usable_to required and must be in the future
        if (req.TravelcardType == TravelcardType.SixteenToSeventeen)
        {
            if (!req.TravelcardUsableTo.HasValue)
                errors.Add("travelcardUsableTo is required for SixteenToSeventeen travelcards");
            else if (req.TravelcardUsableTo.Value <= DateTimeOffset.UtcNow)
                errors.Add("travelcardUsableTo must be in the future");
        }
        else
        {
            if (req.TravelcardUsableTo.HasValue)
                errors.Add("travelcardUsableTo must be null unless travelcardType is SixteenToSeventeen");
        }

        // travelcardName optional pattern ^[A-Za-z0-9 ]*$ length <=255
        if (!string.IsNullOrEmpty(req.TravelcardName))
        {
            if (req.TravelcardName.Length > 255)
                errors.Add("travelcardName must be 255 characters or fewer");
            if (!Regex.IsMatch(req.TravelcardName, "^[A-Za-z0-9 ]*$"))
                errors.Add("travelcardName contains invalid characters");
        }

        // travelcardNumber length 11-22 and pattern alnum
        if (string.IsNullOrWhiteSpace(req.TravelcardNumber) || req.TravelcardNumber.Length < 11 || req.TravelcardNumber.Length > 22 || !Regex.IsMatch(req.TravelcardNumber, "^[A-Za-z0-9]+$"))
            errors.Add("travelcardNumber must be 11-22 alphanumeric characters");

        // transaction reference exact 15 chars pattern ^\d{2}[A-Z0-9]{4}\d{4}\d{5}$
        if (string.IsNullOrWhiteSpace(req.TravelcardTransactionReference) || req.TravelcardTransactionReference.Length != 15 || !Regex.IsMatch(req.TravelcardTransactionReference, "^\\d{2}[A-Z0-9]{4}\\d{4}\\d{5}$"))
            errors.Add("travelcardTransactionReference must be 15 characters and match required pattern");

        // cardholders: 1 or 2 items exactly, exactly one Primary; optional one Secondary; each must provide one of photo fields
        if (req.Cardholders == null || req.Cardholders.Count < 1 || req.Cardholders.Count > 2)
            errors.Add("cardholders must contain 1 or 2 items");
        else
        {
            var primaryCount = req.Cardholders.Count(c => c.CardholderType == CardholderType.Primary);
            if (primaryCount != 1)
                errors.Add("There must be exactly one Primary cardholder");

            if (req.Cardholders.Count == 2)
            {
                // secondary allowed only for certain travelcard types? Business rule: "Check secondary cardholder (optional) is allowed for Travelcard type"
                // Not provided which types allow secondary. We'll assume Secondary allowed for all except 'Young' and 'SixteenToSeventeen' and 'SixteenToSeventeen' etc. To be safe, only allow Secondary for Family, TwoTogether, DevonandCornwall, Barcklays, Network, TwentySixToThirty
                var allowedWithSecondary = new[] { TravelcardType.Family, TravelcardType.TwoTogether, TravelcardType.DevonandCornwall, TravelcardType.Barcklays, TravelcardType.Network, TravelcardType.TwentySixToThirty };
                if (!allowedWithSecondary.Contains(req.TravelcardType))
                    errors.Add("Secondary cardholder is not allowed for this travelcard type");
            }

            foreach (var ch in req.Cardholders)
            {
                // title length 1-15 and pattern
                if (string.IsNullOrWhiteSpace(ch.CardholderTitle) || ch.CardholderTitle.Length > 15)
                    errors.Add("cardholderTitle is required and must be 1-15 characters");
                else if (!Regex.IsMatch(ch.CardholderTitle, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-žºª .'’\\-]+$"))
                    errors.Add("cardholderTitle contains invalid characters");

                if (string.IsNullOrWhiteSpace(ch.CardholderForename) || ch.CardholderForename.Length > 100)
                    errors.Add("cardholderForename is required and must be 1-100 characters");
                else if (!Regex.IsMatch(ch.CardholderForename, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-ž .'’\\-]+$"))
                    errors.Add("cardholderForename contains invalid characters");

                if (string.IsNullOrWhiteSpace(ch.CardholderSurname) || ch.CardholderSurname.Length > 100)
                    errors.Add("cardholderSurname is required and must be 1-100 characters");
                else if (!Regex.IsMatch(ch.CardholderSurname, "^(?!.*[×÷ˇ˘μ])[A-Za-zÀ-ž .'’\\-]+$"))
                    errors.Add("cardholderSurname contains invalid characters");

                if (string.IsNullOrWhiteSpace(ch.CardholderPhotoName) || ch.CardholderPhotoName.Length > 100)
                    errors.Add("cardholderPhotoName is required and must be 1-100 characters");
                else if (!Regex.IsMatch(ch.CardholderPhotoName, "^(?!.*[×÷ˇ˘μ])[A-Za-z0-9À-ž _.\\-()\\[\]'',&+#]+$"))
                    errors.Add("cardholderPhotoName contains invalid characters");

                // OneOf for photo keys: at least one of the three must be present
                if (string.IsNullOrWhiteSpace(ch.CardholderPhotoRRSKey) && string.IsNullOrWhiteSpace(ch.CardholderPhotoURL) && string.IsNullOrWhiteSpace(ch.CardholderPhotoKey))
                    errors.Add("Each cardholder must provide one of cardholderPhotoRRSKey, cardholderPhotoURL, or cardholderPhotoKey");

                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoRRSKey))
                {
                    if (ch.CardholderPhotoRRSKey.Length < 39 || ch.CardholderPhotoRRSKey.Length > 42 || !Regex.IsMatch(ch.CardholderPhotoRRSKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$"))
                        errors.Add("cardholderPhotoRRSKey is invalid");
                }

                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoKey))
                {
                    if (ch.CardholderPhotoKey.Length < 39 || ch.CardholderPhotoKey.Length > 42 || !Regex.IsMatch(ch.CardholderPhotoKey, "^[A-Za-z0-9-]{36}\\.[A-Za-z0-9]{2,5}$"))
                        errors.Add("cardholderPhotoKey is invalid");
                }

                if (!string.IsNullOrWhiteSpace(ch.CardholderPhotoURL))
                {
                    if (ch.CardholderPhotoURL.Length < 20 || ch.CardholderPhotoURL.Length > 2048 || !Regex.IsMatch(ch.CardholderPhotoURL, "^(https?://)[A-Za-z0-9._~:/?#@!$&'()*+,;=%-]+$"))
                        errors.Add("cardholderPhotoURL is invalid");
                }
            }
        }

        return errors;
    }

    public async Task<(string travelcardId, string token)> CreateTravelcardAsync(Request req)
    {
        Console.WriteLine("Persisting travelcard to database");

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            // Insert travelcard
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO public.travelcards
(travelcard_type, travelcard_valid_from, travelcard_valid_to, travelcard_name, travelcard_number, travelcard_requested_date, travelcard_transaction_reference, travelcard_usable_to)
VALUES (@type, @validFrom, @validTo, @name, @number, @requested, @txRef, @usableTo)
RETURNING id";
            cmd.Parameters.AddWithValue("@type", NpgsqlTypes.NpgsqlDbType.Enum, req.TravelcardType);
            cmd.Parameters.AddWithValue("@validFrom", req.TravelcardValidFrom);
            cmd.Parameters.AddWithValue("@validTo", req.TravelcardValidTo);
            cmd.Parameters.AddWithValue("@name", (object?)req.TravelcardName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@number", req.TravelcardNumber);
            cmd.Parameters.AddWithValue("@requested", req.TravelcardRequestedDate);
            cmd.Parameters.AddWithValue("@txRef", req.TravelcardTransactionReference);
            cmd.Parameters.AddWithValue("@usableTo", (object?)req.TravelcardUsableTo ?? DBNull.Value);

            var idObj = await cmd.ExecuteScalarAsync();
            var travelcardIdInt = Convert.ToInt32(idObj);

            // Insert cardholders
            foreach (var ch in req.Cardholders)
            {
                var cmd2 = conn.CreateCommand();
                cmd2.CommandText = @"
INSERT INTO public.cardholders
(travelcard_id, cardholder_title, cardholder_forename, cardholder_surname, cardholder_type, cardholder_photo_name, cardholder_photo_rrs_key, cardholder_photo_url, cardholder_photo_key)
VALUES (@tid, @title, @forename, @surname, @type, @photoName, @rrs, @url, @key)";
                cmd2.Parameters.AddWithValue("@tid", travelcardIdInt);
                cmd2.Parameters.AddWithValue("@title", ch.CardholderTitle);
                cmd2.Parameters.AddWithValue("@forename", ch.CardholderForename);
                cmd2.Parameters.AddWithValue("@surname", ch.CardholderSurname);
                cmd2.Parameters.AddWithValue("@type", NpgsqlTypes.NpgsqlDbType.Enum, ch.CardholderType);
                cmd2.Parameters.AddWithValue("@photoName", ch.CardholderPhotoName);
                cmd2.Parameters.AddWithValue("@rrs", (object?)ch.CardholderPhotoRRSKey ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@url", (object?)ch.CardholderPhotoURL ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@key", (object?)ch.CardholderPhotoKey ?? DBNull.Value);

                await cmd2.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();

            var guid = Guid.NewGuid().ToString();
            var token = GenerateToken(6);
            Console.WriteLine($"Created travelcard id={travelcardIdInt} guid={guid} token={token}");
            return (guid, token);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"Database error: {ex}");
            throw;
        }
    }

    private static string GenerateToken(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var rng = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
    }
}
