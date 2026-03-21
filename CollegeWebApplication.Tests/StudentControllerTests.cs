using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CollegeWebApplication.Controllers;
using CollegeWebApplication.Data;
using CollegeWebApplication.Models;

namespace CollegeWebApplication.Tests
{
    public class StudentControllerTests
    {
        private static CollegeWebDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CollegeWebDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new CollegeWebDbContext(options);
        }

        [Fact]
        public void Index_ReturnsStudentListView_WithAllStudents()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            context.StudentsInfo.AddRange(
                new StudentInfo { FirstName = "Alice", LastName = "A", Email = "a@example.com" },
                new StudentInfo { FirstName = "Bob", LastName = "B", Email = "b@example.com" }
            );
            context.SaveChanges();

            var controller = new StudentController(context);

            var result = controller.Index();
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("StudentList", view.ViewName);

            var model = Assert.IsAssignableFrom<System.Collections.Generic.List<StudentInfo>>(view.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Post_Valid_AddsStudentAndRedirects()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var controller = new StudentController(context);

            var student = new StudentInfo
            {
                FirstName = "Charlie",
                LastName = "C",
                Email = "c@example.com",
                DateBirth = DateTime.UtcNow.AddYears(-20),
                Gender = "Male",
                Address = "Some",
                City = "City",
                PhoneNumber = "1234567890"
            };

            var result = controller.Create(student);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var saved = context.StudentsInfo.SingleOrDefault(s => s.Email == "c@example.com");
            Assert.NotNull(saved);
            Assert.Equal("Charlie", saved.FirstName);
        }

        [Fact]
        public void Create_Post_Invalid_ReturnsViewWithModel()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);
            controller.ModelState.AddModelError("FirstName", "Required");

            var student = new StudentInfo();

            var result = controller.Create(student);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(student, view.Model);
            Assert.Empty(context.StudentsInfo.ToList());
        }

        [Fact]
        public void Edit_Get_Found_ReturnsViewWithStudent()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var student = new StudentInfo { StudentId = 1, FirstName = "D", LastName = "D", Email = "d@example.com" };
            context.StudentsInfo.Add(student);
            context.SaveChanges();

            var controller = new StudentController(context);

            var result = controller.Edit(1);
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StudentInfo>(view.Model);
            Assert.Equal("d@example.com", model.Email);
        }

        [Fact]
        public void Edit_Get_NotFound_RedirectsToIndex()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.Edit(999);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Edit_Post_Valid_UpdatesAndRedirects()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            context.StudentsInfo.Add(new StudentInfo { StudentId = 5, FirstName = "E", LastName = "Old", Email = "e@example.com" });
            context.SaveChanges();

            var controller = new StudentController(context);

            var updated = new StudentInfo
            {
                StudentId = 5,
                FirstName = "E",
                LastName = "New",
                Email = "e@example.com"
            };

            var result = controller.Edit(updated);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var saved = context.StudentsInfo.Find(5);
            Assert.Equal("New", saved.LastName);
            Assert.NotNull(saved.UpdateDate);
        }

        [Fact]
        public void Edit_Post_Invalid_ReturnsViewWithModel()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            context.StudentsInfo.Add(new StudentInfo { StudentId = 6, FirstName = "F", LastName = "F", Email = "f@example.com" });
            context.SaveChanges();

            var controller = new StudentController(context);
            controller.ModelState.AddModelError("FirstName", "Required");

            var student = new StudentInfo { StudentId = 6 };

            var result = controller.Edit(student);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Same(student, view.Model);
        }

        [Fact]
        public void Details_Get_NotFound_RedirectsToIndex()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.Details(1234);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void DeleteConfirmed_RemovesStudent_WhenExists()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            context.StudentsInfo.Add(new StudentInfo { StudentId = 10, FirstName = "G", Email = "g@example.com" });
            context.SaveChanges();

            var controller = new StudentController(context);

            var result = controller.DeleteConfirmed(10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            Assert.Null(context.StudentsInfo.Find(10));
        }

        [Fact]
        public void DeleteConfirmed_WithMissingId_RedirectsToIndex()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.DeleteConfirmed(9999);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}
