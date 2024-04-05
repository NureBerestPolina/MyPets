using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Auth;
using MyPets.Dtos;


namespace MyPets.Repositories
{
    public class OwnerRepository
    {
        private readonly IMongoCollection<Owner> collection;
        private readonly JwtProvider _jwtProvider;
        public OwnerRepository(IConfiguration configuration, JwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pets_db")
                .GetCollection<Owner>("owners");
        }
        public async Task<Owner> Insert(Owner owner)
        {
            var existingOwner = await GetByOwnerName(owner.OwnerName);
            if (existingOwner != null)
                throw new Exception("Pet owner with same name already exists");
               
            owner.Id = Guid.NewGuid();
            await collection.InsertOneAsync(owner);
            return owner;
        }

        public async Task<IReadOnlyCollection<Owner>> GetAll()
        {
            var result = await collection.FindAsync(x => true);
            return result.ToList();
        }

        public Owner GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public async Task<Owner> GetByOwnerName(string ownerName)
        {
            var result = await collection.FindAsync(x => x.OwnerName == ownerName);
            return result.FirstOrDefault();
        }

        private async Task<Owner> GetByNameAndPassword(string ownerName, string password)
        {
            return (await collection.FindAsync(x => x.OwnerName == ownerName && x.Password == password)).FirstOrDefault();
        }

        public async void CreateIndexes()
        {
            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Owner>(Builders<Owner>.IndexKeys.Ascending(_ => _.Id))).ConfigureAwait(false);

            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Owner>(Builders<Owner>.IndexKeys.Ascending(_ => _.OwnerName))).ConfigureAwait(false);
        }

        public async Task<LoginResponse> Login(LoginDto model)
        {
            var user = await GetByNameAndPassword(model.OwnerName, model.Password);
            var token = _jwtProvider.Generate(user);
            return new LoginResponse
            {
                Token = token
            };
        }

        public async Task<Owner> ChangePassword(Guid userId, string newPassword)
        {
            var a = await collection.FindOneAndUpdateAsync(x => x.Id == userId, Builders<Owner>.Update.Set(x=>x.Password, newPassword));
            return a;
        }

        public async Task<Owner> Delete(string ownerName, string password)
        {
            var a = await collection.FindOneAndDeleteAsync(x => x.OwnerName == ownerName && x.Password == password);
            return a;
        }
    }
}
