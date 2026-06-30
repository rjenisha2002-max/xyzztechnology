/*vie assest master*/
CREATE VIEW dbo.vw_AssetMasterDetails
AS
SELECT
    am.AmId,
    am.AmAtypeId,
    at.AtName            AS AssetTypeName,
    am.AmMakeId,
    vd.VdName             AS VendorName,
    am.AmAdId,
    ad.AdName              AS AssetDefinitionName,
    ad.AdClass              AS AssetClass,
    am.AmModel,
    am.AmSnumber,
    am.AmMyyear,
    am.AmPdate,
    am.AmWarranty,
    am.AmFrom,
    am.AmTo,
    am.AmPurchaseOrderId,
    po.PdOrderNo           AS PurchaseOrderNo,
    po.PdStatus            AS PurchaseOrderStatus
FROM dbo.AssetMaster am
INNER JOIN dbo.AssetType at        ON am.AmAtypeId = at.AtId
INNER JOIN dbo.Vendor vd           ON am.AmMakeId = vd.VdId
INNER JOIN dbo.AssetDefinition ad  ON am.AmAdId = ad.AdId
LEFT  JOIN dbo.PurchaseOrder po    ON am.AmPurchaseOrderId = po.PdId;
GO

/*getallassests*/
CREATE PROCEDURE dbo.usp_GetAllAssets
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.vw_AssetMasterDetails ORDER BY AmId DESC;
END
GO

/*get assestby id*/
CREATE PROCEDURE dbo.usp_GetAssetById
    @AmId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.vw_AssetMasterDetails WHERE AmId = @AmId;
END
GO
/*searchassest*/
CREATE PROCEDURE dbo.usp_SearchAssets
    @Keyword VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.vw_AssetMasterDetails
    WHERE AmSnumber          LIKE '%' + @Keyword + '%'
       OR AmModel             LIKE '%' + @Keyword + '%'
       OR AssetTypeName        LIKE '%' + @Keyword + '%'
       OR VendorName            LIKE '%' + @Keyword + '%'
       OR AssetDefinitionName    LIKE '%' + @Keyword + '%'
    ORDER BY AmId DESC;
END
GO
/*create assest*/
CREATE PROCEDURE dbo.usp_CreateAsset
    @AmAtypeId          INT,
    @AmMakeId           INT,
    @AmAdId             INT,
    @AmModel            VARCHAR(40),
    @AmSnumber          VARCHAR(20),
    @AmMyyear           VARCHAR(10),
    @AmPdate            DATE,
    @AmWarranty         VARCHAR(1),
    @AmFrom             DATE,
    @AmTo               DATE,
    @AmPurchaseOrderId  INT = NULL,
    @NewAmId            INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @AmPurchaseOrderId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM dbo.PurchaseOrder
            WHERE PdId = @AmPurchaseOrderId
              AND PdStatus = 'Asset Details registered internally')
        BEGIN
            RAISERROR('Asset cannot be created: Purchase Order is not in ''Asset Details registered internally'' status.', 16, 1);
            RETURN;
        END
    END

    IF EXISTS (SELECT 1 FROM dbo.AssetMaster WHERE AmSnumber = @AmSnumber)
    BEGIN
        RAISERROR('An asset with this serial number already exists.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.AssetMaster
        (AmAtypeId, AmMakeId, AmAdId, AmModel, AmSnumber, AmMyyear, AmPdate, AmWarranty, AmFrom, AmTo, AmPurchaseOrderId)
    VALUES
        (@AmAtypeId, @AmMakeId, @AmAdId, @AmModel, @AmSnumber, @AmMyyear, @AmPdate, @AmWarranty, @AmFrom, @AmTo, @AmPurchaseOrderId);

    SET @NewAmId = SCOPE_IDENTITY();
END
GO

/*update assest*/
CREATE PROCEDURE dbo.usp_UpdateAsset
    @AmId               INT,
    @AmAtypeId          INT,
    @AmMakeId           INT,
    @AmAdId             INT,
    @AmModel            VARCHAR(40),
    @AmSnumber          VARCHAR(20),
    @AmMyyear           VARCHAR(10),
    @AmPdate            DATE,
    @AmWarranty         VARCHAR(1),
    @AmFrom             DATE,
    @AmTo               DATE,
    @AmPurchaseOrderId  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.AssetMaster WHERE AmSnumber = @AmSnumber AND AmId <> @AmId)
    BEGIN
        RAISERROR('Another asset already uses this serial number.', 16, 1);
        RETURN;
    END

    UPDATE dbo.AssetMaster
    SET AmAtypeId = @AmAtypeId,
        AmMakeId = @AmMakeId,
        AmAdId = @AmAdId,
        AmModel = @AmModel,
        AmSnumber = @AmSnumber,
        AmMyyear = @AmMyyear,
        AmPdate = @AmPdate,
        AmWarranty = @AmWarranty,
        AmFrom = @AmFrom,
        AmTo = @AmTo,
        AmPurchaseOrderId = @AmPurchaseOrderId
    WHERE AmId = @AmId;
END
GO
/*delete assest*/
CREATE PROCEDURE dbo.usp_DeleteAsset
    @AmId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.AssetMaster WHERE AmId = @AmId;
END
GO
/*validate user*/
CREATE PROCEDURE dbo.usp_ValidateUser
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LId, l.Username, l.Password, l.UserType, l.IsActive,
           ur.UId, ur.FirstName, ur.LastName, ur.Age, ur.Gender, ur.Address, ur.PhoneNumber
    FROM dbo.Login l
    LEFT JOIN dbo.UserRegistration ur ON ur.LId = l.LId
    WHERE l.Username = @Username AND l.IsActive = 1;
END
GO
