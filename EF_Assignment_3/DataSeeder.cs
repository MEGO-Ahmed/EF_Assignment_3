using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Enums;
using OnlineLearningPlatform.Models;

namespace OnlineLearningPlatform.Data;

public static class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instructor>().HasData(
            new Instructor
            {
                Id = 1,
                FullName = "Dr. Tarek Mansour",
                Email = "tarek.mansour@platform.com",
                Bio = "Senior .NET Architect & Educator",
                JoinedDate = new DateTime(2024, 1, 15)
            },
            new Instructor
            {
                Id = 2,
                FullName = "Nadia El-Sayed",
                Email = "nadia.elsayed@platform.com",
                Bio = "UI/UX & Frontend Specialist",
                JoinedDate = new DateTime(2024, 3, 10)
            }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Programming", Description = "Software development topics", ParentCategoryId = null },
            new Category { Id = 2, Name = "Design", Description = "Digital product design", ParentCategoryId = null },
            new Category { Id = 3, Name = "Backend", Description = "Server-side engineering", ParentCategoryId = 1 },
            new Category { Id = 4, Name = "Frontend", Description = "Client-side web technologies", ParentCategoryId = 1 },
            new Category { Id = 5, Name = "ASP.NET", Description = "Microsoft web framework", ParentCategoryId = 3 }
        );

        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                Id = 1,
                Title = "EF Core Mastery",
                Description = "Deep dive into Entity Framework Core",
                Price = 49.99m,
                Level = CourseLevel.Intermediate,
                IsPublished = true,
                PublishedDate = new DateTime(2025, 2, 1),
                InstructorId = 1,
                CategoryId = 5
            },
            new Course
            {
                Id = 2,
                Title = "C# Fundamentals",
                Description = "Learn C# from scratch",
                Price = 29.99m,
                Level = CourseLevel.Beginner,
                IsPublished = true,
                PublishedDate = new DateTime(2025, 1, 10),
                InstructorId = 1,
                CategoryId = 3
            },
            new Course
            {
                Id = 3,
                Title = "Microservices with .NET",
                Description = "Distributed systems architecture",
                Price = 89.99m,
                Level = CourseLevel.Advanced,
                IsPublished = false,
                PublishedDate = null,
                InstructorId = 1,
                CategoryId = 5
            }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, FullName = "Omar Khaled", Email = "omar.k@student.com", DateOfBirth = new DateOnly(2001, 5, 12) },
            new Student { Id = 2, FullName = "Salma Hassan", Email = "salma.h@student.com", DateOfBirth = new DateOnly(2002, 8, 23) }
        );

        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                StudentId = 1,
                CourseId = 1,
                EnrollmentDate = new DateTime(2025, 3, 1),
                ProgressPercentage = 60,
                Status = EnrollmentStatus.Active,
                FinalGrade = null
            },
            new Enrollment
            {
                StudentId = 2,
                CourseId = 1,
                EnrollmentDate = new DateTime(2025, 3, 5),
                ProgressPercentage = 100,
                Status = EnrollmentStatus.Completed,
                FinalGrade = 95.50m
            }
        );
    }
}