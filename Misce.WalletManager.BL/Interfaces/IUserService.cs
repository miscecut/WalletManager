using Misce.WalletManager.DTO.DTO;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IUserService
    {
        public bool IsUsernameAlreadyTaken(string username);
        public UserDTOOut RegisterUser(UserLoginDTOIn user);
        public UserDTOOut Authenticate(UserLoginDTOIn userLogin);
    }
}
