
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