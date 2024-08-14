using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using herkesuyurkenkodlama.Models;
using Task = herkesuyurkenkodlama.Models.Task;

namespace herkesuyurkenkodlama.Contexts
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Mdepartment> Mdepartments { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Sdepartment> Sdepartments { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-9MUK3CO;Initial Catalog=deneme;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.SubDepartmentId).HasColumnName("SubDepartmentID");

                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_comment_mdepartment");

                entity.HasOne(d => d.SubDepartment)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.SubDepartmentId)
                    .HasConstraintName("FK_comment_sdepartment");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_comment_task");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_comment_user");
            });

            modelBuilder.Entity<Mdepartment>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);

                entity.ToTable("mdepartment");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.DepartmanName).HasMaxLength(50);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.OwnerUserId).HasColumnName("OwnerUserID");

                entity.Property(e => e.SubDepartmentId).HasColumnName("SubDepartmentID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_project_mdepartment");

                entity.HasOne(d => d.OwnerUser)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.OwnerUserId)
                    .HasConstraintName("FK_project_user");

                entity.HasOne(d => d.SubDepartment)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.SubDepartmentId)
                    .HasConstraintName("FK_project_sdepartment");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Rolename)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sdepartment>(entity =>
            {
                entity.HasKey(e => e.SubDepartmentId);

                entity.ToTable("sdepartment");

                entity.Property(e => e.SubDepartmentId).HasColumnName("SubDepartmentID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.SubDepartmentName).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Sdepartments)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_sdepartment_mdepartment");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("task");

                entity.Property(e => e.TaskId).HasColumnName("TaskID");

                entity.Property(e => e.AssignedUserId).HasColumnName("AssignedUserID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.SubDepartmentId).HasColumnName("SubDepartmentID");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.AssignedUser)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.AssignedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_user");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_task_mdepartment");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_project");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_status");

                entity.HasOne(d => d.SubDepartment)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.SubDepartmentId)
                    .HasConstraintName("FK_task_sdepartment");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SubDepartmentId).HasColumnName("SubDepartmentID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_user_mdepartment");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_role");

                entity.HasOne(d => d.SubDepartment)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.SubDepartmentId)
                    .HasConstraintName("FK_user_sdepartment");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
