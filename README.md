# Healthcare Document Exchange

ASP.NET Core REST API for exchanging patient documents and healthcare information between care providers.

Built for a case where a hospital refers a patient to a nursing home. The hospital sends a referral letter with the patient's allergies, and the nursing home then asks the hospital for the patient's medication.

Each care provider runs its own instance of the API, called a party.

## Disclaimer

This project is built for demonstration purposes only. Things a production system needs, such as authentication and a database, are left out on purpose. All patient data is fake and reset each time the API starts.


## Project structure

```text
src/DocumentExchange.Api/
  Contracts/        Request and response models for endpoints
  Data/             Repositories, in-memory database and seed data
  Endpoints/        The minimal API endpoints
  Hosted/           Seeding of test data on startup
  Identification/   Identity specifications (X-Identity header)
  Models/           Data models stored in-memory
  Utils/            BSN validation and parsing of endpoint data
  Validation/       Validation attributes for endpoints
test/
  DocumentExchange.UnitTests/          Unit testing
  DocumentExchange.IntegrationTests/   Integration testing
```

## Endpoints

- `POST /api/referrals` receives a referral letter. The patient in it is added, or merged when already known.
- `GET /api/patients/{bsn}` returns a patient. Add `?include` for extra information. Supports `allergies` and `medications`.

Every request needs an `X-Identity` header that says who is calling. Requests that miss identification will be rejected.

The OpenAPI document is available at `/openapi/v1.json` when running in development.

## Getting started

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

See [global.json](global.json) for more information on specific versioning.

Run `start-project.cmd` to build the API and start two parties, each in its own window:

| Party   | URL                   | Remark                     |
| ------- | --------------------- | -------------------------- |
| Party A | http://localhost:5100 | Seeds with a test patient  |
| Party B | http://localhost:5200 |                            |

Alternatively use Visual Studio or another editor to call and debug the API directly.

The information set is specified by [launchsettings.json](src/DocumentExchange.Api/Properties/launchSettings.json).

You can modify the settings directly, or set them separately as environment variables.

| Setting        | Description                                                  |
| -------------- | -------------------------------------------------------------|
| `SeedDatabase` | Adds the test patient on startup. Only works in development. |
| `LogFilePath`  | Where to write JSON logs. If empty, no file is written to.   |

## Trying it out

The [http file](src/DocumentExchange.Api/DocumentExchange.Api.http) has requests for all endpoints, including intentionally failing requests.

Endpoints can be accessed based on party by configuring the variables on top.

Each party logs what it received or shared, in its own window and in `logs/`. The format of these logs is in JSON, and written by Serilog.

## Testing

```cmd
dotnet test
```

The unit tests cover the internal logic, including validation and general edge cases that might occur.

The integration tests run the API in memory and call the endpoints over its own running instance.