# Country Management System

## 📌 Overview
This project is a **Country Management System** built using **Clean Layered Architecture** in .NET Core (Backend) and Angular (Frontend).  
It allows users to **add, edit, delete, search, filter, and sort countries** with **soft delete** functionality and a **responsive frontend UI**.

---

## Database
### Table: `Countries`
| Column       | Type          | Constraints                     |
| ------------ | ------------- | ------------------------------- |
| Id           | int           | Primary Key, Identity           |
| Name         | nvarchar(100) | Required, Unique                |
| Code         | nvarchar(5)   | Required, Unique                |
| CreatedDate  | datetime      | Default = current date          |
| IsDeleted    | bit           | Default = 0 (for soft delete)  |

**Indexes:**
- Unique index on `Name`  
- Unique index on `Code`  

---

## 🏗️ Architecture Overview
The solution follows **Clean Architecture** principles and consists of **4 layers**:

### 1️⃣ Domain Layer
- **Purpose:** Holds the core business logic (no rules required).
- **Contents:**
  - **Entities:** Defines the main data structures (e.g., `Country` entity).
  - **DTOs:** Data Transfer Objects for communication between layers.
  - **Interfaces:** Contracts for repository patterns.
  
---

### 2️⃣ Application Layer
- **Purpose:** Contains application-specific logic and service implementations.
- **Contents:**
  - Application services to handle business processes.
  - Not yet fully implemented in this version — future work includes adding more services and CQRS pattern.

---

### 3️⃣ Infrastructure Layer
- **Purpose:** Handles data access and persistence.
- **Contents:**
  - **DbContext:** Manages database connections and entity configurations.
  - **Repositories:** Implements the repository interfaces from Domain.
  - **Soft Delete Implementation:** Adds a `IsDeleted` flag to entities and filters queries to exclude deleted items.
  - EF Core configurations for entities.

---

### 4️⃣ API Layer
- **Purpose:** Exposes the application functionality through RESTful endpoints.
- **Contents:**
  - **Controllers:** Handle HTTP requests and responses.
  - **Error Handling Middleware:** Standardizes error responses.
  - Endpoints for:
    - `GET /countries` — Retrieve list with search, filter, and sorting.
    - `POST /countries` — Add new country.
    - `PUT /countries/{id}` — Update country.
    - `DELETE /countries/{id}` — Soft delete country.

---

## 🖥️ Frontend (Angular)
- **Purpose:** Provides a responsive UI for managing countries.
- **Features Implemented:**
  - **List View:** Displays paginated list of countries.
  - **Add / Edit Forms:** Validates input and shows success/error toasts.
  - **Search & Filter:** Allows filtering by name, code, and date (without search option).
  - **Sorting:** Clickable table headers to sort by name, code, or date.
  - **Soft Delete:** Deleted countries are hidden without permanent removal.
  - **Toast Notifications:** Success, error, and confirmation messages.
  - **Delete Confirmation Toast:** Custom styled confirmation before deletion.

---

## ⚙️ Features Summary
| Feature                  | Description |
|--------------------------|-------------|
| **Soft Delete**          | Marks records as deleted without removing them from the database. |
| **Search**               | Search by country name or code. |
| **Sorting**              | Sort results by name, code, or creation date. |
| **Filtering**            | Filter results based on criteria (e.g., date range). |
| **Pagination**           | Navigate through country list. |
| **Error Handling**       | Centralized API error responses. |
| **Responsive UI**        | Works across desktop and mobile devices. |

---
- **Validation:**  
  - `Name` and `Code` are required  
  - `Code` length ≤ 5 characters  
  - Duplicate `Name` or `Code` are not allowed  
- **Soft Delete:** Deleted countries are not physically removed  
- **Pagination:** Backend supports `pageNumber` and `pageSize`  
- **Search & Filtering:** Filter by `Name` or `Code`  
- **Sorting:** Sort by `Name`, `Code`, or `CreatedDate`  

### Error Handling
- Returns proper **HTTP status codes** (`400`, `404`, `500`)  
- Returns detailed error messages for frontend consumption  
---

## 🛠️ Technologies Used
### Backend:
- **.NET Core** (Clean Architecture)
- **Entity Framework Core**
- **SQL Server**

### Frontend:
- **Angular**
- **TypeScript**
- **HTML / CSS**

---

## 🚀 How to Run the Project

### Backend:
1. Clone the repository.
2. Configure **appsettings.json** with your database connection string.
3. Run EF Core migrations:
   ```bash
   dotnet ef database update
