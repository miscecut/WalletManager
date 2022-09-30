using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;
using System.Security.Cryptography;

namespace Misce.WalletManager.BL.Classes
{
    public class UserService : IUserService
    {
        private WalletManagerContext _walletManagerContext;

        public UserService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        public UserDTOOut? Authenticate(UserLoginDTOIn userLogin)
        {
            var query = from u in _walletManagerContext.Users
                        where u.Username == userLogin.Username
                        select u;

            if(query.Any())
            {
                var user = query.First();
                var storedSalt = user.Salt;
                var storedHash = user.Password;

                var saltBytes = Convert.FromBase64String(storedSalt);
                var rfc2898DeriveBytes = new Rfc2898DeriveBytes(userLogin.Password, saltBytes, 10000);
                var isPasswordCorrect =  Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;

                if (isPasswordCorrect)
                    return new UserDTOOut
                    {
                        Id = user.Id,
                        Username = userLogin.Username
                    };

                return null;
            }

            return null;
        }

        public bool IsUsernameAlreadyTaken(string username)
        {
            var query = from u in _walletManagerContext.Users
                        where u.Username == username
                        select u.Username;

            return query.Any();
        }

        public UserDTOOut RegisterUser(UserLoginDTOIn user)
        {
            //TODO: check username presence here
            var saltAndHashedPassword = GenerateSaltedHash(64, user.Password);

            var createdUser = new User
            {
                Username = user.Username,
                Password = saltAndHashedPassword.Hash,
                Salt = saltAndHashedPassword.Salt
            };

            _walletManagerContext.Users.Add(createdUser);
            _walletManagerContext.SaveChanges();

            return new UserDTOOut
            {
                Username = createdUser.Username,
                Id = createdUser.Id
            };
        }

        private HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }

        private record HashSalt
        {
            public string Hash { get; init; } = null!;
            public string Salt { get; init; } = null!;
        }
    }
}
