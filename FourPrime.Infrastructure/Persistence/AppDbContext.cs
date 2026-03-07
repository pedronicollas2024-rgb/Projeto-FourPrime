using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FourPrime.Infrastructure.Entities;     // ApplicationUser
using FourPrime.Domain.Entities;            // Marca, Categoria, Carro, TipoUsuario, Usuario, Sessao, TokenRecuperacaoSenha

namespace FourPrime.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // =======================
    // Domínio (site)
    // =======================
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Carro> Carros => Set<Carro>();
    public DbSet<TipoUsuario> TiposUsuario => Set<TipoUsuario>();

    // =======================
    // Login desktop (antigo)
    // =======================
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Sessao> Sessoes => Set<Sessao>();
    public DbSet<TokenRecuperacaoSenha> TokensRecuperacaoSenha => Set<TokenRecuperacaoSenha>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =======================
        // ApplicationUser (Identity)
        // =======================
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.Ativo)
                  .IsRequired()
                  .HasDefaultValue(true);

            entity.Property(e => e.NomeCompleto)
                  .HasMaxLength(200);

            entity.HasOne(u => u.TipoUsuario)
                .WithMany()
                .HasForeignKey(u => u.TipoUsuarioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =======================
        // Marca
        // =======================
        builder.Entity<Marca>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Nome)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(m => m.PaisOrigem)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasMany(m => m.Carros)
                  .WithOne(c => c.Marca)
                  .HasForeignKey(c => c.MarcaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =======================
        // Categoria
        // =======================
        builder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nome)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(c => c.Descricao)
                  .HasMaxLength(500);

            entity.HasMany(c => c.Carros)
                  .WithOne(carro => carro.Categoria)
                  .HasForeignKey(carro => carro.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =======================
        // Carro
        // =======================
        builder.Entity<Carro>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Modelo)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(c => c.Cor)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(c => c.Combustivel)
                  .IsRequired()
                  .HasMaxLength(30);

            entity.Property(c => c.ImagemUrl)
                  .HasMaxLength(500);

            entity.Property(c => c.Descricao)
                  .HasMaxLength(1000);

            entity.Property(c => c.Preco)
                  .HasColumnType("decimal(18,2)");

            entity.HasOne(c => c.Marca)
                  .WithMany(m => m.Carros)
                  .HasForeignKey(c => c.MarcaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Categoria)
                  .WithMany(cat => cat.Carros)
                  .HasForeignKey(c => c.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.Ano);
            entity.HasIndex(c => c.Preco);
        });

        // =======================
        // TipoUsuario
        // =======================
        builder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Nome)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(t => t.Descricao)
                  .HasMaxLength(200);
        });

        // =======================
        // Usuario (desktop)
        // =======================
        builder.Entity<Usuario>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.NomeDeUsuario).HasMaxLength(50).IsRequired();
            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.Property(x => x.HashSenha).HasMaxLength(300).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.NomeDeUsuario).IsUnique();
        });

        // =======================
        // Sessao (desktop)
        // =======================
        builder.Entity<Sessao>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TokenSessao).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.TokenSessao).IsUnique();
        });

        // =======================
        // TokenRecuperacaoSenha (desktop)
        // =======================
        builder.Entity<TokenRecuperacaoSenha>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
        });
    }
}