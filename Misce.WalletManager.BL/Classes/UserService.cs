using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.User;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;
using System.Security.Cryptography;

namespace Misce.WalletManager.BL.Classes
{
    public class UserService : IUserService
    {
        #region Properties

        private WalletManagerContext _walletManagerContext;

        #endregion

        #region CTORs

        public UserService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        #endregion

        #region Public Methods

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
                {
                    //update user's last login datetime
                    user.LastLoginDateTime = DateTime.UtcNow;

                    //and save changes in the db
                    _walletManagerContext.SaveChanges();

                    return new UserDTOOut
                    {
                        Id = user.Id,
                        Username = userLogin.Username
                    };
                }

                return null;
            }

            return null;
        }

        public UserDTOOut RegisterUser(UserSignInDTOIn user)
        {
            //user registration data validation
            var validationResults = Utils.Utils.ValidateDTO(user);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //check if the username is available
            var usersWithSameUsernameQuery = from u in _walletManagerContext.Users
                                             where u.Username == user.Username
                                             select u;

            if (usersWithSameUsernameQuery.Any())
                throw new UsernameNotAvailableException();

            //generate salt and hash the password
            var saltAndHashedPassword = GenerateSaltedHash(64, user.Password);

            //create the user
            var createdUser = new User
            {
                Username = user.Username,
                Password = saltAndHashedPassword.Hash,
                Salt = saltAndHashedPassword.Salt
            };
            _walletManagerContext.Users.Add(createdUser);

            //commit changes int he db
            _walletManagerContext.SaveChanges();

            //return the created user data
            return new UserDTOOut
            {
                Username = createdUser.Username,
                Id = createdUser.Id
            };
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region Private Classes

        private record HashSalt
        {
            public string Hash { get; init; } = null!;
            public string Salt { get; init; } = null!;
        }

        #endregion
    }
}
