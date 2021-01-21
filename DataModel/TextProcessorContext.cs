using Microsoft.EntityFrameworkCore;

namespace TextProcessor.DataModel
{
    class TextProcessorContext : DbContext
    {
        public DbSet<FrequentWord> FrequentWords { get; set; }

        public TextProcessorContext()
        {
            Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=textproc;Trusted_Connection=True;");
        }
    }
}
