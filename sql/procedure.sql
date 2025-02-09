CREATE PROCEDURE [dbo].[SP_LoginInfo]
	 @UserInfoId int
	,@EmailId varchar(MAX)
	,@Username varchar(MAX)	
	,@Password varchar(MAX)
	,@IsMailConfirmed bit
	,@OperationType int

AS
BEGIN TRAN
	
	IF(@OperationType = 1) --Signup
	BEGIN
		SET @UserInfoId = (SELECT MAX(UserInfoId) FROM LoginInfo) + 1

		INSERT INTO LoginInfo (UserInfoId,		EmailId,	Username,	[Password], IsMailConfirmed,		Created,		Updated)
			            VALUES(@UserInfoId,		@EmailId,	@Username,	@Password,	0,						GETDATE(),		GETDATE())

		SELECT * FROM LoginInfo WHERE UserInfoId=@UserInfoId

	END
	ELSE IF(@OperationType = 2) --Confirm Mail
	BEGIN
		IF (@Username = null OR @Username = '')
		BEGIN
			ROLLBACK
				RAISERROR (N'Invalid user. Please create account. !!!~',16,1);	
			RETURN
		END

		UPDATE LoginInfo SET IsMailConfirmed=1
					   WHERE Username=@Username

		SELECT * FROM LoginInfo WHERE Username=@Username
	END
COMMIT TRAN