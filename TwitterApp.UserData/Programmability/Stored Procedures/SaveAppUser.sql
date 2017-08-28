CREATE PROCEDURE [dbo].[SaveAppUser]
	@id int,
	@email varchar(60),
	@password varchar(512),
	@type int
AS
	IF @id > 0 BEGIN
		UPDATE [dbo].[AppUser] SET Email = @email, [Type] = @type, [Password] = @password WHERE [Id] = @id

		END
	ELSE BEGIN
		INSERT INTO [dbo].[AppUser] ([Email], [Type], [Password]) VALUES (@email, @type, @password);
		SET @id = @@IDENTITY;

		END
	
	SELECT @id;