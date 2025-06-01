# Officina Meccanica Milani

A Blazor Server web app for managing an auto repair workshop. Customers, mechanics, and administrators each have dedicated dashboards. Core functions include user authentication, vehicle management, appointment scheduling, quote generation, repair tracking, and email notifications.

---

## Key Features

- **Role-Based Access**:  
  - **Admin**: Manage users and roles  
  - **Customer**: Register/login, add vehicles, schedule appointments, view/approve quotes, track repairs  
  - **Mechanic**: View assigned jobs, create quotes, update repair status

- **Authentication & Security**:  
  - Cookie-based login/registration  
  - “Forgot password” via email token

- **Data Persistence**:  
  - Entity Framework Core with automatic migrations on startup  
  - SQL Server (LocalDB or full instance)

- **Email Notifications**:  
  - SMTP via MailKit for password resets and quote notifications

---

## Technologies

- **.NET 9.0** & **Blazor Server**  
- **Entity Framework Core 8.0** (SQL Server)  
- **AutoMapper** for DTO ↔ entity mapping  
- **MailKit** for SMTP  
- **Cookie Authentication** (ASP.NET Core)

---

## Prerequisites

1. **.NET 9.0 SDK**  
2. **SQL Server** (e.g., LocalDB or Express)  
3. **SMTP Account** (Gmail, company server, etc.)  
4. **Visual Studio 2022** (recommended) or VS Code

---

## Setup & Run

1. **Clone the repo**  
   git clone https://github.com/<your-username>/OfficinaMeccanicaMilani.git
   cd OfficinaMeccanicaMilani
2. **Configure Database**

In appsettings.json, setn "DefaultConnection" to your SQL Server instance.

Example (LocalDB):

json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OfficinaDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}

3. **Configure SMTP**
   In appsettings.json, update "Smtp" with valid host, port, credentials, and sender info.
   Example (Gmail):

json
Copia
Modifica
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSsl": true,
  "UserName": "you@gmail.com",
  "Password": "yourAppPassword",
  "FromName": "Officina Milani",
  "FromEmail": "noreply@officinamilani.it"
}

4. **Run the App**
From CLI:
dotnet restore
dotnet build
dotnet run --project BlazorOfficina.csproj
Or open BlazorOfficina.sln in Visual Studio and press F5.

5. **Access in Browser**
Open https://localhost:5001 (or http://localhost:5000)

Register a new account (Customer, Mechanic, or Admin) and start using the dashboard.

**License**
Distributed under the MIT License
