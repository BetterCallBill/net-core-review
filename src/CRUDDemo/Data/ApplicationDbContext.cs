
using Microsoft.EntityFrameworkCore;

namespace CRUDDemo.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

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

        //Fluent API
        modelBuilder.Entity<Person>().Property(temp => temp.TIN)
          .HasColumnName("TaxIdentificationNumber")
          .HasColumnType("varchar(8)")
          .HasDefaultValue("ABC12345");
    }
}
