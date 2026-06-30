/*use db*/

USE AssetMgmtDB;
GO

/*assettype*/
INSERT INTO dbo.AssetType (AtId, AtName) VALUES
(1, 'Mobile'),
(2, 'Thermal Printer'),
(3, 'Sensor'),
(4, 'Gateway'),
(5, 'Laptop'),
(6, 'Boom Barrier');

GO

/*assest definition*/
INSERT INTO dbo.AssetDefinition (AdId, AdName, AdTypeId, AdClass) VALUES
(1,  'Geo Magnetic Sensor - Ground', 3, 'HW'),
(2,  'LoRa Getway - Tata',           4, 'HW'),
(3,  'Mobile Phone',                 1, 'HW'),
(4,  'Laser Printer - Colour',       2, 'HW'),
(5,  'Laptop',                       5, 'HW'),
(6,  'Desktop',                      5, 'HW'),
(7,  'Mobile Charger',               1, 'HW'),
(8,  'Printer Charger',              2, 'HW'),
(9,  'Thermal Printer',              2, 'HW'),
(10, 'Lora Getaway - ICFOSS',        4, 'HW');

GO

/*vendor*/
INSERT INTO dbo.Vendor (VdId, VdName, VdType, VdAtypeId, VdFrom, VdTo, VdAddr) VALUES
(1, 'Samsung',       'Supplier', 1, '2019-06-15', '2099-12-31', 'Bengaluru, Karnataka'),
(2, 'MI',             'Supplier', 1, '2019-06-15', '2099-12-31', 'Bengaluru, Karnataka'),
(3, 'Vivo',           'Supplier', 1, '2019-06-15', '2099-12-31', 'Greater Noida, UP'),
(4, 'Softland India', 'Supplier', 2, '2019-06-15', '2099-12-31', 'Kochi, Kerala'),
(5, 'Mobio',          'Supplier', 5, '2019-06-15', '2099-12-31', 'Chennai, Tamil Nadu'),
(6, 'ICOFSS',         'Supplier', 4, '2019-06-15', '2099-12-31', 'Thiruvananthapuram, Kerala'),
(7, 'WiFi solutions',  'Supplier', 4, '2019-06-15', '2099-12-31', 'Kochi, Kerala'),
(8, 'Talent Services', 'Supplier', 3, '2019-06-15', '2099-12-31', 'Kochi, Kerala');

GO

/*purchase order*/

INSERT INTO dbo.PurchaseOrder (PdId, PdOrderNo, PdAdId, PdTypeId, PdQty, PdVendorId, PdDate, PdDdate, PdStatus) VALUES
(1, 'PO0001', 3, 1, 25, 1, '2025-01-10', '2025-01-25', 'Asset Details registered internally'),
(2, 'PO0002', 5, 5, 10, 5, '2025-02-05', '2025-02-20', 'Asset Details registered internally'),
(3, 'PO0003', 9, 2, 5,  4, '2025-02-15', '2025-03-01', 'Consignment Received'),
(4, 'PO0004', 2, 4, 8,  6, '2025-03-01', '2025-03-15', 'PO - Raised with Supplier'),
(5, 'PO0005', 1, 3, 50, 8, '2025-03-10', '2025-03-25', 'Awaiting Delivery by Supplier'),
(6, 'PO0006', 6, 5, 15, 5, '2025-04-01', '2025-04-15', 'Asset Allocated to Resources');
/*insert values*/
INSERT INTO dbo.Login (Username, Password, UserType, IsActive) VALUES
('admin', '$2b$11$EFstR6oh.jnJBXZLoJbGtOTkxdG93OjexwWrtW.WfGbwxLJUDKLFa', 'Admin', 1);

INSERT INTO dbo.UserRegistration (FirstName, LastName, Age, Gender, Address, PhoneNumber, LId)
VALUES ('System', 'Administrator', 30, 'Male', 'XYZ Technologies HQ', '9999999999',
        (SELECT LId FROM dbo.Login WHERE Username = 'admin'));
GO
