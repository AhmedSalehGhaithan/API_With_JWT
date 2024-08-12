namespace Shared_ClassLibrary.DTOs;
public class ServicesResponses
    {
        public record class GeneralResponse (bool Flag,string Message);
        public record class LoginResponse (bool Flag, string Token, string Message);
    }

