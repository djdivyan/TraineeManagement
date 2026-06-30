## Project Name
Trainee Management API
 
## Technology Used
 - Asp.net core
 - Mysql
 - Redis
 - RabbitMQ
 - Docker

## System Architecture
<img src="filename.png" alt="Alt text" width="500">

# Configuration
## Configure .env
  For local execution, you will need to set up `.env` files for the following projects: `TraineeManagementApi`, `SubmissionProcessingWorker`, and `TrainingDirectory.Api`.

  1. Navigate to each project directory.
  2. Copy the example environment file: `cp .env.example .env`
  3. Fill in the appropriate values (see requirements below).

### Required Environment Variables

1. Database (MySQL)
Add connection string to all three projects. 
Note: If running via Docker Compose, use the container name in place of localhost
```env
ConnectionStrings__DefaultConnection="Server=mysql;Database=trainee_management_db;User=root;Password=your_password;"

```
2. Distributed Cache (Redis)
Add in TraineeManagementApi and SubmissionProcessingWorker.
```env
ConnectionStrings__Redis="localhost/container_name:6379,password=your_password"

```
3. Message Broker (RabbitMQ)
Add in TraineeManagementApi and SubmissionProcessingWorker.
```env
RabbitMq__HostName="localhost/container_name"
RabbitMq__Port="5672"
RabbitMq__VirtualHost="/"
RabbitMq__UserName="your_username"
RabbitMq__Password="your_password"

```

4. Internal Service Communication
Add the correct Internal service URL with port number in SubmissionProcessingWorker
```env
InternalService__Url="localhost/container_name:port"

```

## MySQL setup steps
1. Get a database connection string
  [MySQLSetup](MySqlSetup.md)

2. Add Database connection string in the .env file of TraineeManagementApi, SubmissionProcessingWorker and TrainingDirectory.Api  
  ```javascript 
    ConnectionStrings__DefaultConnection=Your-Database-Connection-String
  ```

3. Run the following command in the root of the project to make sure there are no errors.  
  ```javascript 
    dotnet build 
  ```

4. Apply Entity Framework migrations to generate the required tables  
  ```javascript 
    dotnet ef database update -p TraineeManagement.Shared -s TraineeManagementApi
  ```

## Redis setup steps
1. Get a redis connection string and ensure your redis instance is up and running

2. Add Redis connection string in the .env file of TraineeManagementApi and SubmissionProcessingWorker  
  ``` javascript 
    ConnectionStrings__Redis="your_redis_connection_string"
  ```  

## RabbitMQ setup steps
1. Ensure a rabbitMQ instance is up and running

2. Add RabbitMQ username in the .env file of TraineeManagementApi and SubmissionProcessingWorker 
  ``` javascript 
    RabbitMQ__UserName="your_ysername" 
  ```

3. Add RabbitMQ password in the .env file of TraineeManagementApi and SubmissionProcessingWorker  
  ``` javascript 
    RabbitMQ__Password="your_password" 
  ```

## Setup using Docker
1. Ensure the .env files in TraineeManagement.Api, SubmissionProcessingWorker and TrainingDirectoryApi are appropriately filled and locations are correct in the docker compose file

2. Add appropriate values in the root project's .env which will be used in docker compose

3. Run the following command in the root of the project directory to start the application using docker:  
  ``` javascript
    docker compose up --build -d
  ```
4. Apply Entity Framework migrations to generate the required tables  
  ```javascript 
    dotnet ef database update -p TraineeManagement.Shared -s TraineeManagementApi
  ```

## Login Credentials for testing 
# POST /api/auth/login
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
    "token": "{DemoToken}",
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

 - POST /api/auth/login

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

 - POST   /api/Submssion 
 - GET    /api/Submssion 
 - GET    /api/Submssion/{id}
 - POST   /api/Submssion/{submissionId}/files
 - GET    /api/Submission/{submissionId}/summary

 - GET    /api/SubmissionFiles/{SubmissionFileId}/download
 - DELETE /api/SubmissionFiles/{SubmissionFileId}

 - GET /api/ProcessingJob/{id}

 - POST   /api/reviews 
 - GET    /api/reviews 
 - GET    /api/reviews/{id} 

## For Sample Request Response Json Refer 
  [Sample Requests and Responses](RequestResponse.md)

## Key Design Decisions


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
