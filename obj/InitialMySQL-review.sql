CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `Product` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext NOT NULL,
    `Price` decimal(18,2) NOT NULL,
    `Stock` int NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `User` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Email` longtext NOT NULL,
    `Password` longtext NOT NULL,
    `Fullname` longtext NOT NULL,
    `Role` longtext NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Cart` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    `UserId` int NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Cart_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Order` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `TotalAmount` decimal(18,2) NOT NULL,
    `Status` longtext NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Order_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `RefreshToken` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Token` longtext NOT NULL,
    `UserId` int NOT NULL,
    `ExpiredDate` datetime(6) NOT NULL,
    `IsRevoked` tinyint(1) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RefreshToken_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CartItem` (
    `CartId` int NOT NULL,
    `ProductId` int NOT NULL,
    `Quantity` int NOT NULL,
    PRIMARY KEY (`CartId`, `ProductId`),
    CONSTRAINT `FK_CartItem_Cart_CartId` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CartItem_Product_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Product` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `OrderItem` (
    `OrderId` int NOT NULL,
    `ProductId` int NOT NULL,
    `Quantity` int NOT NULL,
    `Price` decimal(18,2) NOT NULL,
    PRIMARY KEY (`OrderId`, `ProductId`),
    CONSTRAINT `FK_OrderItem_Order_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Order` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_OrderItem_Product_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Product` (`Id`) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX `IX_Cart_UserId` ON `Cart` (`UserId`);

CREATE INDEX `IX_CartItem_ProductId` ON `CartItem` (`ProductId`);

CREATE INDEX `IX_Order_UserId` ON `Order` (`UserId`);

CREATE INDEX `IX_OrderItem_ProductId` ON `OrderItem` (`ProductId`);

CREATE INDEX `IX_RefreshToken_UserId` ON `RefreshToken` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261005170048_InitialMySQL', '10.0.11');

COMMIT;

