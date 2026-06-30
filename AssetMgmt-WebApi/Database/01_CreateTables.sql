/*create database*/
CREATE DATABASE AssetMgmtDB;

/*use database*/
USE AssetMgmtDB;
GO
/*login table*/

CREATE TABLE dbo.Login
(
    LId         INT IDENTITY(1,1)   NOT NULL,
    Username    VARCHAR(50)         NOT NULL,
    Password    VARCHAR(200)        NOT NULL,
    UserType    VARCHAR(30)         NOT NULL,  
    IsActive    BIT                 NOT NULL    DEFAULT (1),
    CreatedDate DATETIME            NOT NULL    DEFAULT (GETDATE()),
    CONSTRAINT PK_Login PRIMARY KEY (LId),
    CONSTRAINT UQ_Login_Username UNIQUE (Username),
    CONSTRAINT CK_Login_UserType CHECK (UserType IN ('Admin', 'Purchase Manager'))
);
GO

/*registration table*/
CREATE TABLE dbo.UserRegistration
(
    UId         INT IDENTITY(1,1)   NOT NULL,
    FirstName   VARCHAR(50)         NOT NULL,
    LastName    VARCHAR(50)         NOT NULL,
    Age         INT                 NOT NULL,
    Gender      VARCHAR(10)         NOT NULL,
    Address     VARCHAR(200)        NOT NULL,
    PhoneNumber VARCHAR(15)         NOT NULL,
    LId         INT                 NOT NULL,
    CONSTRAINT PK_UserRegistration PRIMARY KEY (UId),
    CONSTRAINT FK_UserRegistration_Login FOREIGN KEY (LId) REFERENCES dbo.Login (LId) ON DELETE CASCADE
);
GO

/*assest type*/
CREATE TABLE dbo.AssetType
(
    AtId    INT IDENTITY(1,1)  NOT NULL,
    AtName  VARCHAR(50)        NOT NULL,
    CONSTRAINT PK_AssetType PRIMARY KEY (AtId)
);
GO

/*assest definition*/
CREATE TABLE dbo.AssetDefinition
(
    AdId        INT IDENTITY(1,1)  NOT NULL,
    AdName      VARCHAR(100)       NOT NULL,
    AdTypeId    INT                NOT NULL,
    AdClass     VARCHAR(5)         NOT NULL,    
    CONSTRAINT PK_AssetDefinition PRIMARY KEY (AdId),
    CONSTRAINT FK_AssetDefinition_AssetType FOREIGN KEY (AdTypeId) REFERENCES dbo.AssetType (AtId),
    CONSTRAINT CK_AssetDefinition_Class CHECK (AdClass IN ('HW', 'SW'))
);
GO

/*vendor*/
CREATE TABLE dbo.Vendor
(
    VdId        INT IDENTITY(1,1)  NOT NULL,
    VdName      VARCHAR(100)       NOT NULL,
    VdType      VARCHAR(40)        NOT NULL,    
    VdAtypeId   INT                NOT NULL,
    VdFrom      DATE               NOT NULL,
    VdTo        DATE               NOT NULL,
    VdAddr      VARCHAR(200)       NOT NULL,
    CONSTRAINT PK_Vendor PRIMARY KEY (VdId),
    CONSTRAINT FK_Vendor_AssetType FOREIGN KEY (VdAtypeId) REFERENCES dbo.AssetType (AtId)
);
GO
/*purchase order*/

CREATE TABLE dbo.PurchaseOrder
(
    PdId        INT IDENTITY(1,1)  NOT NULL,
    PdOrderNo   VARCHAR(10)        NOT NULL,
    PdAdId      INT                NOT NULL,
    PdTypeId    INT                NOT NULL,
    PdQty       INT                NOT NULL,
    PdVendorId  INT                NOT NULL,
    PdDate      DATE               NOT NULL,
    PdDdate     DATE               NOT NULL,
    PdStatus    VARCHAR(60)        NOT NULL,
    CONSTRAINT PK_PurchaseOrder PRIMARY KEY (PdId),
    CONSTRAINT FK_PurchaseOrder_AssetDefinition FOREIGN KEY (PdAdId) REFERENCES dbo.AssetDefinition (AdId),
    CONSTRAINT FK_PurchaseOrder_AssetType FOREIGN KEY (PdTypeId) REFERENCES dbo.AssetType (AtId),
    CONSTRAINT FK_PurchaseOrder_Vendor FOREIGN KEY (PdVendorId) REFERENCES dbo.Vendor (VdId),
    CONSTRAINT CK_PurchaseOrder_Status CHECK (PdStatus IN (
        'PO - Raised with Supplier',
        'Awaiting Delivery by Supplier',
        'Consignment Received',
        'Asset Details registered internally',
        'Asset Allocated to Resources',
        'Identified Faulty',
        'Replaced - Repaired'))
);
GO

/*assestmaster*/
CREATE TABLE dbo.AssetMaster
(
    AmId                INT IDENTITY(1,1)  NOT NULL,
    AmAtypeId           INT                NOT NULL,
    AmMakeId            INT                NOT NULL,   
    AmAdId              INT                NOT NULL,
    AmModel             VARCHAR(40)        NOT NULL,
    AmSnumber           VARCHAR(20)        NOT NULL,
    AmMyyear            VARCHAR(10)        NOT NULL,
    AmPdate             DATE               NOT NULL,
    AmWarranty          VARCHAR(1)         NOT NULL,   
    AmFrom              DATE               NOT NULL,
    AmTo                DATE               NOT NULL,
    AmPurchaseOrderId   INT                NULL,
    CONSTRAINT PK_AssetMaster PRIMARY KEY (AmId),
    CONSTRAINT UQ_AssetMaster_Snumber UNIQUE (AmSnumber),
    CONSTRAINT FK_AssetMaster_AssetType FOREIGN KEY (AmAtypeId) REFERENCES dbo.AssetType (AtId),
    CONSTRAINT FK_AssetMaster_Vendor FOREIGN KEY (AmMakeId) REFERENCES dbo.Vendor (VdId),
    CONSTRAINT FK_AssetMaster_AssetDefinition FOREIGN KEY (AmAdId) REFERENCES dbo.AssetDefinition (AdId),
    CONSTRAINT FK_AssetMaster_PurchaseOrder FOREIGN KEY (AmPurchaseOrderId) REFERENCES dbo.PurchaseOrder (PdId),
    CONSTRAINT CK_AssetMaster_Warranty CHECK (AmWarranty IN ('Y', 'N'))
);
GO
