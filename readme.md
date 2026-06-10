## Project Name
Trainee Management API
 
## Technology Used
Asp.net core
 
## How to Run
run `dotnet run` in the root of the project directory
 
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
 