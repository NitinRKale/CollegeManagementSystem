using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CollegeWebApplication.Data;
using CollegeWebApplication.Controllers;
using CollegeWebApplication.Models;
using System.Collections.Generic;

namespace CollegeWebApplication.Tests
{
    public class StudentControllerTests
    {
        private CollegeWebDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CollegeWebDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new CollegeWebDbContext(options);
        }

        [Test]
        public void Index_ReturnsStudentListView_WithSeededStudents()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            context.StudentsInfo.AddRange(
             new StudentInfo
             {
                 FirstName = "Nitin",
                 LastName = "Kale",
                 Email = "a@example.com",
                 DateBirth = DateTime.UtcNow.AddYears(-20),
                 Gender = "Male",
                 Address = "Addr",
                 City = "C",
                 PhoneNumber = "0123456789",
                 Stream = "Science",
                 YearOfStudy = "2"
             },
             new StudentInfo
             {
                 FirstName = "Pankaj",
                 LastName = "Naik",
                 Email = "a@example.com",
                 DateBirth = DateTime.UtcNow.AddYears(-20),
                 Gender = "Female",
                 Address = "Addr",
                 City = "C",
                 PhoneNumber = "0123456789",
                 Stream = "Science",
                 YearOfStudy = "2"
             }
           );
            context.SaveChanges();

            var controller = new StudentController(context,);

            var result = controller.Index() as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("StudentList"));

            var model = result.Model as IEnumerable<StudentInfo>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.Create() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Null);
        }

        [Test]
        public void Create_Post_ValidModel_AddsStudentAndRedirects()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var controller = new StudentController(context);

            var student = new StudentInfo { FirstName = "New", LastName = "Student", DateBirth = DateTime.Today, Gender = "M", Address = "A", City = "C", Email = "n@e.com", PhoneNumber = "1111111111", Stream = "S", YearOfStudy = "1" };

            var result = controller.Create(student) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(context.StudentsInfo.Count(), Is.EqualTo(1));
            var added = context.StudentsInfo.First();
            Assert.That(added.FirstName, Is.EqualTo("New"));
        }

        [Test]
        public void Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);
            controller.ModelState.AddModelError("FirstName", "Required");

            var student = new StudentInfo();

            var result = controller.Create(student) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.SameAs(student));
        }

        [Test]
        public void Edit_Get_NotFound_RedirectsToIndex()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.Edit(100) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public void Edit_Get_Found_ReturnsViewWithModel()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var student = new StudentInfo { FirstName = "E", LastName = "L", DateBirth = DateTime.Today, Gender = "M", Address = "A", City = "C", Email = "e@e.com", PhoneNumber = "1234567890", Stream = "S", YearOfStudy = "1" };
            context.StudentsInfo.Add(student);
            context.SaveChanges();

            var controller = new StudentController(context);
            var result = controller.Edit(student.StudentId) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((StudentInfo)result.Model).StudentId, Is.EqualTo(student.StudentId));
        }

        [Test]
        public void Edit_Post_Valid_UpdatesAndRedirects()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var student = new StudentInfo { FirstName = "Before", LastName = "Name", DateBirth = DateTime.Today, Gender = "M", Address = "A", City = "C", Email = "b@b.com", PhoneNumber = "2222222222", Stream = "S", YearOfStudy = "1" };
            context.StudentsInfo.Add(student);
            context.SaveChanges();

            var controller = new StudentController(context);

            student.FirstName = "After";

            var result = controller.Edit(student) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            var updated = context.StudentsInfo.Find(student.StudentId);
            Assert.That(updated.FirstName, Is.EqualTo("After"));
            Assert.That(updated.UpdateDate, Is.Not.Null);
        }

        [Test]
        public void Details_NotFound_RedirectsToIndex()
        {
            using var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new StudentController(context);

            var result = controller.Details(999) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public void Details_Found_ReturnsViewWithModel()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var student = new StudentInfo { FirstName = "D", LastName = "L", DateBirth = DateTime.Today, Gender = "M", Address = "A", City = "C", Email = "d@d.com", PhoneNumber = "3333333333", Stream = "S", YearOfStudy = "1" };
            context.StudentsInfo.Add(student);
            context.SaveChanges();

            var controller = new StudentController(context);
            var result = controller.Details(student.StudentId) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((StudentInfo)result.Model).StudentId, Is.EqualTo(student.StudentId));
        }

        [Test]
        public void DeleteConfirmed_RemovesStudentAndRedirects()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var student = new StudentInfo { FirstName = "T", LastName = "R", DateBirth = DateTime.Today, Gender = "M", Address = "A", City = "C", Email = "t@t.com", PhoneNumber = "4444444444", Stream = "S", YearOfStudy = "1" };
            context.StudentsInfo.Add(student);
            context.SaveChanges();

            var controller = new StudentController(context);
            var result = controller.DeleteConfirmed(student.StudentId) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(context.StudentsInfo.Count(), Is.EqualTo(0));
        }
    }
}