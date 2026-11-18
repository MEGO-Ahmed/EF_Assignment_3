using OnlineLearningPlatform.Data;
using OnlineLearningPlatform.Enums;
using OnlineLearningPlatform.Services;

namespace OnlineLearningPlatform
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();
            var ops = new PlatformOperations(context);

            Console.WriteLine("=== 1. CREATE OPERATIONS ===");
            int newInstructorId = ops.AddInstructor("Hana Mahmoud", "hana.m@platform.com", "Cloud & DevOps Engineer");
            int newCourseId = ops.AddCourse("Docker & Kubernetes", "Container orchestration essentials", 59.99m, CourseLevel.Intermediate, newInstructorId, categoryId: 3);

            ops.AddCourse("Invalid Course", null, 10m, CourseLevel.Beginner, instructorId: 999, categoryId: 3);

            List<int> studentIds = ops.AddThreeStudents();
            int firstNewStudentId = studentIds[0];

            ops.EnrollStudent(firstNewStudentId, newCourseId);

            ops.EnrollStudent(firstNewStudentId, courseId: 1);

            ops.EnrollStudent(firstNewStudentId, courseId: 1);


            Console.WriteLine("\n=== 2. READ OPERATIONS ===");
            ops.FindCourse(id: 1, title: "C# Fundamentals");
            ops.FindCourse(id: 999, title: "Non-Existent Course");
            ops.FilterCoursesByPrice(minPrice: 25m, maxPrice: 70m);
            ops.PrintCategories(parentCategoryId: 1);
            ops.PrintCourseRoster(courseId: 1);


            Console.WriteLine("\n=== 3. UPDATE OPERATIONS ===");
            ops.ChangeCoursePrice(courseId: 2, newPrice: 34.99m);

            ops.UpdateEnrollmentProgress(firstNewStudentId, courseId: 1, newProgress: 120);

            ops.UpdateEnrollmentProgress(firstNewStudentId, courseId: 1, newProgress: 100, finalGrade: 92.0m);

            ops.PublishCourse(newCourseId);
            ops.EnrollStudent(firstNewStudentId, newCourseId);


            Console.WriteLine("\n=== 4. DELETE OPERATIONS ===");
            ops.UnenrollStudent(firstNewStudentId, newCourseId);

            ops.DeleteCategoryUnsafe(categoryId: 1);

            ops.DeleteCategorySafe(categoryId: 1);
            ops.DeleteCourseWithEnrollments(courseId: 1);
        }
    }
}