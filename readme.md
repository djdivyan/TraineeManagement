## Project Name
Trainee Management API
 
## Technology Used
Asp.net core
 
## How to Run
run `dotnet run` in the root of the project directory

# MySQL Setup Commands (WSL/Ubuntu)
 
## 1. Update Ubuntu Packages
 
```bash
sudo apt update
````
 
***
 
## 2. Install MySQL Server
 
```bash
sudo apt install mysql-server -y
```
 
***
 
## 3. Start MySQL Service
 
```bash
sudo service mysql start
```
 
***
 
## 4. Check MySQL Status
 
```bash
sudo service mysql status
```
 
Expected:
 
```text
active (running)
```
 
***
 
## 5. Open MySQL as sudo User
 
```bash
sudo mysql
```
 
***
 
## 6. Change Root Authentication to Password-Based Login
 
```sql
ALTER USER 'root'@'localhost'
IDENTIFIED WITH mysql_native_password
BY 'Root@123';
```
 
***
 
## 7. Apply Changes
 
```sql
FLUSH PRIVILEGES;
```
 
***
 
## 8. Verify Authentication Plugin
 
```sql
SELECT user, host, plugin FROM mysql.user;
```
 
Expected:
 
```text
root | localhost | mysql_native_password
```
 
***
 
## 9. Exit MySQL
 
```sql
exit;
```
 
***
 
## 10. Restart MySQL
 
```bash
sudo service mysql restart
```
 
***
 
## 11. Login Using Root Password
 
```bash
mysql -u root -p
```
 
Password:
 
```text
Root@123
```
 
***
 
## 12. Create Database
 
```sql
CREATE DATABASE trainee_management_db;
```
 
***
 
## 13. Verify Database
 
```sql
SHOW DATABASES;
```
 
Expected:
 
```text
trainee_management_db
```
 
***
 
## 14. Exit MySQL
 
```sql
exit;
```
 
***
 
# EF Core + MySQL Setup Commands
 
## 1. Remove Old InMemory Package
 
```bash
dotnet remove package Microsoft.EntityFrameworkCore.InMemory
```
 
***
 
## 2. Install EF Core MySQL Packages
 
```bash
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Relational --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
```
 
***
 
## 3. Restore Packages
 
```bash
dotnet restore
```
 
***
 
## 4. Install dotnet ef Tool
 
```bash
dotnet tool install --global dotnet-ef
```
 
***
 
## 5. Add dotnet Tools Path
 
```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```
 
***
 
## 6. Create Migration
 
```bash
dotnet ef migrations add InitialCreate
```
 
***
 
## 7. Apply Migration
 
```bash
dotnet ef database update
```
 
***
 
## 8. Run Application
 
```bash
dotnet run
```
 
***
 
# Verify Tables in MySQL
 
Login:
 
```bash
mysql -u root -p
```
 
Select database:
 
```sql
USE trainee_management_db;
```
 
Show tables:
 
```sql
SHOW TABLES;
```
 
Expected:
 
```text
Trainees
__EFMigrationsHistory
```
 
## API List
- GET /api/health
- GET /api/trainees 
- GET /api/trainees?search
- GET /api/trainees/{id}
- POST /api/trainees
- PUT /api/trainees/{id}
- DELETE /api/trainees/{id}
 
## Sample Request JSON
Sample POST and PUT /api/trainees request:
{
  "firstName": "john",
  "lastName": "joe",
  "email": "john.doe@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Active"
}
{
  "firstName": "Divyan",
  "lastName": "Jain",
  "email": "dj@gmail.com",
  "techStack": "React, .Net",
  "status": "Active"
}
 
 
## Sample Response JSON
Sample GET /api/health response:
{
  "status": "running",
  "application": "Trainee Management API",
  "timestamp": "2026-06-10T06:38:09.9985091+00:00"
}
 
Sample GET /api/trainees response:
[
  {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active",
    "createdDate": "2026-06-10T06:38:58.0911902+00:00",
    "updatedDate": "2026-06-10T06:38:58.0912088+00:00"
  },
  {
    "id": 2,
    "firstName": "Divyan",
    "lastName": "Jain",
    "email": "dj@gmail.com",
    "techStack": "React, .Net",
    "status": "Active",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
  }
]
 
Sample GET /api/trainees?search={search} response :
Search term : Divyan
[
  {
    "id": 2,
    "firstName": "Divyan",
    "lastName": "Jain",
    "email": "dj@gmail.com",
    "techStack": "React, .Net",
    "status": "Active",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
  }
]
 
Sample POST /api/trainees response:
 
{
    "id": 2,
    "firstName": "Divyan",
    "lastName": "Jain",
    "email": "dj@gmail.com",
    "techStack": "React, .Net",
    "status": "Active",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
}
 
 
Sample GET /api/trainees/{id} response:
{
    "id": 2,
    "firstName": "Divyan",
    "lastName": "Jain",
    "email": "dj@gmail.com",
    "techStack": "React, .Net",
    "status": "Active",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
}
 
 
Sample PUT /api/trainees/{id} response:
{
    "id": 2,
    "firstName": "Divyan",
    "lastName": "Jain",
    "email": "dj@gmail.com",
    "techStack": "React, .Net",
    "status": "Active",
    "createdDate": "2026-06-10T06:40:04.676972+00:00",
    "updatedDate": "2026-06-10T06:40:04.6769743+00:00"
}
 
## Known Limitations
- Using In-memory database instead of Sql or NoSql database
 