# Payment Utility API

A robust RESTful API built with ASP.NET Core 8 for managing utility payments including airtime, data, cable TV, electricity, and more.

---

## Tech Stack

- **Framework:** ASP.NET Core 8
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer Token
- **Documentation:** Swagger UI

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio 2022

### Setup
1. Clone the repository
   ```bash
   git clone https://github.com/dashlin23/Project_utility_backend.git
   ```

2. Create your `appsettings.Development.json` in the `PaymentUtility` project:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Your SQL Server connection string here"
     },
     "Jwt": {
       "Key": "Your secret key here"
     }
   }
   ```

3. Run migrations
   ```bash
   Update-Database -Project Logic
   ```

4. Run the project and open Swagger UI at `https://localhost:{port}/swagger`

---

## Authentication

All protected endpoints require a Bearer token in the header:
```
Authorization: Bearer {your_token}
```

Get your token by calling the Login endpoint.

---

## API Endpoints

### 🔐 Authentication
> Base URL: `/api/Auth`

#### Register
```
POST /api/Auth/register
```
**Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "Test@1234",
  "confirmPassword": "Test@1234"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Account created successfully",
  "token": null
}
```

---

#### Login
```
POST /api/Auth/login
```
**Body:**
```json
{
  "email": "john@example.com",
  "password": "Test@1234"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGci..."
}
```

---

#### Logout
```
POST /api/Auth/logout
```
🔒 **Requires:** Bearer Token

**Response:**
```json
{
  "success": true,
  "message": "Logged out successfully",
  "token": null
}
```

---

#### Forgot Password
```
POST /api/Auth/forgot-password
```
**Body:**
```json
{
  "email": "john@example.com"
}
```
**Response:**
```json
{
  "success": true,
  "message": "If your email exists you will receive a reset link",
  "token": "reset_token_here"
}
```

---

#### Reset Password
```
POST /api/Auth/reset-password
```
**Body:**
```json
{
  "token": "reset_token_here",
  "newPassword": "NewTest@1234",
  "confirmPassword": "NewTest@1234"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Password reset successfully",
  "token": null
}
```

---

#### Change Password
```
POST /api/Auth/change-password
```
🔒 **Requires:** Bearer Token

**Body:**
```json
{
  "currentPassword": "Test@1234",
  "newPassword": "NewTest@1234",
  "confirmPassword": "NewTest@1234"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Password changed successfully",
  "token": null
}
```

---

### 👤 User
> Base URL: `/api/User`

🔒 **All endpoints require Bearer Token**

#### Get Profile
```
GET /api/User/profile
```
**Response:**
```json
{
  "success": true,
  "message": "Profile retrieved successfully",
  "data": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "createdAt": "2026-06-05T12:01:41"
  }
}
```

---

#### Delete Account
```
DELETE /api/User/delete-account
```
**Response:**
```json
{
  "success": true,
  "message": "Account deleted successfully",
  "data": null
}
```

---

### 🔢 Transaction PIN
> Base URL: `/api/TransactionPin`

🔒 **All endpoints require Bearer Token**

#### Create PIN
```
POST /api/TransactionPin/create
```
**Body:**
```json
{
  "pin": "1234",
  "confirmPin": "1234"
}
```
**Response:**
```json
{
  "success": true,
  "message": "PIN created successfully"
}
```

---

#### Change PIN
```
PUT /api/TransactionPin/change
```
**Body:**
```json
{
  "currentPin": "1234",
  "newPin": "5678",
  "confirmNewPin": "5678"
}
```
**Response:**
```json
{
  "success": true,
  "message": "PIN changed successfully"
}
```

---

#### Verify PIN
```
POST /api/TransactionPin/verify
```
**Body:**
```json
{
  "pin": "1234"
}
```
**Response:**
```json
{
  "success": true,
  "message": "PIN verified successfully"
}
```

---

### 🔔 Notifications
> Base URL: `/api/Notification`

🔒 **All endpoints require Bearer Token**

#### Get Notifications
```
GET /api/Notification
```
**Response:**
```json
{
  "success": true,
  "message": "Notifications retrieved successfully",
  "data": [
    {
      "id": 1,
      "title": "Welcome!",
      "message": "Your account has been created successfully.",
      "isRead": false,
      "createdAt": "2026-06-09T09:24:17"
    }
  ]
}
```

---

#### Mark Notification as Read
```
PUT /api/Notification/{notificationId}/mark-as-read
```
**Response:**
```json
{
  "success": true,
  "message": "Notification marked as read"
}
```

---

### 📱 Airtime
> Base URL: `/api/Airtime`

🔒 **All endpoints require Bearer Token**

#### Get Networks
```
GET /api/Airtime/networks
```
**Response:**
```json
{
  "success": true,
  "message": "Networks retrieved successfully",
  "data": [
    { "id": 1, "name": "MTN", "code": "mtn" },
    { "id": 2, "name": "Airtel", "code": "airtel" },
    { "id": 3, "name": "Glo", "code": "glo" },
    { "id": 4, "name": "9mobile", "code": "9mobile" }
  ]
}
```

---

#### Purchase Airtime
```
POST /api/Airtime/purchase
```
**Body:**
```json
{
  "network": "MTN",
  "phoneNumber": "08012345678",
  "amount": 100
}
```
**Response:**
```json
{
  "success": true,
  "message": "Airtime purchased successfully",
  "data": {
    "id": 1,
    "network": "MTN",
    "phoneNumber": "08012345678",
    "amount": 100,
    "status": "Success",
    "reference": "AIR-639165952878163773",
    "createdAt": "2026-06-09T09:48:07Z"
  }
}
```

---

#### Airtime History
```
GET /api/Airtime/history
```
**Response:**
```json
{
  "success": true,
  "message": "Airtime history retrieved successfully",
  "data": [...]
}
```

---

### 📶 Data
> Base URL: `/api/Data`

🔒 **All endpoints require Bearer Token**

#### Get Networks
```
GET /api/Data/networks
```
**Response:**
```json
{
  "success": true,
  "message": "Networks retrieved successfully",
  "data": [
    { "id": 1, "name": "MTN", "code": "mtn" },
    { "id": 2, "name": "Airtel", "code": "airtel" },
    { "id": 3, "name": "Glo", "code": "glo" },
    { "id": 4, "name": "9mobile", "code": "9mobile" }
  ]
}
```

---

#### Get Data Plans
```
GET /api/Data/plans/{network}
```
**Example:** `/api/Data/plans/mtn`

**Response:**
```json
{
  "success": true,
  "message": "Data plans for mtn retrieved successfully",
  "data": [
    { "code": "mtn-100mb-daily", "name": "100MB Daily", "amount": 100 },
    { "code": "mtn-1gb-daily", "name": "1GB Daily", "amount": 300 },
    { "code": "mtn-2gb-weekly", "name": "2GB Weekly", "amount": 500 },
    { "code": "mtn-5gb-monthly", "name": "5GB Monthly", "amount": 1500 },
    { "code": "mtn-10gb-monthly", "name": "10GB Monthly", "amount": 2500 }
  ]
}
```

---

#### Purchase Data
```
POST /api/Data/purchase
```
**Body:**
```json
{
  "network": "mtn",
  "phoneNumber": "08012345678",
  "planCode": "mtn-1gb-daily"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Data purchased successfully",
  "data": {
    "id": 1,
    "network": "mtn",
    "phoneNumber": "08012345678",
    "planName": "1GB Daily",
    "amount": 300,
    "status": "Success",
    "reference": "DATA-639165963260270150",
    "createdAt": "2026-06-09T10:05:26Z"
  }
}
```

---

#### Data History
```
GET /api/Data/history
```
**Response:**
```json
{
  "success": true,
  "message": "Data history retrieved successfully",
  "data": [...]
}
```

---

### 📺 Cable TV
> Base URL: `/api/CableTV`

🔒 **All endpoints require Bearer Token**

#### Get Providers
```
GET /api/CableTV/providers
```
**Response:**
```json
{
  "success": true,
  "message": "Providers retrieved successfully",
  "data": [
    { "id": 1, "name": "DStv", "code": "dstv" },
    { "id": 2, "name": "GOtv", "code": "gotv" },
    { "id": 3, "name": "StarTimes", "code": "startimes" }
  ]
}
```

---

#### Get Packages
```
GET /api/CableTV/packages/{provider}
```
**Example:** `/api/CableTV/packages/dstv`

**Response:**
```json
{
  "success": true,
  "message": "Packages for dstv retrieved successfully",
  "data": [
    { "code": "dstv-padi", "name": "DStv Padi", "amount": 2500 },
    { "code": "dstv-yanga", "name": "DStv Yanga", "amount": 3500 },
    { "code": "dstv-confam", "name": "DStv Confam", "amount": 6200 },
    { "code": "dstv-compact", "name": "DStv Compact", "amount": 10500 },
    { "code": "dstv-premium", "name": "DStv Premium", "amount": 24500 }
  ]
}
```

---

#### Subscribe
```
POST /api/CableTV/subscribe
```
**Body:**
```json
{
  "provider": "dstv",
  "packageCode": "dstv-compact",
  "smartCardNumber": "1234567890"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Cable TV subscription successful",
  "data": {
    "id": 1,
    "provider": "dstv",
    "packageName": "DStv Compact",
    "smartCardNumber": "1234567890",
    "amount": 10500,
    "status": "Success",
    "reference": "CABLE-639165975204586280",
    "createdAt": "2026-06-09T10:25:20Z"
  }
}
```

---

#### Cable TV History
```
GET /api/CableTV/history
```
**Response:**
```json
{
  "success": true,
  "message": "Cable TV history retrieved successfully",
  "data": [...]
}
```

---

### ⚡ Electricity
> Base URL: `/api/Electricity`

🔒 **All endpoints require Bearer Token**

#### Get Disco Providers
```
GET /api/Electricity/providers
```
**Response:**
```json
{
  "success": true,
  "message": "Disco providers retrieved successfully",
  "data": [
    { "id": 1, "name": "Ikeja Electric", "code": "ikeja-electric" },
    { "id": 2, "name": "Eko Electric", "code": "eko-electric" },
    { "id": 3, "name": "Abuja Electric", "code": "abuja-electric" },
    { "id": 4, "name": "Kano Electric", "code": "kano-electric" },
    { "id": 5, "name": "Port Harcourt Electric", "code": "phed" },
    { "id": 6, "name": "Enugu Electric", "code": "enugu-electric" },
    { "id": 7, "name": "Ibadan Electric", "code": "ibadan-electric" },
    { "id": 8, "name": "Kaduna Electric", "code": "kaduna-electric" },
    { "id": 9, "name": "Jos Electric", "code": "jos-electric" },
    { "id": 10, "name": "Benin Electric", "code": "benin-electric" }
  ]
}
```

---

#### Verify Meter
```
POST /api/Electricity/verify-meter
```
**Body:**
```json
{
  "discoProvider": "ikeja-electric",
  "meterNumber": "1234567890",
  "meterType": "prepaid"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Meter verified successfully",
  "data": {
    "meterNumber": "1234567890",
    "meterType": "prepaid",
    "discoProvider": "ikeja-electric",
    "customerName": "John Doe",
    "address": "123 Test Street, Lagos"
  }
}
```

---

#### Purchase Electricity
```
POST /api/Electricity/purchase
```
**Body:**
```json
{
  "discoProvider": "ikeja-electric",
  "meterNumber": "1234567890",
  "meterType": "prepaid",
  "customerName": "John Doe",
  "amount": 5000
}
```
**Response:**
```json
{
  "success": true,
  "message": "Electricity purchase successful",
  "data": {
    "id": 1,
    "discoProvider": "ikeja-electric",
    "meterNumber": "1234567890",
    "meterType": "prepaid",
    "customerName": "John Doe",
    "amount": 5000,
    "token": "2821-8682-9951-5003-5838",
    "status": "Success",
    "reference": "ELECT-639165988034782600",
    "createdAt": "2026-06-09T10:46:43Z"
  }
}
```

---

#### Electricity History
```
GET /api/Electricity/history
```
**Response:**
```json
{
  "success": true,
  "message": "Electricity history retrieved successfully",
  "data": [...]
}
```

---

## Error Responses

All endpoints return consistent error responses:

```json
{
  "success": false,
  "message": "Error message here"
}
```

| HTTP Status | Meaning |
|---|---|
| 200 | Success |
| 400 | Bad Request |
| 401 | Unauthorized |
| 404 | Not Found |
| 500 | Server Error |

---

## Project Structure

```
Project_utility_backend/
├── Core/                          # Models, DTOs, Interfaces
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── User/
│   │   ├── Airtime/
│   │   ├── Data/
│   │   ├── CableTV/
│   │   ├── Electricity/
│   │   ├── Notification/
│   │   └── TransactionPin/
│   ├── Interfaces/
│   └── Models/
├── Logic/                         # Services, DbContext, Migrations
│   ├── Data/
│   ├── Migrations/
│   └── Services/
└── PaymentUtility/                # Controllers, Program.cs
    └── Controllers/
```

---

## Notes for Frontend Team

- Always call `GET /api/Electricity/verify-meter` before `POST /api/Electricity/purchase` to get the `customerName`
- Always call `GET /api/Data/plans/{network}` before purchasing data to get valid `planCode`
- Always call `GET /api/CableTV/packages/{provider}` before subscribing to get valid `packageCode`
- Token expires in **7 days** — store it securely and refresh by logging in again
- All amounts are in **Nigerian Naira (₦)**
