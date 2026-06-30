# Test Cases

Test cases are implemented as automated xUnit tests in `AssetMgmt-WebApi.Tests`
(`UserServiceTests.cs`, `AssetMasterServiceTests.cs`), summarized below, plus manual
API test requests in `AssetMgmt-WebApi/AssetMgmt-WebApi.http`.

## Module A - User Registration and Login

| # | Scenario | Expected Result | Automated? |
|---|----------|------------------|------------|
| A1 | Register with a username that already exists | Registration rejected with "Username already exists." | Yes |
| A2 | Register with an invalid UserType (not Admin/Purchase Manager) | Registration rejected, validation message returned | Yes |
| A3 | Register with valid, unique data | User + Login records created, success message returned | Yes |
| A4 | Login with a username that doesn't exist | Returns null / 401 Unauthorized | Yes |
| A5 | Login with correct username but wrong password | Returns null / 401 Unauthorized | Yes |
| A6 | Login with correct username and password | Returns a signed JWT token, user type, and full name | Yes |
| A7 | Access a protected endpoint (e.g. `/api/assets`) without a token | Returns 401 Unauthorized | Manual (.http) |
| A8 | Access a protected endpoint with an expired/invalid token | Returns 401 Unauthorized | Manual |

## Module E - Asset Creation

| # | Scenario | Expected Result | Automated? |
|---|----------|------------------|------------|
| E1 | Create an asset linked to a Purchase Order whose status is **not** 'Asset Details registered internally' | Creation rejected with a descriptive error message | Yes |
| E2 | Create an asset with a serial number that already exists | Creation rejected ("serial number already exists") | Yes |
| E3 | Create an asset linked to an eligible Purchase Order | Asset created successfully, returned with joined Asset Type / Vendor / Asset Definition names | Yes |
| E4 | Create a stand-alone asset (no Purchase Order link) | Asset created successfully without requiring PO eligibility checks | Yes |
| E5 | Delete a non-existent asset | Returns `false` / 404 Not Found | Yes |
| E6 | Search assets by partial serial number / model / vendor / type | Returns only matching assets | Yes |
| E7 | Update an asset with a serial number used by another asset | Update rejected | Manual (mirrors E2 logic) |
| E8 | Delete an asset as a non-Admin user | Returns 403 Forbidden (role-based authorization) | Manual (.http, requires Purchase Manager token) |
| E9 | Create/Update an asset as an unauthenticated user | Returns 401 Unauthorized | Manual (.http) |

## Running the automated tests

```bash
cd AssetMgmt-WebApi.Tests
dotnet test
```

## Running the manual tests

Open `AssetMgmt-WebApi/AssetMgmt-WebApi.http` in Visual Studio / VS Code (with the
REST Client extension) or import the requests into Postman, after starting the API
with `dotnet run`.
