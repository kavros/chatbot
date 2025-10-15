using Amazon;
using Amazon.DSQL.Util;
using Amazon.Runtime.Credentials;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public new Guid Id { get; set; }
}
public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public new Guid Id { get; set; }
}
public class UserDbContext : IdentityDbContext<
    IdentityUser<Guid>,
    IdentityRole<Guid>,
    Guid,
    ApplicationUserClaim,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    ApplicationRoleClaim,
    IdentityUserToken<Guid>>
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
    : base(options)
    {
    }
    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var clusterUser = Environment.GetEnvironmentVariable("CLUSTER_USER");
        var clusterEndpoint = Environment.GetEnvironmentVariable("CLUSTER_ENDPOINT");
        var region = Environment.GetEnvironmentVariable("REGION");

        // Set up DI
        var awsCredentials = DefaultAWSCredentialsIdentityResolver.GetCredentialsAsync().GetAwaiter().GetResult();
        var regionName = RegionEndpoint.GetBySystemName(region!);

        var password = await DSQLAuthTokenGenerator.GenerateDbConnectAdminAuthTokenAsync(
               awsCredentials, regionName, clusterEndpoint);

        var connectionString = $"Host={clusterEndpoint};" +
            $"Port=5432;Database=postgres;Username={clusterUser};" +
            $"Password={password};" +
            "SslMode=Require;Trust Server Certificate=true;NoResetOnClose=true;";

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationRoleClaim>(b =>
        {
            b.HasKey(rc => rc.Id);
            b.Property(rc => rc.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()"); // For PostgreSQL
        });

        builder.Entity<ApplicationUserClaim>(b =>
        {
            b.HasKey(uc => uc.Id);
            b.Property(uc => uc.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });
        builder.Entity<IdentityUser<Guid>>()
            .HasMany<IdentityUserRole<Guid>>()
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired(false);

        // Remove all foreign keys because foreign keys are not suppored by Aurora DSQL.
        var foreignKeysToRemove = new List<(string, string)>
        {
            ("AspNetUserRoles", "AspNetUsers"),
            ("AspNetUserRoles", "AspNetRoles"),
            ("AspNetUserClaims", "AspNetUsers"),
            ("AspNetRoleClaims", "AspNetRoles"),
            ("AspNetUserLogins", "AspNetUsers"),
            ("AspNetUserTokens", "AspNetUsers")
        };

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName == null) continue;

            // Find the foreign keys to remove for the current table.
            var fks = entityType.GetForeignKeys()
                .Where(fk => fk.PrincipalEntityType.GetTableName() != null &&
                             foreignKeysToRemove.Contains((tableName, fk.PrincipalEntityType.GetTableName()!)))
                .ToList();

            if (fks.Any())
            {
                foreach (var fk in fks)
                {
                    entityType.RemoveForeignKey(fk);
                }
            }
        }
    }
}