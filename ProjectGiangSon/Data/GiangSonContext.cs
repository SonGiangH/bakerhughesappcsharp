using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectGiangSon.Data;

public partial class GiangSonContext : DbContext
{
    public GiangSonContext()
    {
    }

    public GiangSonContext(DbContextOptions<GiangSonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Sensor> Sensors { get; set; }

    public virtual DbSet<Slide> Slides { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-8707B6N;Initial Catalog=GiangSon;Encrypt=false;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sensor__3213E83F3F77C327");

            entity.ToTable("Sensor");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Offset).HasColumnName("offset");
        });

        modelBuilder.Entity<Slide>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slides__3214EC2776C639BF");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.BitDepth).HasColumnName("Bit_Depth");
            entity.Property(e => e.Bur30m).HasColumnName("BUR30m");
            entity.Property(e => e.Burm).HasColumnName("BURm");
            entity.Property(e => e.DpLength).HasColumnName("DP_Length");
            entity.Property(e => e.Ed).HasColumnName("ED");
            entity.Property(e => e.Incbit).HasColumnName("INCBIT");
            entity.Property(e => e.MetterSeen).HasColumnName("MetterSEEN");
            entity.Property(e => e.St).HasColumnName("ST");
            entity.Property(e => e.SurveyDepth).HasColumnName("Survey_Depth");
            entity.Property(e => e.ToolFace).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
