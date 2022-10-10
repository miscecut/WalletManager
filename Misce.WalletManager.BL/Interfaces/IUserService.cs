using Misce.WalletManager.DTO.DTO.User;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IUserService
    {
        public UserDTOOut RegisterUser(UserSignInDTOIn user);
        public UserDTOOut Authenticate(UserLoginDTOIn userLogin);
    }
}
