CREATE PROCEDURE [dbo].[GetUser]
	@email varchar(60)
AS
	SELECT A.[Id], [Email], [Password], [Type], [Name] as 'TypeName'
	FROM [dbo].[AppUser] (nolock) A
	JOIN [dbo].[AppUserType] (nolock) Ay ON Ay.Id = A.[Type]
	WHERE A.[Email] = @email;
