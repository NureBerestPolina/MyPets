using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyPets.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MyPets.Repositories
{
    public class OwnerRepository
    {
        private readonly IMongoCollection<Owner> collection;
        public OwnerRepository(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("MongoDBConnection");
            collection = new MongoClient(connString)
                .GetDatabase("pets_db")
                .GetCollection<Owner>("owners");
        }
        public Owner Insert(Owner owner)
        {
            var existingOwner = GetByOwnerName(owner.OwnerName);
            if (existingOwner != null)
                throw new Exception("Pet owner with same name already exists");
               
            owner.Id = Guid.NewGuid();
            collection.InsertOne(owner);
            return owner;
        }

        public IReadOnlyCollection<Owner> GetAll()
        {
            return collection.Find(x => true).ToList();
        }

        public Owner GetById(Guid id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public Owner GetByOwnerName(string ownerName)
        {
            return collection.Find(x => x.OwnerName == ownerName).FirstOrDefault();
        }

        public Owner GetByNameAndPassword(string ownerName, string password)
        {
            return collection.Find(x => x.OwnerName == ownerName && x.Password == password).FirstOrDefault();
        }

        public async void CreateIndexes()
        {
            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Owner>(Builders<Owner>.IndexKeys.Ascending(_ => _.Id))).ConfigureAwait(false);

            await collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Owner>(Builders<Owner>.IndexKeys.Ascending(_ => _.OwnerName))).ConfigureAwait(false);
        }
    }
}
