namespace JwtAppApi.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel() { UserName = "marci_admin", EmailAddress = "janosmarci@gmail.com", Password =
                "jelszo", GivenName = "Marci", SurName = "Janos", Role = "Adminastrator"},
            new UserModel() { UserName = "anna_seller", EmailAddress = "anna@gmail.com", Password =
                "1234", GivenName = "Anna", SurName = "Bog", Role = "Seller"}
        };
    }
}
