CREATE TABLE `battleinformation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `playerCharacterId` int NOT NULL,
  `enemyCharacterId` int NOT NULL,
  `playerCharacterHP` int DEFAULT NULL,
  `enemyCharacterHP` int DEFAULT NULL,
  `battleStartedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `playerCharacterId` (`playerCharacterId`),
  KEY `enemyCharacterId` (`enemyCharacterId`),
  CONSTRAINT `battleinformation_ibfk_1` FOREIGN KEY (`playerCharacterId`) REFERENCES `characters` (`id`),
  CONSTRAINT `battleinformation_ibfk_2` FOREIGN KEY (`enemyCharacterId`) REFERENCES `characters` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
