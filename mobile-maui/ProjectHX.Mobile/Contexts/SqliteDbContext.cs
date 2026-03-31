using Microsoft.EntityFrameworkCore;
namespace ProjectHX.Mobile.Contexts
{
	public class SqliteDbContext : DbContext
	{
		private readonly AppStorageContext _appStorageContext;

		public SqliteDbContext(AppStorageContext appStorageContext)
		{
			_appStorageContext = appStorageContext;

			SQLitePCL.Batteries_V2.Init();
		}

		public SqliteDbContext(DbContextOptions<SqliteDbContext> options, AppStorageContext appStorageContext) : base(options)
		{
			_appStorageContext = appStorageContext;

			SQLitePCL.Batteries_V2.Init();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (optionsBuilder.IsConfigured)
			{
				return;
			}

			string dbPath = Path.Combine(_appStorageContext.DatabasePath, _appStorageContext.DatabaseFilename);

			optionsBuilder
				.UseSqlite($"Filename={dbPath}");
		}
	}
}
