namespace TwitterApp.Common
{
    /// <summary>
    /// General user object from the data base.
    /// </summary>
    public class AppUser
    {
        public AppUser() { }

        public AppUser(int id, string email, string password, AppUserType type)
        {
            Id = id;
            Email = email;
            Type = type;
            Password = password;
        }

        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public AppUserType Type { get; set; }

        public string TypeName { get; set; }

    }
}
