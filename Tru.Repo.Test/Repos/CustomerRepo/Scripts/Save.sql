IF EXISTS (SELECT * FROM TruTest.Customer WHERE Email = @Email)
BEGIN
	UPDATE TruTest.Customer SET FirstName = @FirstName, LastName = @LastName WHERE Email = @Email
END
ELSE
BEGIN
	INSERT INTO TruTest.Customer VALUES(@Email, @FirstName, @LastName)
END