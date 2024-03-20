using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Pet Insert(Pet pet)
        {
            pet.Id = Guid.NewGuid();
            collection.InsertOne(pet);
            return pet;
        }

        public Pet GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public IReadOnlyCollection<Pet> GetByOwnerId(Guid ownerId)
        {
            return collection.Find(x => x.OwnerId == ownerId).ToList();
        }

        public void Delete(Guid id)
        {
            collection.DeleteOne((x) => x.Id == id);
        }

        public async void CreateIndexes()
        {
            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Pet>(Builders<Pet>.IndexKeys.Ascending(_ => _.Id))).ConfigureAwait(false);

            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Pet>(Builders<Pet>.IndexKeys.Ascending(_ => _.OwnerId))).ConfigureAwait(false);
        }
    }
}
