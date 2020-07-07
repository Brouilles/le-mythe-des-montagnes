using Henagone_WebGame.Models.AdminViewModels;
using Henagone_WebGame.Models.GameViewModels;
using Henagone_WebGame.Models.ShopViewModels;
using Microsoft.EntityFrameworkCore;

namespace Henagone_WebGame.Models
{
    public class SiteDbContext : DbContext
    {
        // Game
        public DbSet<Characters> Characters { get; set; }
        public DbSet<Gold> Gold { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Potions> Potions { get; set; }
        public DbSet<Buff> Buff { get; set; }
        public DbSet<AtWork> AtWork { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Companions> Companions { get; set; }
        public DbSet<CompanionQuests> CompanionQuests { get; set; }
        public DbSet<OnQuest> OnQuest { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<Monsters> Monsters { get; set; }
        public DbSet <MonstersType> MonstersType { get; set; }
        public DbSet<HeroQuests> HeroQuests { get; set; }
        public DbSet<HeroesQuestsState> HeroesQuestsState { get; set; }
        public DbSet<AtBattlePVE> AtBattlePVE { get; set; }

        public DbSet<NarrativeQuest> NarrativeQuest { get; set; }
        public DbSet<NarrativeQuestStep> NarrativeQuestStep { get; set; }
        public DbSet<AtNarrativeQuest> AtNarrativeQuest { get; set; }

        public DbSet<Hunt_statistics> Hunt_statistics { get; set; }

        // Admin
        public DbSet<Conf> Conf { get; set; }
        public DbSet<Connections_statistics> Connections_statistics { get; set; }

        // Shop
        public DbSet<ArtifactsPrices> ArtifactsPrices { get; set; }
        public DbSet<Artifacts> Artifacts { get; set; }
        public DbSet<Gifts> Gifts { get; set; }
        public DbSet<Deals_statistics> Deals_statistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LeMytheDesMontagnes;Trusted_Connection=True;MultipleActiveResultSets=true");
            /*optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=LeMytheDesMontagnes;Trusted_Connection=True;MultipleActiveResultSets=true;"); /* Integrated Security=SSPI */
        }
    }
}
