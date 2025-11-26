Create a new railcard.

Project: createrailcard
Lambda handler: createrailcard (configured in aws-lambda-tools-defaults.json)

Sample request payload (JSON):

{
  "railcardType": "Young",
  "railcardValidFrom": "2025-01-01T00:00:00Z",
  "railcardValidTo": "2026-01-01T00:00:00Z",
  "railcardName": "Young",
  "railcardNumber": "ABC12345678",
  "railcardRequestedDate": "2025-01-01T00:00:00Z",
  "railcardTransactionReference": "12ABCD123412345",
  "cardholders": [
    {
      "cardholderTitle": "Mr",
      "cardholderForename": "John",
      "cardholderSurname": "Doe",
      "cardholderType": "Primary",
      "cardholderPhotoName": "john_doe.jpg",
      "cardholderPhotoURL": "https://example.com/photos/john_doe.jpg"
    }
  ]
}

Response sample:
{
  "railcardId": "f4a3c742-e9c6-4c18-8f4b-b76b377b7574",
  "token": "P5SSY6"
}

Configuration:
- Update appsettings.json ConnectionStrings:PostgreSql with your PostgreSQL connection string.

Notes:
- The service will insert into tables public.railcards and public.railcard_cardholders using Npgsql and mapped enums.
- Errors are returned as structured JSON in the "error" property.
