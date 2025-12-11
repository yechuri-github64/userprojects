AWS Lambda function that accepts a JSON body with a dob field and returns the elapsed time since that DOB in days, weeks, minutes, and seconds.

Usage (API Gateway proxy example):
Send POST with JSON body: {"dob":"1990-05-15"}
Response: { "success": true, "data": { "dob":"1990-05-15", "calculatedAt":"...", "days":..., "weeks":..., "minutes":..., "seconds":..., "message":"..." } }

Project structure follows clean architecture with handlers, services, and utils.

Errors return success: false with appropriate HTTP status codes.