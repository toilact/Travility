namespace Travility.Data.Model
{
    public partial class User
    {
        public bool IsAdmin
        {
            get { return Role != null && Role.Name == "Admin"; }
        }
    }
}
