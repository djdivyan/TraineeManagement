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