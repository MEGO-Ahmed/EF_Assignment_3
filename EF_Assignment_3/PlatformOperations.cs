using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Data;
using OnlineLearningPlatform.Enums;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Services;

public class PlatformOperations
{
    private readonly AppDbContext _context;

    public PlatformOperations(AppDbContext context)
    {
        _context = context;
    }

    

    public int AddInstructor(string fullName, string email, string? bio)
    {
        if (_context.Instructors.Any(i => i.Email == email))
        {
            Console.WriteLine($"[Refused] Instructor with email '{email}' already exists.");
            return -1;
        }

        var instructor = new Instructor
        {
            FullName = fullName,
            Email = email,
            Bio = bio,
            JoinedDate = DateTime.UtcNow
        };

        _context.Instructors.Add(instructor);
        _context.SaveChanges();

        Console.WriteLine($"[Created] Instructor '{instructor.FullName}' added with Id: {instructor.Id}");
        return instructor.Id;
    }

    public int AddCourse(string title, string? description, decimal price, CourseLevel level, int instructorId, int categoryId)
    {
        bool instructorExists = _context.Instructors.Any(i => i.Id == instructorId);
        if (!instructorExists)
        {
            Console.WriteLine($"[Refused] Cannot create course: Instructor Id {instructorId} does not exist.");
            return -1;
        }

        bool categoryExists = _context.Categories.Any(c => c.Id == categoryId);
        if (!categoryExists)
        {
            Console.WriteLine($"[Refused] Cannot create course: Category Id {categoryId} does not exist.");
            return -1;
        }

        var course = new Course
        {
            Title = title,
            Description = description,
            Price = price,
            Level = level,
            IsPublished = false,
            PublishedDate = null,
            InstructorId = instructorId,
            CategoryId = categoryId
        };

        _context.Courses.Add(course);
        _context.SaveChanges();

        Console.WriteLine($"[Created] Course '{course.Title}' added with Id: {course.Id} (Unpublished)");
        return course.Id;
    }

    public List<int> AddThreeStudents()
    {
        var newStudents = new List<Student>
        {
            new() { FullName = "Youssef Adel", Email = "youssef.a@student.com", DateOfBirth = new DateOnly(2000, 11, 2) },
            new() { FullName = "Mariam Nabil", Email = "mariam.n@student.com", DateOfBirth = new DateOnly(2003, 2, 19) },
            new() { FullName = "Karim Mostafa", Email = "karim.m@student.com", DateOfBirth = new DateOnly(2001, 7, 30) }
        };

        _context.Students.AddRange(newStudents);
        _context.SaveChanges();

        var ids = newStudents.Select(s => s.Id).ToList();
        Console.WriteLine($"[Created] 3 Students added in bulk. Generated Ids: {string.Join(", ", ids)}");
        return ids;
    }

    public void EnrollStudent(int studentId, int courseId)
    {
        var student = _context.Students.Find(studentId);
        if (student is null)
        {
            Console.WriteLine($"[Refused] Enrollment failed: Student Id {studentId} does not exist.");
            return;
        }

        var course = _context.Courses.Find(courseId);
        if (course is null)
        {
            Console.WriteLine($"[Refused] Enrollment failed: Course Id {courseId} does not exist.");
            return;
        }

        if (!course.IsPublished)
        {
            Console.WriteLine($"[Refused] Enrollment failed: Course '{course.Title}' (Id: {courseId}) is not published yet.");
            return;
        }

        bool alreadyEnrolled = _context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
        if (alreadyEnrolled)
        {
            Console.WriteLine($"[Refused] Enrollment failed: Student {studentId} is already enrolled in Course {courseId}.");
            return;
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            ProgressPercentage = 0,
            Status = EnrollmentStatus.Active,
            FinalGrade = null
        };

        _context.Enrollments.Add(enrollment);
        _context.SaveChanges();

        Console.WriteLine($"[Enrolled] Student '{student.FullName}' enrolled in '{course.Title}'.");
    }

   

    public void FindCourse(int id, string title)
    {
        var byId = _context.Courses.Find(id);
        Console.WriteLine(byId is null
            ? $"[Find by Id {id}] Not found."
            : $"[Find by Id {id}] Found: '{byId.Title}' (${byId.Price})");

        var byTitle = _context.Courses.FirstOrDefault(c => c.Title == title);
        Console.WriteLine(byTitle is null
            ? $"[FirstOrDefault '{title}'] Not found."
            : $"[FirstOrDefault '{title}'] Found: Id {byTitle.Id}, Level: {byTitle.Level}");
    }

    public void FilterCoursesByPrice(decimal minPrice, decimal maxPrice)
    {
        var courses = _context.Courses
            .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
            .OrderByDescending(c => c.Price)
            .Select(c => new { c.Title, c.Level, c.Price })
            .ToList();

        Console.WriteLine($"[Courses between ${minPrice} and ${maxPrice}]:");
        if (!courses.Any())
        {
            Console.WriteLine("  No courses found in this price range.");
            return;
        }

        foreach (var c in courses)
        {
            Console.WriteLine($"  - {c.Title} | Level: {c.Level} | Price: ${c.Price:F2}");
        }
    }

    public void PrintCategories(int parentCategoryId)
    {
        var subCategories = _context.Categories
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .ToList();

        Console.WriteLine($"[Direct Sub-Categories of Category Id {parentCategoryId}]:");
        if (!subCategories.Any())
            Console.WriteLine("  None found.");
        else
            subCategories.ForEach(c => Console.WriteLine($"  - [{c.Id}] {c.Name}"));

        var topLevelCategories = _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .ToList();

        Console.WriteLine("[Top-Level Categories]:");
        topLevelCategories.ForEach(c => Console.WriteLine($"  - [{c.Id}] {c.Name}"));
    }

    public void PrintCourseRoster(int courseId)
    {
        var course = _context.Courses.Find(courseId);
        if (course is null)
        {
            Console.WriteLine($"[Roster] Course Id {courseId} not found.");
            return;
        }

        List<int> studentIds = _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToList();

        List<string> studentNames = _context.Students
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => s.FullName)
            .ToList();

        Console.WriteLine($"[Course Roster for '{course.Title}'] Total Enrolled: {studentNames.Count}");
        foreach (var name in studentNames)
        {
            Console.WriteLine($"  - {name}");
        }
    }

   

    public void ChangeCoursePrice(int courseId, decimal newPrice)
    {
        if (newPrice < 0)
        {
            Console.WriteLine("[Invalid Input] Price cannot be negative.");
            return;
        }

        var course = _context.Courses.Find(courseId);
        if (course is null)
        {
            Console.WriteLine($"[Not Found] Course Id {courseId} does not exist.");
            return;
        }

        decimal oldPrice = course.Price;
        course.Price = newPrice;
        _context.SaveChanges();

        Console.WriteLine($"[Updated Price] '{course.Title}': ${oldPrice:F2} -> ${course.Price:F2}");
    }

    public void UpdateEnrollmentProgress(int studentId, int courseId, int newProgress, decimal? finalGrade = null)
    {
        if (newProgress < 0 || newProgress > 100)
        {
            Console.WriteLine($"[Rejected] Progress {newProgress}% is invalid. Must be between 0 and 100.");
            return;
        }

        var enrollment = _context.Enrollments.Find(studentId, courseId);
        if (enrollment is null)
        {
            Console.WriteLine($"[Not Found] Enrollment for Student {studentId} in Course {courseId} not found.");
            return;
        }

        enrollment.ProgressPercentage = newProgress;

        if (newProgress == 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.FinalGrade = finalGrade;
        }

        _context.SaveChanges();
        Console.WriteLine($"[Updated Progress] Student {studentId}, Course {courseId} -> {enrollment.ProgressPercentage}% ({enrollment.Status})");
    }

    public void PublishCourse(int courseId)
    {
        var course = _context.Courses.Find(courseId);
        if (course is null)
        {
            Console.WriteLine($"[Not Found] Course Id {courseId} does not exist.");
            return;
        }

        if (course.IsPublished)
        {
            Console.WriteLine($"[Info] Course '{course.Title}' is already published.");
            return;
        }

        course.IsPublished = true;
        course.PublishedDate = DateTime.UtcNow;
        _context.SaveChanges();

        Console.WriteLine($"[Published] Course '{course.Title}' published on {course.PublishedDate:yyyy-MM-dd}.");
    }

    

    public void UnenrollStudent(int studentId, int courseId)
    {
        var enrollment = _context.Enrollments.Find(studentId, courseId);
        if (enrollment is null)
        {
            Console.WriteLine($"[Not Found] No enrollment found for Student {studentId} in Course {courseId}.");
            return;
        }

        _context.Enrollments.Remove(enrollment);
        _context.SaveChanges();

        Console.WriteLine($"[Deleted] Unenrolled Student {studentId} from Course {courseId}.");
    }

    public void DeleteCategoryUnsafe(int categoryId)
    {
        var category = _context.Categories.Find(categoryId);
        if (category is null)
        {
            Console.WriteLine($"[Not Found] Category Id {categoryId} does not exist.");
            return;
        }

        try
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
            Console.WriteLine($"[Deleted] Category '{category.Name}' removed.");
        }
        catch (DbUpdateException ex)
        {
            _context.Entry(category).State = EntityState.Unchanged;
            Console.WriteLine($"[Caught DbUpdateException] Cannot delete category '{category.Name}': {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public void DeleteCategorySafe(int categoryId)
    {
        var category = _context.Categories.Find(categoryId);
        if (category is null)
        {
            Console.WriteLine($"[Not Found] Category Id {categoryId} does not exist.");
            return;
        }

        bool hasSubCategories = _context.Categories.Any(c => c.ParentCategoryId == categoryId);
        if (hasSubCategories)
        {
            Console.WriteLine($"[Safe Check Blocked] Category '{category.Name}' still has sub-categories and cannot be deleted.");
            return;
        }

        bool hasCourses = _context.Courses.Any(c => c.CategoryId == categoryId);
        if (hasCourses)
        {
            Console.WriteLine($"[Safe Check Blocked] Category '{category.Name}' still has assigned courses and cannot be deleted.");
            return;
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();
        Console.WriteLine($"[Safe Delete] Category '{category.Name}' deleted successfully.");
    }

    public void DeleteCourseWithEnrollments(int courseId)
    {
        var course = _context.Courses.Find(courseId);
        if (course is null)
        {
            Console.WriteLine($"[Not Found] Course Id {courseId} does not exist.");
            return;
        }

        int enrollmentsBefore = _context.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"[Before Course Delete] Enrollments for Course {courseId} ('{course.Title}'): {enrollmentsBefore}");

        _context.Courses.Remove(course);
        _context.SaveChanges();

      

        int enrollmentsAfter = _context.Enrollments.Count(e => e.CourseId == courseId);
        Console.WriteLine($"[After Course Delete] Enrollments for Course {courseId}: {enrollmentsAfter}");
    }
}