using MongoDB.Bson;
using Server.Core.Auth;
using Server.Core.Helpers;
using Server.Models.Exceptions;
using Server.Persistence.Entity;
using Server.Persistence.MongoDb;
using Shared.Api.Request;
using Shared.Api.Result;
using Shared.DTOs;

namespace Server.Core.Services
{
    internal sealed class UserService
    {
        private readonly UserRepository _repository;
        private readonly TokenGenerator _tokenGenerator;

        public UserService(UserRepository repository, TokenGenerator tokenGenerator) =>
            (_repository, _tokenGenerator) = (repository, tokenGenerator);

        public async Task<UserDto> GetUserByNameAsync(string userName)
        {
            var userRaw = await _repository.GetByNameAsync(userName);

            if (userRaw is null)
            {
                throw new NotFoundUserInDbException();
            }

            return userRaw.MapToDto();
        }

        public async Task<UserDto> GetUserByIdAsync(ObjectId id)
        {
            var userRaw = await _repository.GetByIdAsync(id);

            if (userRaw is null)
            {
                throw new NotFoundUserInDbException();
            }

            return userRaw.MapToDto();
        }

        public async Task<UserDto> UpdateUser(ObjectId id, UserDto user)
        {
            UserDto result;

            var userRaw = user.MapToUser();

            var updateResult = await _repository.UpadteByIdAsync(id, userRaw);

            if (updateResult is null)
            {
                throw new NotFoundUserInDbException();
            }

            result = updateResult.MapToDto();

            return result;
        }

        public async Task<(string token, string userId)> GenerateTokenAsync(UserAuthRequest authRequest)
        { 
            string tokenResult = null;

            var dbUser = await _repository.GetByNameAsync(authRequest.UserName);

            if (dbUser is null)
            {
                throw new NotFoundUserInDbException();
            }
            var hashedPassword = SecurityHelper.HashPassword(authRequest.Password, dbUser.PasswordSalt);

            if (!hashedPassword.Equals(dbUser.Password))
            {
                throw new IncorrectPasswordException();
            }

            var loginResult = new SuccessLoginResult(dbUser.Id, authRequest.UserName);
            tokenResult = _tokenGenerator.GetForSuccessLoginResult(loginResult);

            return (tokenResult, dbUser.Id.ToString());
        }

        public async Task<(string token, string userId)> GenerateTokenForNewUserAsync(UserAuthRequest authRequest)
        {
            string tokenResult = null;

            var passwordSalt = SecurityHelper.GenerateSalt();
            var hashedPassword = SecurityHelper.HashPassword(authRequest.Password, passwordSalt);

            var userRaw = authRequest.MapToUser(hashedPassword, passwordSalt);

            var foundedUser = await _repository.GetByNameAsync(authRequest.UserName);
            
            if(foundedUser is not null)
            {
                throw new FoundUserInDbException();
            }

            await _repository.AddAsync(userRaw);

            var loginResult = new SuccessLoginResult(userRaw.Id, authRequest.UserName);

            tokenResult = _tokenGenerator.GetForSuccessLoginResult(loginResult);

            return (tokenResult, userRaw.Id.ToString());
        }

    }
}
