using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Auth;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Search;
using MyPets.Dtos;


namespace MyPets.Repositories
{
    public class OwnerRepository
    {
        private readonly IMongoCollection<Owner> collection;
        private readonly IMongoCollection<Pet> petCollection;
        private readonly PetRepository _petRepository;
        private readonly JwtProvider _jwtProvider;
        public OwnerRepository(IConfiguration configuration, JwtProvider jwtProvider, PetRepository petRepository)
        {
            _jwtProvider = jwtProvider;
            _petRepository = petRepository;
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pets_db")
                .GetCollection<Owner>("owners");
            petCollection = new MongoClient(connString)
                .GetDatabase("pets_db")
                .GetCollection<Pet>("pets");
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

        public async Task<Owner> GetById(Guid id)
        {
            return await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
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
            var owner = await collection.FindOneAndDeleteAsync(x => x.OwnerName == ownerName && x.Password == password);
            var deletedCount = await _petRepository.DeleteRange(owner.Id);
            return owner;
        }

        public async Task<List<Owner>> GetNearestOwnersOfSpecie(double latitude, double longtitude, string specie,
            double radius)
        {
            var filter = Builders<Owner>.Filter.GeoWithinCenter(x => x.Location, latitude,longtitude, radius);

            var a = await collection.Aggregate().Match(filter).ToListAsync();
            var b = (await petCollection.Aggregate().ToListAsync())
                .Where(x => x.Specie == specie)
                .DistinctBy(x=>x.OwnerId)
                .Select(x => x.OwnerId)
                .ToList();
            var af = a.Where(x => b.Contains(x.Id)).ToList();
            return af;
        }

        public async Task<(double, double)> GetLoc(Guid id)
        {
            var a = (await collection.FindAsync(x => x.Id == id)).FirstOrDefault();
            return (a.Location.Coordinates.X, a.Location.Coordinates.Y);
        }
    }
}
