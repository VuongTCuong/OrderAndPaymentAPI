START TRANSACTION;
ALTER TABLE `Cart` DROP CONSTRAINT `FK_Cart_User_UserId`;

ALTER TABLE `RefreshToken` DROP CONSTRAINT `FK_RefreshToken_User_UserId`;

ALTER TABLE `Order` DROP CONSTRAINT `FK_Order_User_UserId`;

ALTER TABLE `CartItem` DROP CONSTRAINT `FK_CartItem_Cart_CartId`;

ALTER TABLE `CartItem` DROP CONSTRAINT `FK_CartItem_Product_ProductId`;

ALTER TABLE `OrderItem` DROP CONSTRAINT `FK_OrderItem_Order_OrderId`;

ALTER TABLE `OrderItem` DROP CONSTRAINT `FK_OrderItem_Product_ProductId`;

ALTER TABLE `User` MODIFY `Role` longtext NOT NULL;

ALTER TABLE `User` MODIFY `Password` longtext NOT NULL;

ALTER TABLE `User` MODIFY `Fullname` longtext NOT NULL;

ALTER TABLE `User` MODIFY `Email` longtext NOT NULL;

ALTER TABLE `User` MODIFY `CreatedDate` datetime(6) NOT NULL;

ALTER TABLE `User` MODIFY `Id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `RefreshToken` MODIFY `UserId` int NOT NULL;

ALTER TABLE `RefreshToken` MODIFY `Token` longtext NOT NULL;

ALTER TABLE `RefreshToken` MODIFY `IsRevoked` tinyint(1) NOT NULL;

ALTER TABLE `RefreshToken` MODIFY `ExpiredDate` datetime(6) NOT NULL;

ALTER TABLE `RefreshToken` MODIFY `Id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `Product` MODIFY `Stock` int NOT NULL;

ALTER TABLE `Product` MODIFY `Price` decimal(18,2) NOT NULL;

ALTER TABLE `Product` MODIFY `Name` longtext NOT NULL;

ALTER TABLE `Product` MODIFY `Id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `OrderItem` MODIFY `Quantity` int NOT NULL;

ALTER TABLE `OrderItem` MODIFY `Price` decimal(18,2) NOT NULL;

ALTER TABLE `OrderItem` MODIFY `ProductId` int NOT NULL;

ALTER TABLE `OrderItem` MODIFY `OrderId` int NOT NULL;

ALTER TABLE `Order` MODIFY `UserId` int NOT NULL;

ALTER TABLE `Order` MODIFY `UpdatedAt` datetime(6) NOT NULL;

ALTER TABLE `Order` MODIFY `TotalAmount` decimal(18,2) NOT NULL;

ALTER TABLE `Order` MODIFY `Status` longtext NOT NULL;

ALTER TABLE `Order` MODIFY `CreatedAt` datetime(6) NOT NULL;

ALTER TABLE `Order` MODIFY `Id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `CartItem` MODIFY `Quantity` int NOT NULL;

ALTER TABLE `CartItem` MODIFY `ProductId` int NOT NULL;

ALTER TABLE `CartItem` MODIFY `CartId` int NOT NULL;

ALTER TABLE `Cart` MODIFY `UserId` int NOT NULL;

ALTER TABLE `Cart` MODIFY `UpdatedAt` datetime(6) NOT NULL;

ALTER TABLE `Cart` MODIFY `CreatedAt` datetime(6) NOT NULL;

ALTER TABLE `Cart` MODIFY `Id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `Cart` ADD CONSTRAINT `FK_Cart_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE;

ALTER TABLE `RefreshToken` ADD CONSTRAINT `FK_RefreshToken_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Order` ADD CONSTRAINT `FK_Order_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `CartItem` ADD CONSTRAINT `FK_CartItem_Cart_CartId` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE CASCADE;

ALTER TABLE `CartItem` ADD CONSTRAINT `FK_CartItem_Product_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Product` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `OrderItem` ADD CONSTRAINT `FK_OrderItem_Order_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Order` (`Id`) ON DELETE CASCADE;

ALTER TABLE `OrderItem` ADD CONSTRAINT `FK_OrderItem_Product_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Product` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261005160955_MySQLMigration', '10.0.11');

COMMIT;

