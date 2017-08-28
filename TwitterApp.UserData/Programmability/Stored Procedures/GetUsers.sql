CREATE PROCEDURE [dbo].[GetUsers]
AS
	SELECT A.[Id], [Email], [Password], [Type], [Name] as 'TypeName'
	FROM [dbo].[AppUser] (nolock) A
	JOIN [dbo].[AppUserType] (nolock) Ay ON Ay.Id = A.[Type]
