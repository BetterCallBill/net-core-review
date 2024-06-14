using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions<PersonsDbContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Persons");
            modelBuilder.Entity<Country>().ToTable("Countries");

            //Seed to Countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
                modelBuilder.Entity<Country>().HasData(country);


            //Seed to Persons
            string personsJson = File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);
        }
    }
}