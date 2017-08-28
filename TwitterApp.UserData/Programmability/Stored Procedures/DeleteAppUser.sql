CREATE PROCEDURE [dbo].[DeleteAppUser]
	@email varchar(60)
AS
	DELETE FROM [dbo].[AppUser] WHERE [Email] = @email;