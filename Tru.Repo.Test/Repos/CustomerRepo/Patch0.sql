CREATE TABLE TruTest.Customer
(
	Email NVARCHAR(100) NOT NULL,
	FirstName NVARCHAR(100) NOT NULL,
	LastName NVARCHAR(100) NOT NULL,
	CONSTRAINT PK_TruTest_Customer_Email PRIMARY KEY CLUSTERED (Email)
)

CREATE NONCLUSTERED INDEX IDX_TruTest_Customer_FirstName ON TruTest.Customer (FirstName)
CREATE NONCLUSTERED INDEX IDX_TruTest_Customer_LastName ON TruTest.Customer (LastName)