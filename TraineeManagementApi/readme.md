## Project Name
Trainee Management API
 
## Technology Used
Asp.net core
 
## How to Run
First you need to `git clone https://github.com/djdivyan/TraineeManagement.git` and then go to directory using `cd TraineeManagement`
then run command `dotnet run` to build and run the backend api, Open swagger in browser by going to `http://localhost:5231/swagger` to test the developed API's

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
 
## Login Credentials for testing 
# POST /api/auht/login
```json
{
  "username": "admin",
  "password": "admin"
}
```
- Response of login
```json
{
  "loginResponse": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiJhZG1pbiIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTc4MTUzMjEwOCwiZXhwIjoxNzgxNTMzOTA4LCJpYXQiOjE3ODE1MzIxMDgsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIzMS8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyMzEvIn0.SQHrEzNoc5k36V85ddgLYzqbMpnEAvlNvhiutKYeHVM",
    "expiresIn": 1799,
    "responseUser": {
      "id": 1,
      "username": "admin",
      "role": "Admin"
    }
  },
  "exception": null,
  "statusCode": 0
}
```

## JWT usage instructions
Copy the token from login response and use it with every request sent by the client to protected routes
```json
Authorization: Bearer <token>
```

## API List
 - GET /api/health

 - POST /api/auht/login

 - GET    /api/trainees?pageNumber=1&pageSize=10&search=amit&status=Active 
 - GET    /api/trainees/{id} 
 - POST   /api/trainees 
 - PUT    /api/trainees/{id} 
 - DELETE /api/trainees/{id} 

 - GET    /api/mentors 
 - GET    /api/mentors/{id} 
 - POST   /api/mentors 
 - PUT    /api/mentors/{id} 
 - DELETE /api/mentors/{id} 

 - GET    /api/learning-tasks 
 - GET    /api/learning-tasks/{id} 
 - POST   /api/learning-tasks 
 - PUT    /api/learning-tasks/{id} 
 - DELETE /api/learning-tasks/{id} 

 - POST   /api/task-assignments 
 - GET    /api/task-assignments 
 - GET    /api/task-assignments/{id} 
 - PUT    /api/task-assignments/{id}/status 

 - POST   /api/submissions 
 - GET    /api/submissions 
 - GET    /api/submissions/{id} 

 - POST   /api/reviews 
 - GET    /api/reviews 
 - GET    /api/reviews/{id} 


## Sample Request JSON
```json
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
 ```
## Sample Response JSON

 ```json
Sample GET /api/health response:
{
  "status": "running",
  "application": "Trainee Management API",
  "timestamp": "2026-06-10T06:38:09.9985091+00:00"
}
 
Sample POST /api/Auth/login
{
  "loginResponse": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiJhZG1pbiIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTc4MTUyOTEzMiwiZXhwIjoxNzgxNTMwOTMyLCJpYXQiOjE3ODE1MjkxMzIsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIzMS8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyMzEvIn0.lJaxWyur2aSPLT56wLb1yIFn6y-xVd9y8oALFAOVLRY",
    "expiresIn": 1799,
    "responseUser": {
      "id": 1,
      "username": "admin",
      "role": "Admin"
    }
  },
  "exception": null,
  "statusCode": 0
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

Sample GET /api/mentors 
[
  {
    "id": 2,
    "firstName": "string",
    "lastName": "string",
    "email": "user@example.com",
    "expertise": "string",
    "mentorStatus": "Active",
    "createdDate": "2026-06-12T01:29:41.555901",
    "updatedDate": "2026-06-12T01:29:41.555907"
  },
  {
    "id": 3,
    "firstName": "string",
    "lastName": "string",
    "email": "user@example.com",
    "expertise": "string",
    "mentorStatus": "Active",
    "createdDate": "2026-06-12T01:29:42.944563",
    "updatedDate": "2026-06-12T01:29:42.944567"
  }
]

Sample GET  /api/mentors/{id} 
{
  "id": 19,
  "firstName": "divyan",
  "lastName": "jain",
  "email": "dj@example.com",
  "expertise": ".Net",
  "mentorStatus": "Active",
  "createdDate": "2026-06-15T06:45:03.005504",
  "updatedDate": "2026-06-15T06:45:03.005526"
}

Sample POST   /api/mentors 
{
  "id": 19,
  "firstName": "divyan",
  "lastName": "jain",
  "email": "dj@example.com",
  "expertise": ".Net",
  "mentorStatus": "Active",
  "createdDate": "2026-06-15T06:45:03.0055042-07:00",
  "updatedDate": "2026-06-15T06:45:03.0055263-07:00"
}
Sample PUT    /api/mentors/{id} 
{
  "id": 19,
  "firstName": "divyan",
  "lastName": "jain",
  "email": "Update@email.com",
  "expertise": ".Net",
  "mentorStatus": "Active",
  "createdDate": "2026-06-15T06:45:03.005504",
  "updatedDate": "2026-06-15T06:51:11.5846077-07:00"
}

Sample DELETE /api/mentors/{id}
  Status code 204

Sample GET    /api/learning-tasks 
[
  {
    "id": 1,
    "title": "string",
    "description": "string",
    "expectedTechStack": "string",
    "dueDate": "2026-06-12T10:59:32.206",
    "learningTaskStatus": "Draft",
    "createdDate": "2026-06-12T11:01:16.497661",
    "updatedDate": "2026-06-12T11:01:16.497676"
  },
  {
    "id": 2,
    "title": "string",
    "description": "string",
    "expectedTechStack": "string",
    "dueDate": "2026-06-12T10:59:32.206",
    "learningTaskStatus": "Draft",
    "createdDate": "2026-06-12T11:01:19.275252",
    "updatedDate": "2026-06-12T11:01:19.275252"
  }
]
Sample GET    /api/learning-tasks/{id} 
{
  "id": 2,
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-12T10:59:32.206",
  "learningTaskStatus": "Draft",
  "createdDate": "2026-06-12T11:01:19.275252",
  "updatedDate": "2026-06-12T11:01:19.275252"
}
Sample POST   /api/learning-tasks 
{
  "id": 4,
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-15T14:03:48.653Z",
  "learningTaskStatus": "Draft",
  "createdDate": "2026-06-15T14:03:53.4237126Z",
  "updatedDate": "2026-06-15T14:03:53.4237258Z"
}
Sample PUT    /api/learning-tasks/{id} 
{
  "id": 4,
  "title": "updated",
  "description": "task",
  "expectedTechStack": "stack",
  "dueDate": "2026-06-15T14:04:09.391Z",
  "learningTaskStatus": "Closed",
  "createdDate": "2026-06-15T14:03:53.423712",
  "updatedDate": "2026-06-15T07:04:48.8237663-07:00"
}
Sample DELETE /api/learning-tasks/{id} 
  Status code 204

Sample POST   /api/task-assignments 
{
  "id": 5,
  "traineeId": 2,
  "mentorId": 19,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T13:53:32.907Z",
  "dueDate": "2026-06-15T13:53:32.907Z",
  "taskAssignmentStatus": "Assigned",
  "remarks": "string"
}
Sample GET    /api/task-assignments 
[{
  "id": 5,
  "traineeId": 2,
  "mentorId": 19,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T13:53:32.907Z",
  "dueDate": "2026-06-15T13:53:32.907Z",
  "taskAssignmentStatus": "Assigned",
  "remarks": "string"
}]
Sample GET    /api/task-assignments/{id} 
{
  "id": 5,
  "traineeId": 2,
  "mentorId": 19,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T13:53:32.907Z",
  "dueDate": "2026-06-15T13:53:32.907Z",
  "taskAssignmentStatus": "Assigned",
  "remarks": "string"
}
Sample PUT    /api/task-assignments/{id}/status 
{
  "id": 5,
  "traineeId": 2,
  "trainee": null,
  "mentorId": 19,
  "mentor": null,
  "learningTaskId": 1,
  "learningTask": null,
  "assignedDate": "2026-06-15T13:53:32.907",
  "dueDate": "2026-06-15T13:53:32.907",
  "taskAssignmentStatus": "Completed",
  "remarks": "string"
}

Sample POST   /api/submissions 
{
  "id": 4,
  "taskAssignmentId": 3,
  "submissionUrl": "string",
  "notes": "string",
  "submittedDate": "2026-06-15T06:54:41.8728241-07:00",
  "submissionStatus": "Submitted"
}
Sample GET    /api/submissions 
[
  {
    "id": 1,
    "taskAssignmentId": 1,
    "submissionUrl": "string",
    "notes": "string",
    "submittedDate": "2026-06-15T04:34:39.283216",
    "submissionStatus": "Submitted"
  },
  {
    "id": 2,
    "taskAssignmentId": 3,
    "submissionUrl": "string",
    "notes": "string",
    "submittedDate": "2026-06-15T04:36:39.703145",
    "submissionStatus": "Submitted"
  }
]
Sample GET    /api/submissions/{id} 
{
  "id": 4,
  "taskAssignmentId": 3,
  "submissionUrl": "string",
  "notes": "string",
  "submittedDate": "2026-06-15T06:54:41.872824",
  "submissionStatus": "Submitted"
}

Sample POST   /api/reviews 
{
  "id": 3,
  "submissionId": 2,
  "mentorId": 2,
  "feedback": "good",
  "score": 1000,
  "reviewStatus": "Accepted",
  "reviewedDate": "2026-06-15T11:25:51.386Z"
}
Sample GET    /api/reviews 
[
  {
    "id": 1,
    "submissionId": 2,
    "mentorId": 2,
    "feedback": "good",
    "score": 1000,
    "reviewStatus": "Accepted",
    "reviewedDate": "2026-06-15T11:25:51.386"
  },
  {
    "id": 2,
    "submissionId": 2,
    "mentorId": 2,
    "feedback": "good",
    "score": 1000,
    "reviewStatus": "Accepted",
    "reviewedDate": "2026-06-15T11:25:51.386"
  }
]
Sample GET    /api/reviews/{id} 
{
  "id": 2,
  "submissionId": 2,
  "mentorId": 2,
  "feedback": "good",
  "score": 1000,
  "reviewStatus": "Accepted",
  "reviewedDate": "2026-06-15T11:25:51.386"
}

```
## Known Limitations
- Token refresh
- Role based Authentication

## Security Checklist
- [x] Authentication - JWT validation enabled 
- [x] Authorization - Protected APIs require token 
- [x] Password storage - Passwords stored as hash only 
- [x] Excessive data exposure - DTOs used, Password hash not returned 
- [x] Injection - EF Core used; no unsafe raw SQL 
- [x] Security misconfiguration - CORS restricted to expected origin 
- [x] Sensitive data exposure - Secrets not hardcoded in controllers 
- [x] Error handling - Stack traces not returned 
- [x] Logging - Passwords and tokens not logged

## Next Improvement areas
