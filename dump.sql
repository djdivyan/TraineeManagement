-- MySQL dump 10.13  Distrib 8.0.46, for Linux (x86_64)
--
-- Host: localhost    Database: trainee_management_db
-- ------------------------------------------------------
-- Server version	8.0.46-0ubuntu0.24.04.2

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `LearningTasks`
--

DROP TABLE IF EXISTS `LearningTasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LearningTasks` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ExpectedTechStack` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DueDate` datetime(6) NOT NULL,
  `LearningTaskStatus` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LearningTasks`
--

LOCK TABLES `LearningTasks` WRITE;
/*!40000 ALTER TABLE `LearningTasks` DISABLE KEYS */;
INSERT INTO `LearningTasks` VALUES (1,'string','string','string','2026-06-12 10:59:32.206000',0,'2026-06-12 11:01:16.497661','2026-06-12 11:01:16.497676'),(2,'string','string','string','2026-06-12 10:59:32.206000',0,'2026-06-12 11:01:19.275252','2026-06-12 11:01:19.275252'),(3,'string','string','string','2026-06-12 10:59:32.206000',0,'2026-06-12 11:01:20.278977','2026-06-12 11:01:20.278977'),(4,'updated','task','stack','2026-06-15 14:04:09.391000',2,'2026-06-15 14:03:53.423712','2026-06-15 07:04:48.823766');
/*!40000 ALTER TABLE `LearningTasks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Mentors`
--

DROP TABLE IF EXISTS `Mentors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Mentors` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Expertise` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MentorStatus` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Mentors`
--

LOCK TABLES `Mentors` WRITE;
/*!40000 ALTER TABLE `Mentors` DISABLE KEYS */;
INSERT INTO `Mentors` VALUES (2,'string','string','user@example.com','string',0,'2026-06-12 01:29:41.555901','2026-06-12 01:29:41.555907'),(3,'string','string','user@example.com','string',0,'2026-06-12 01:29:42.944563','2026-06-12 01:29:42.944567'),(4,'string','string','user@example.com','string',0,'2026-06-12 03:59:05.494834','2026-06-12 03:59:05.494878'),(5,'string','string','user@example.com','string',0,'2026-06-12 03:59:07.333986','2026-06-12 03:59:07.333996'),(7,'string','string','user@example.com','string',0,'2026-06-12 03:59:07.723168','2026-06-12 03:59:07.723175'),(8,'string','string','user@example.com','string',0,'2026-06-12 03:59:07.933748','2026-06-12 03:59:07.933751'),(9,'string','string','user@example.com','string',0,'2026-06-12 03:59:08.123289','2026-06-12 03:59:08.123293'),(10,'string','string','user@example.com','string',0,'2026-06-12 03:59:08.315121','2026-06-12 03:59:08.315127'),(11,'string','string','user@example.com','string',0,'2026-06-12 03:59:08.490444','2026-06-12 03:59:08.490446'),(12,'string','string','user@example.com','string',0,'2026-06-12 03:59:08.646805','2026-06-12 03:59:08.646808'),(13,'string','string','user@example.com','string',0,'2026-06-12 03:59:08.867634','2026-06-12 03:59:08.867636'),(14,'string','string','user@example.com','string',0,'2026-06-12 03:59:09.322484','2026-06-12 03:59:09.322485'),(15,'string','string','user@example.com','string',0,'2026-06-12 03:59:09.508834','2026-06-12 03:59:09.508836'),(16,'string','string','user@example.com','string',0,'2026-06-12 03:59:09.698232','2026-06-12 03:59:09.698234'),(17,'string','string','user@example.com','string',0,'2026-06-12 03:59:09.887115','2026-06-12 03:59:09.887117'),(18,'string','string','user@example.com','string',0,'2026-06-12 03:59:10.104998','2026-06-12 03:59:10.104999'),(19,'divyan','jain','Update@email.com','.Net',0,'2026-06-15 06:45:03.005504','2026-06-15 06:51:11.584607');
/*!40000 ALTER TABLE `Mentors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Reviews`
--

DROP TABLE IF EXISTS `Reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Reviews` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SubmissionId` int NOT NULL,
  `MentorId` int NOT NULL,
  `Feedback` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Score` int DEFAULT NULL,
  `ReviewStatus` int NOT NULL,
  `ReviewedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Reviews_MentorId` (`MentorId`),
  KEY `IX_Reviews_SubmissionId` (`SubmissionId`),
  CONSTRAINT `FK_Reviews_Mentors_MentorId` FOREIGN KEY (`MentorId`) REFERENCES `Mentors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Reviews_Submissions_SubmissionId` FOREIGN KEY (`SubmissionId`) REFERENCES `Submissions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Reviews`
--

LOCK TABLES `Reviews` WRITE;
/*!40000 ALTER TABLE `Reviews` DISABLE KEYS */;
INSERT INTO `Reviews` VALUES (1,2,2,'good',1000,0,'2026-06-15 11:25:51.386000'),(2,2,2,'good',1000,0,'2026-06-15 11:25:51.386000'),(3,2,2,'good',1000,0,'2026-06-15 11:25:51.386000');
/*!40000 ALTER TABLE `Reviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SubmissionFiles`
--

DROP TABLE IF EXISTS `SubmissionFiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SubmissionFiles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SubmissionId` int NOT NULL,
  `OriginalFileName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GeneratedStorageName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ContentType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Size` bigint NOT NULL,
  `Checksum` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UploadedByUser` int NOT NULL,
  `Timestamp` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SubmissionFiles_SubmissionId` (`SubmissionId`),
  CONSTRAINT `FK_SubmissionFiles_Submissions_SubmissionId` FOREIGN KEY (`SubmissionId`) REFERENCES `Submissions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SubmissionFiles`
--

LOCK TABLES `SubmissionFiles` WRITE;
/*!40000 ALTER TABLE `SubmissionFiles` DISABLE KEYS */;
INSERT INTO `SubmissionFiles` VALUES (4,1,'Fresher Training Task – Chrome DevTools & Performance Analys.pdf','1eb60567-7e55-4711-b1f2-e0b09f931e62.pdf','application/pdf',2077697,'95980A570A38E42EE37AD1C62DD2473E',1,'2026-06-19 04:23:21.264102'),(5,1,'Fresher Training Task – Chrome DevTools & Performance Analys.pdf','d6f1be96-ecba-4290-8173-425887a905d4.pdf','application/pdf',2077697,'95980A570A38E42EE37AD1C62DD2473E',1,'2026-06-19 04:49:05.313428');
/*!40000 ALTER TABLE `SubmissionFiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Submissions`
--

DROP TABLE IF EXISTS `Submissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Submissions` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TaskAssignmentId` int NOT NULL,
  `SubmissionUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Notes` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SubmittedDate` datetime(6) NOT NULL,
  `SubmissionStatus` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Submissions_TaskAssignmentId` (`TaskAssignmentId`),
  CONSTRAINT `FK_Submissions_TaskAssignments_TaskAssignmentId` FOREIGN KEY (`TaskAssignmentId`) REFERENCES `TaskAssignments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Submissions`
--

LOCK TABLES `Submissions` WRITE;
/*!40000 ALTER TABLE `Submissions` DISABLE KEYS */;
INSERT INTO `Submissions` VALUES (1,1,'string','string','2026-06-15 04:34:39.283216',0),(2,3,'string','string','2026-06-15 04:36:39.703145',0),(3,3,'string','string','2026-06-15 04:38:27.930529',0),(4,3,'string','string','2026-06-15 06:54:41.872824',0);
/*!40000 ALTER TABLE `Submissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TaskAssignments`
--

DROP TABLE IF EXISTS `TaskAssignments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TaskAssignments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TraineeId` int NOT NULL,
  `MentorId` int NOT NULL,
  `LearningTaskId` int NOT NULL,
  `AssignedDate` datetime(6) NOT NULL,
  `DueDate` datetime(6) NOT NULL,
  `TaskAssignmentStatus` int NOT NULL,
  `Remarks` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_TaskAssignments_LearningTaskId` (`LearningTaskId`),
  KEY `IX_TaskAssignments_MentorId` (`MentorId`),
  KEY `IX_TaskAssignments_TraineeId` (`TraineeId`),
  CONSTRAINT `FK_TaskAssignments_LearningTasks_LearningTaskId` FOREIGN KEY (`LearningTaskId`) REFERENCES `LearningTasks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_TaskAssignments_Mentors_MentorId` FOREIGN KEY (`MentorId`) REFERENCES `Mentors` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_TaskAssignments_Trainees_TraineeId` FOREIGN KEY (`TraineeId`) REFERENCES `Trainees` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TaskAssignments`
--

LOCK TABLES `TaskAssignments` WRITE;
/*!40000 ALTER TABLE `TaskAssignments` DISABLE KEYS */;
INSERT INTO `TaskAssignments` VALUES (1,2,2,2,'2026-06-15 07:44:24.920000','2026-06-15 07:44:24.920000',0,'string'),(3,1,5,1,'2026-06-15 11:28:29.528000','2026-06-15 11:28:29.528000',0,'string'),(4,1,5,1,'2026-06-15 11:28:29.528000','2026-06-15 11:28:29.528000',0,'string'),(5,2,19,1,'2026-06-15 13:53:32.907000','2026-06-15 13:53:32.907000',4,'string'),(6,3,3,2,'2026-06-17 07:30:12.320000','2026-06-17 07:30:12.320000',0,'string'),(7,3,3,2,'2026-06-17 07:30:12.320000','2026-06-17 07:30:12.320000',0,'string');
/*!40000 ALTER TABLE `TaskAssignments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Trainees`
--

DROP TABLE IF EXISTS `Trainees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Trainees` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TechStack` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Trainees`
--

LOCK TABLES `Trainees` WRITE;
/*!40000 ALTER TABLE `Trainees` DISABLE KEYS */;
INSERT INTO `Trainees` VALUES (1,'Divyan','Jain','dj@example.com','JAVA',1,'2026-06-10 02:42:30.022682','2026-06-10 02:42:30.022703'),(2,'Michael','sometg','uuuu@example.com','yyyy',2,'2026-06-10 02:43:39.541815','2026-06-10 02:43:39.541826'),(3,'Adit','sometg','uuuu@example.com','yyyy',2,'2026-06-10 03:49:23.479082','2026-06-10 03:49:23.479107'),(4,'Adit','sometg','uuuu@example.com','yyyy',2,'2026-06-10 04:01:40.123927','2026-06-10 04:01:40.123964'),(5,'sam','sambrosurname','sam@sir.com','java',1,'2026-06-11 06:17:22.958356','2026-06-11 06:17:22.958377'),(6,'new','string','user@example.com','string',0,'2026-06-22 03:04:26.521154','2026-06-22 03:05:16.394537');
/*!40000 ALTER TABLE `Trainees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` int NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `UpdatedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Users`
--

LOCK TABLES `Users` WRITE;
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` VALUES (1,'admin','admin@gmail.com','AQAAAAIAAYagAAAAEKUzHYCmUbGQ4Gb3sJB/993gVZBn5tVtyWo2gX/A847VeY5OcS4lFBAfVkMzKo2MqQ==',0,'2026-06-10 04:49:46.461115','0001-01-01 00:00:00.000000');
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20260610093558_InitialCreate','9.0.0'),('20260610111015_UserMigration','9.0.0'),('20260610111429_NewMigration','9.0.0'),('20260611051619_NewMigration-1','9.0.0'),('20260612081209_MentorMigration-1','9.0.0'),('20260612093446_LearningTask','9.0.0'),('20260612110033_LearningTask-1','9.0.0'),('20260615064259_TaskAssignment','9.0.0'),('20260615080640_TaskAssignment1','9.0.0'),('20260615090739_SubmissionNReview','9.0.0'),('20260615091518_SubmissionNReview1','9.0.0'),('20260619092447_SubmissionFile','9.0.0'),('20260619092849_SubmissionFile-1','9.0.0'),('20260619103928_SubmissionFile-2','9.0.0'),('20260619110425_SubmissionFile-3','9.0.0');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-22  3:45:47
