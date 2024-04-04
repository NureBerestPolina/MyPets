using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MyPets.Dtos;

namespace MyPets.Repositories
{
    public class PetRepository
    {
        private readonly IMongoCollection<Pet> collection;
        public PetRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString).GetDatabase("pets_db").GetCollection<Pet>("pets");
        }

        public async Task<Pet> Insert(Pet pet)
        {
            pet.Id = Guid.NewGuid();
            await collection.InsertOneAsync(pet);
            return pet;
        }

        public async Task<Pet> GetById(Guid id)
        {
            return (await collection.FindAsync(x => x.Id == id)).FirstOrDefault();
        }

        public async Task<List<Pet>> GetByOwnerId(Guid ownerId)
        {
            return (await collection.FindAsync(x => x.OwnerId == ownerId)).ToList();
        }

        public async Task<Pet> Delete(Guid id)
        {
            var a = await collection.FindOneAndDeleteAsync(x=>x.Id == id);
            return a;
        }

        public async Task<Pet> Edit(EditPetDto editPetDto)
        {
            var a = await collection.FindOneAndUpdateAsync(x => x.Id == editPetDto.Id,
                Builders<Pet>.Update
                    .Set(x => x.Name, editPetDto.Name)
                    .Set(x => x.Specie, editPetDto.Specie)
                    .Set(x => x.Breed, editPetDto.Breed));
            return a;
        }

        public async void CreateIndexes()
        {
            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Pet>(Builders<Pet>.IndexKeys.Ascending(_ => _.Id))).ConfigureAwait(false);

            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Pet>(Builders<Pet>.IndexKeys.Ascending(_ => _.OwnerId))).ConfigureAwait(false);
        }

        public async Task<List<Pet>> Search(string substring)
        {
            var regexPattern = new BsonRegularExpression(new System.Text.RegularExpressions.Regex(substring, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
            var result = await collection.Aggregate().Match(Builders<Pet>.Filter.Regex("Name", regexPattern)).ToListAsync();

            return result;
        }
    }
}
