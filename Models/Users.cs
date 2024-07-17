namespace WebApplicationTRY_TWO.Models;


public class Users
{
    public int id { get; set; }
    public string user_name { get; set; } = string.Empty;
    public string? user_email { get; set; }
    public string? user_password { get; set; }
    public string user_type { get; set; } = string.Empty;
    
}