namespace ECommerceOS.AuthService.Infrastructure.Persistence.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                id => new UserId(id));
        
        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.Email)
            .HasMaxLength(100);
        
        builder.Property(user => user.Name)
            .HasMaxLength(100);
        
        builder.OwnsMany(user => user.RefreshTokens, tokens =>
            {
                tokens.ToTable("RefreshTokens");
                tokens.WithOwner().HasForeignKey(token => token.UserId);
                tokens.HasKey(token => new { token.Token, token.UserId });
            });
        
        builder.Navigation(tokens => tokens.RefreshTokens)
            .HasField("_refreshTokens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}