using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Realchat.Domain.Entities;

namespace Realchat.Infrastructure.Persistence.Contexts;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chatbot> Chatbots { get; set; }

    public virtual DbSet<InformationChunk> InformationChunks { get; set; }

    public virtual DbSet<KnowledgeBase> KnowledgeBases { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Script> Scripts { get; set; }

    public virtual DbSet<ScriptType> ScriptTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=password;database=realchat", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.0.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Chatbot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chatbot");

            entity.HasIndex(e => e.OrganizationId, "fk_chatbot_organization");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(50)
                .HasColumnName("display_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(36)
                .HasColumnName("organization_id");

            entity.HasOne(d => d.Organization).WithMany(p => p.Chatbots)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("fk_chatbot_organization");
        });

        modelBuilder.Entity<InformationChunk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("information_chunk");

            entity.HasIndex(e => e.KnowledgeBaseId, "fk_information_chunk_knowledge_base");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.ChunkNumber)
                .HasColumnType("int(11)")
                .HasColumnName("chunk_number");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.KnowledgeBaseId)
                .HasMaxLength(36)
                .HasColumnName("knowledge_base_id");

            entity.HasOne(d => d.KnowledgeBase).WithMany(p => p.InformationChunks)
                .HasForeignKey(d => d.KnowledgeBaseId)
                .HasConstraintName("fk_information_chunk_knowledge_base");
        });

        modelBuilder.Entity<KnowledgeBase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("knowledge_base");

            entity.HasIndex(e => e.ChatbotId, "fk_knowledge_base_chatbot");

            entity.HasIndex(e => e.OrganizationId, "fk_knowledge_base_organization");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.ChatbotId)
                .HasMaxLength(36)
                .HasColumnName("chatbot_id");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(36)
                .HasColumnName("organization_id");

            entity.HasOne(d => d.Chatbot).WithMany(p => p.KnowledgeBases)
                .HasForeignKey(d => d.ChatbotId)
                .HasConstraintName("fk_knowledge_base_chatbot");

            entity.HasOne(d => d.Organization).WithMany(p => p.KnowledgeBases)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("fk_knowledge_base_organization");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("organization");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(50)
                .HasColumnName("display_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Script>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("script");

            entity.HasIndex(e => e.ChatbotId, "fk_script_chatbot");

            entity.HasIndex(e => e.OrganizationId, "fk_script_organization");

            entity.HasIndex(e => e.ScriptTypeId, "fk_script_script_type");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasColumnType("text")
                .HasColumnName("action");
            entity.Property(e => e.ChatbotId)
                .HasMaxLength(36)
                .HasColumnName("chatbot_id");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(36)
                .HasColumnName("organization_id");
            entity.Property(e => e.ScriptTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("script_type_id");
            entity.Property(e => e.TriggerText)
                .HasMaxLength(100)
                .HasColumnName("trigger_text")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.Chatbot).WithMany(p => p.Scripts)
                .HasForeignKey(d => d.ChatbotId)
                .HasConstraintName("fk_script_chatbot");

            entity.HasOne(d => d.Organization).WithMany(p => p.Scripts)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("fk_script_organization");

            entity.HasOne(d => d.ScriptType).WithMany(p => p.Scripts)
                .HasForeignKey(d => d.ScriptTypeId)
                .HasConstraintName("fk_script_script_type");
        });

        modelBuilder.Entity<ScriptType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("script_type");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
