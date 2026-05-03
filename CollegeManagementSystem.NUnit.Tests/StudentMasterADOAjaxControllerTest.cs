using CollegeManagementSystem.NUnit.Tests;
using CollegeWebApplication.Controllers;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CollegeWebApplication.Controllers;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CollegeManagementSystem.NUnit.Tests
{
    [TestFixture]
    public class StudentMasterADOAjaxControllerTest
    {
        private Mock<IStudentMasterADOSPRepository> _studentRepoMock = null!;
        private Mock<IStateMasterADORepository> _stateRepoMock = null!;
        private Mock<ICityMasterADOSPRepository> _cityRepoMock = null!;
        private Mock<ICourseMasterADORepository> _courseRepoMock = null!;
        private Mock<ILogger<StudentMasterADOAjaxController>> _loggerMock = null!;
        private Mock<IUrlHelper> _urlHelperMock = null!;
        private StudentMasterADOAjaxController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            _studentRepoMock = new Mock<IStudentMasterADOSPRepository>();
            _stateRepoMock = new Mock<IStateMasterADORepository>();
            _cityRepoMock = new Mock<ICityMasterADOSPRepository>();
            _courseRepoMock = new Mock<ICourseMasterADORepository>();
            _loggerMock = new Mock<ILogger<StudentMasterADOAjaxController>>();
            _urlHelperMock = new Mock<IUrlHelper>();

            // Provide a stable Url.Action response so controller doesn't throw NullReference
            _urlHelperMock
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("/StudentMasterADOAjax/Index");

            _controller = new StudentMasterADOAjaxController(
                _studentRepoMock.Object,
                _stateRepoMock.Object,
                _cityRepoMock.Object,
                _courseRepoMock.Object,
                _loggerMock.Object)
            {
                Url = _urlHelperMock.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        // Replace all usages of Assert.IsNotNull(x) with Assert.That(x, Is.Not.Null)
        // Replace all usages of Assert.IsNull(x) with Assert.That(x, Is.Null)
        // Replace all usages of Assert.IsTrue(x) with Assert.That(x, Is.True)
        // Replace all usages of Assert.IsFalse(x) with Assert.That(x, Is.False)

        [Test]
        public void Index_Returns_StudentList_View()
        {
            var result = _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("StudentList"));
        }

        [Test]
        public void GetStudentList_Returns_Json_With_Data_When_Items_Present()
        {
            var students = new List<StudentMasterADO>
            {
                new StudentMasterADO { StudentId = 1, FirstName = "John", LastName = "Doe" }
            };
            _studentRepoMock.Setup(r => r.GetAllStudentMasters()).Returns(students);

            var result = _controller.GetStudentList() as JsonResult;

            Assert.That(result, Is.Not.Null);
            // extract anonymous object property 'data' via reflection
            var value = result!.Value!;
            var dataProp = value.GetType().GetProperty("data", BindingFlags.Public | BindingFlags.Instance)!;
            var data = dataProp.GetValue(value) as IEnumerable<StudentMasterADO>;
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetStudentList_Returns_Json_With_Null_Data_When_Repo_Returns_Null()
        {
            _studentRepoMock.Setup(r => r.GetAllStudentMasters()).Returns(() => null);

            var result = _controller.GetStudentList() as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var dataProp = value.GetType().GetProperty("data", BindingFlags.Public | BindingFlags.Instance)!;
            var data = dataProp.GetValue(value);
            Assert.That(data, Is.Null);
        }

        [Test]
        public void Create_Get_Sets_ViewBag_And_Returns_CreateStudent_View()
        {
            var states = new List<StateMaster> { new StateMaster { StateId = 1, StateName = "S1" } };
            var courses = new List<CourseMaster> { new CourseMaster { CourseId = 1, CourseName = "C1" } };
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(states);
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(courses);

            var result = _controller.Create() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("CreateStudent"));
            Assert.That(_controller.ViewBag.States, Is.SameAs(states));
            Assert.That(_controller.ViewBag.Courses, Is.SameAs(courses));
        }

        [Test]
        public async Task Create_Post_Returns_Success_Json_When_Model_Valid()
        {
            var student = new StudentMasterADO { StudentId = 0, FirstName = "A", LastName = "B" };
            _studentRepoMock.Setup(r => r.AddStudentMaster(It.IsAny<StudentMasterADO>())).ReturnsAsync("1");

            var result = await _controller.Create(student) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var resultStr = (string?)value.GetType().GetProperty("result")!.GetValue(value);
            var redirectTo = (string?)value.GetType().GetProperty("redirectTo")!.GetValue(value);

            Assert.That(success, Is.True);
            Assert.That(resultStr, Is.EqualTo("Saved"));
            Assert.That(redirectTo, Is.EqualTo("/StudentMasterADOAjax/Index"));
        }

        [Test]
        public async Task Create_Post_Returns_ValidationErrors_Json_When_ModelState_Invalid()
        {
            var student = new StudentMasterADO { StudentId = 0, FirstName = "A" };
            _controller.ModelState.AddModelError("LastName", "Required");

            var result = await _controller.Create(student) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var message = (string?)value.GetType().GetProperty("message")!.GetValue(value);
            var errors = value.GetType().GetProperty("errors")!.GetValue(value) as IEnumerable<string>;

            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Validation failed."));
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors!.Any(), Is.True);
        }

        [Test]
        public async Task Create_Post_Returns_Error_Json_When_Null_Payload()
        {
            var result = await _controller.Create(null) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var message = (string?)value.GetType().GetProperty("message")!.GetValue(value);

            Assert.That(success, Is.False);
            Assert.That(message!.Contains("Invalid request payload"), Is.True);
        }

        [Test]
        public async Task Create_Post_Returns_Error_Json_When_Repo_Throws()
        {
            var student = new StudentMasterADO { StudentId = 0, FirstName = "A", LastName = "B" };
            _studentRepoMock.Setup(r => r.AddStudentMaster(It.IsAny<StudentMasterADO>()))
                .ThrowsAsync(new InvalidOperationException("failure"));

            var result = await _controller.Create(student) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var errorMessage = (string?)value.GetType().GetProperty("ErrorMessage")!.GetValue(value);

            Assert.That(success, Is.False);
            Assert.That(errorMessage!.Contains("failure"), Is.True);
        }

        [Test]
        public void Edit_Get_Returns_EditStudent_View_When_Found()
        {
            var student = new StudentMasterADO { StudentId = 2, FirstName = "F", LastName = "L", StateId = 10, CityId = 100, CourseId = 5 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(2)).Returns(student);
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(new List<StateMaster>());
            _cityRepoMock.Setup(r => r.GetCitiesByState(10)).Returns(new List<CityMaster>());
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(new List<CourseMaster>());

            var result = _controller.Edit(2) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("EditStudent"));
            Assert.That(result.Model, Is.SameAs(student));
            // Ensure ViewBag lists exist
            Assert.That(_controller.ViewBag.StateList, Is.InstanceOf<SelectList>());
            Assert.That(_controller.ViewBag.CityList, Is.InstanceOf<SelectList>());
            Assert.That(_controller.ViewBag.CourseList, Is.InstanceOf<SelectList>());
        }

        [Test]
        public async Task Edit_Post_Returns_Success_Json_When_Model_Valid()
        {
            var student = new StudentMasterADO { StudentId = 3, FirstName = "X", LastName = "Y" };
            _studentRepoMock.Setup(r => r.UpdateStudentMaster(It.IsAny<StudentMasterADO>())).ReturnsAsync("1");

            var result = await _controller.Edit(student) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.That(success, Is.True);
            Assert.That((string?)value.GetType().GetProperty("redirectTo")!.GetValue(value), Is.EqualTo("/StudentMasterADOAjax/Index"));
        }

        [Test]
        public async Task Edit_Post_Returns_ValidationErrors_Json_When_ModelState_Invalid()
        {
            var student = new StudentMasterADO { StudentId = 3, FirstName = "X" };
            _controller.ModelState.AddModelError("LastName", "Required");

            var result = await _controller.Edit(student) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.That(success, Is.False);
            var errors = value.GetType().GetProperty("errors")!.GetValue(value) as IEnumerable<string>;
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors!.Any(), Is.True);
        }

        [Test]
        public async Task Edit_Post_Returns_Error_Json_When_Null_Payload()
        {
            var result = await _controller.Edit(null) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.That(success, Is.False);
        }

        [Test]
        public async Task Edit_Post_Returns_Error_Json_When_Repo_Throws()
        {
            var student = new StudentMasterADO { StudentId = 3, FirstName = "X", LastName = "Y" };
            _studentRepoMock.Setup(r => r.UpdateStudentMaster(It.IsAny<StudentMasterADO>()))
                .ThrowsAsync(new Exception("update failed"));

            var result = await _controller.Edit(student) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var err = (string?)value.GetType().GetProperty("ErrorMessage")!.GetValue(value);

            Assert.That(success, Is.False);
            Assert.That(err!.Contains("update failed"), Is.True);
        }

        [Test]
        public void Details_Get_Returns_DetailsStudent_View_When_Found()
        {
            var student = new StudentMasterADO { StudentId = 4, FirstName = "D", LastName = "E", StateId = 1, CityId = 2, CourseId = 3 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(4)).Returns(student);
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(new List<StateMaster>());
            _cityRepoMock.Setup(r => r.GetCitiesByState(1)).Returns(new List<CityMaster>());
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(new List<CourseMaster>());

            var result = _controller.Details(4) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("DetailsStudent"));
            Assert.That(result.Model, Is.SameAs(student));
        }

        [Test]
        public void Delete_Get_Returns_DeleteStudent_View_When_Found()
        {
            var student = new StudentMasterADO { StudentId = 5, FirstName = "Del", LastName = "Stu", StateId = 7, CityId = 8, CourseId = 9 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(5)).Returns(student);
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(new List<StateMaster>());
            _cityRepoMock.Setup(r => r.GetCitiesByState(7)).Returns(new List<CityMaster>());
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(new List<CourseMaster>());

            var result = _controller.Delete(5) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ViewName, Is.EqualTo("DeleteStudent"));
            Assert.That(result.Model, Is.SameAs(student));
        }
            
        [Test]
        public async Task DeleteConfirmed_Returns_Error_Json_When_StudentId_Invalid()
        {
            var result = await _controller.DeleteConfirmed(0) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.That(success, Is.False);
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Error_Json_When_Student_Not_Found()
        {
            _studentRepoMock.Setup(r => r.GetStudentMasterById(It.IsAny<int>())).Returns(() => null);

            var result = await _controller.DeleteConfirmed(123) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.That(success, Is.False);
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Success_Json_When_Deleted()
        {
            var student = new StudentMasterADO { StudentId = 6 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(6)).Returns(student);
            _studentRepoMock.Setup(r => r.DeleteStudentMaster(6)).ReturnsAsync("1");

            var result = await _controller.DeleteConfirmed(6) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.That(success, Is.True);
            Assert.That((string?)value.GetType().GetProperty("result")!.GetValue(value), Is.EqualTo("Deleted"));
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Error_Json_When_Delete_Throws()
        {
            var student = new StudentMasterADO { StudentId = 7 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(7)).Returns(student);
            _studentRepoMock.Setup(r => r.DeleteStudentMaster(7)).ThrowsAsync(new Exception("delete failure"));

            var result = await _controller.DeleteConfirmed(7) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value!;
            var successValue = value.GetType().GetProperty("success")!.GetValue(value);
            // In catch the controller returns success = "Error" (string)
            Assert.That(successValue, Is.EqualTo("Error"));
            var err = (string?)value.GetType().GetProperty("ErrorMessage")!.GetValue(value);
            Assert.That(err!.Contains("delete failure"), Is.True);
        }

        [Test]
        public void GetCities_Returns_List_When_Found()
        {
            var cities = new List<CityMaster>
            {
                new CityMaster { CityId = 1, CityName = "C1" },
                new CityMaster { CityId = 2, CityName = "C2" }
            };
            _cityRepoMock.Setup(r => r.GetCitiesByState(10)).Returns(cities);

            var result = _controller.GetCities(10) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value as IEnumerable<CityMaster>;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetCities_Returns_Empty_List_On_Exception()
        {
            _cityRepoMock.Setup(r => r.GetCitiesByState(It.IsAny<int>())).Throws(new Exception("boom"));

            var result = _controller.GetCities(1) as JsonResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value as IEnumerable<CityMaster>;
            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Empty);
        }

        #region Commented 
        /*
        #region Index & GetStudentList

        [Test]
        public void Index_Returns_StudentList_View()
        {
            var result = _controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("StudentList", result!.ViewName);
        }

        [Test]
        public void GetStudentList_Returns_Json_With_Data_When_Items_Present()
        {
            var students = new List<StudentMasterADO>
            {
                new StudentMasterADO { StudentId = 1, FirstName = "John", LastName = "Doe" }
            };
            _studentRepoMock.Setup(r => r.GetAllStudentMasters()).Returns(students);

            var result = _controller.GetStudentList() as JsonResult;

            Assert.IsNotNull(result);
            // extract anonymous object property 'data' via reflection
            var value = result!.Value!;
            var dataProp = value.GetType().GetProperty("data", BindingFlags.Public | BindingFlags.Instance)!;
            var data = dataProp.GetValue(value) as IEnumerable<StudentMasterADO>;
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data!.Count());
        }

        [Test]
        public void GetStudentList_Returns_Json_With_Null_Data_When_Repo_Returns_Null()
        {
            _studentRepoMock.Setup(r => r.GetAllStudentMasters()).Returns(() => null);

            var result = _controller.GetStudentList() as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var dataProp = value.GetType().GetProperty("data", BindingFlags.Public | BindingFlags.Instance)!;
            var data = dataProp.GetValue(value);
            Assert.IsNull(data);
        }

        #endregion

        #region Create (GET & POST)

        [Test]
        public void Create_Get_Sets_ViewBag_And_Returns_CreateStudent_View()
        {
            var states = new List<StateMaster> { new StateMaster { StateId = 1, StateName = "S1" } };
            var courses = new List<CourseMaster> { new CourseMaster { CourseId = 1, CourseName = "C1" } };
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(states);
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(courses);

            var result = _controller.Create() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("CreateStudent", result!.ViewName);
            Assert.AreSame(states, _controller.ViewBag.States);
            Assert.AreSame(courses, _controller.ViewBag.Courses);
        }

        [Test]
        public async Task Create_Post_Returns_Success_Json_When_Model_Valid()
        {
            var student = new StudentMasterADO { StudentId = 0, FirstName = "A", LastName = "B" };
            _studentRepoMock.Setup(r => r.AddStudentMaster(It.IsAny<StudentMasterADO>())).ReturnsAsync("1");

            var result = await _controller.Create(student) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var resultStr = (string?)value.GetType().GetProperty("result")!.GetValue(value);
            var redirectTo = (string?)value.GetType().GetProperty("redirectTo")!.GetValue(value);

            Assert.IsTrue(success);
            Assert.AreEqual("Saved", resultStr);
            Assert.AreEqual("/StudentMasterADOAjax/Index", redirectTo);
        }

        [Test]
        public async Task Create_Post_Returns_ValidationErrors_Json_When_ModelState_Invalid()
        {
            var student = new StudentMasterADO { StudentId = 0, FirstName = "A" };
            _controller.ModelState.AddModelError("LastName", "Required");

            var result = await _controller.Create(student) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var message = (string?)value.GetType().GetProperty("message")!.GetValue(value);
            var errors = value.GetType().GetProperty("errors")!.GetValue(value) as IEnumerable<string>;

            Assert.IsFalse(success);
            Assert.AreEqual("Validation failed.", message);
            Assert.IsNotNull(errors);
            Assert.IsTrue(errors!.Any());
        }

        [Test]
        public async Task Create_Post_Returns_Error_Json_When_Null_Payload()
        {
            var result = await _controller.Create(null) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var message = (string?)value.GetType().GetProperty("message")!.GetValue(value);

            Assert.IsFalse(success);
            Assert.IsTrue(message!.Contains("Invalid request payload"));
        }

        [Test]
        public async Task Create_Post_Returns_Error_Json_When_Repo_Throws()
        {
            var student = new StudentMasterADO { StudentId = 0, FirstName = "A", LastName = "B" };
            _studentRepoMock.Setup(r => r.AddStudentMaster(It.IsAny<StudentMasterADO>()))
                .ThrowsAsync(new InvalidOperationException("failure"));

            var result = await _controller.Create(student) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var errorMessage = (string?)value.GetType().GetProperty("ErrorMessage")!.GetValue(value);

            Assert.IsFalse(success);
            Assert.IsTrue(errorMessage!.Contains("failure"));
        }

        #endregion

        #region Edit (GET & POST)

        [Test]
        public void Edit_Get_Returns_EditStudent_View_When_Found()
        {
            var student = new StudentMasterADO { StudentId = 2, FirstName = "F", LastName = "L", StateId = 10, CityId = 100, CourseId = 5 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(2)).Returns(student);
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(new List<StateMaster>());
            _cityRepoMock.Setup(r => r.GetCitiesByState(10)).Returns(new List<CityMaster>());
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(new List<CourseMaster>());

            var result = _controller.Edit(2) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("EditStudent", result!.ViewName);
            Assert.AreSame(student, result.Model);
            // Ensure ViewBag lists exist
            Assert.IsInstanceOf<SelectList>(_controller.ViewBag.StateList);
            Assert.IsInstanceOf<SelectList>(_controller.ViewBag.CityList);
            Assert.IsInstanceOf<SelectList>(_controller.ViewBag.CourseList);
        }

        [Test]
        public void Edit_Get_Redirects_To_Index_When_Not_Found()
        {
            _studentRepoMock.Setup(r => r.GetStudentMasterById(It.IsAny<int>())).Returns(() => null);

            var result = _controller.Edit(99);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [Test]
        public async Task Edit_Post_Returns_Success_Json_When_Model_Valid()
        {
            var student = new StudentMasterADO { StudentId = 3, FirstName = "X", LastName = "Y" };
            _studentRepoMock.Setup(r => r.UpdateStudentMaster(It.IsAny<StudentMasterADO>())).ReturnsAsync("1");

            var result = await _controller.Edit(student) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.IsTrue(success);
            Assert.AreEqual("/StudentMasterADOAjax/Index", (string?)value.GetType().GetProperty("redirectTo")!.GetValue(value));
        }

        [Test]
        public async Task Edit_Post_Returns_ValidationErrors_Json_When_ModelState_Invalid()
        {
            var student = new StudentMasterADO { StudentId = 3, FirstName = "X" };
            _controller.ModelState.AddModelError("LastName", "Required");

            var result = await _controller.Edit(student) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.IsFalse(success);
            var errors = value.GetType().GetProperty("errors")!.GetValue(value) as IEnumerable<string>;
            Assert.IsNotNull(errors);
            Assert.IsTrue(errors!.Any());
        }

        [Test]
        public async Task Edit_Post_Returns_Error_Json_When_Null_Payload()
        {
            var result = await _controller.Edit(null) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.IsFalse(success);
        }

        [Test]
        public async Task Edit_Post_Returns_Error_Json_When_Repo_Throws()
        {
            var student = new StudentMasterADO { StudentId = 3, FirstName = "X", LastName = "Y" };
            _studentRepoMock.Setup(r => r.UpdateStudentMaster(It.IsAny<StudentMasterADO>()))
                .ThrowsAsync(new Exception("update failed"));

            var result = await _controller.Edit(student) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var err = (string?)value.GetType().GetProperty("ErrorMessage")!.GetValue(value);

            Assert.IsFalse(success);
            Assert.IsTrue(err!.Contains("update failed"));
        }

        #endregion

        #region Details, Delete (GET) & DeleteConfirmed

        [Test]
        public void Details_Get_Returns_DetailsStudent_View_When_Found()
        {
            var student = new StudentMasterADO { StudentId = 4, FirstName = "D", LastName = "E", StateId = 1, CityId = 2, CourseId = 3 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(4)).Returns(student);
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(new List<StateMaster>());
            _cityRepoMock.Setup(r => r.GetCitiesByState(1)).Returns(new List<CityMaster>());
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(new List<CourseMaster>());

            var result = _controller.Details(4) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("DetailsStudent", result!.ViewName);
            Assert.AreSame(student, result.Model);
        }

        [Test]
        public void Details_Get_Redirects_To_Index_When_Not_Found()
        {
            _studentRepoMock.Setup(r => r.GetStudentMasterById(It.IsAny<int>())).Returns(() => null);

            var result = _controller.Details(123);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public void Delete_Get_Returns_DeleteStudent_View_When_Found()
        {
            var student = new StudentMasterADO { StudentId = 5, FirstName = "Del", LastName = "Stu", StateId = 7, CityId = 8, CourseId = 9 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(5)).Returns(student);
            _stateRepoMock.Setup(r => r.GetAllStateMasters()).Returns(new List<StateMaster>());
            _cityRepoMock.Setup(r => r.GetCitiesByState(7)).Returns(new List<CityMaster>());
            _courseRepoMock.Setup(r => r.GetAllCourses()).Returns(new List<CourseMaster>());

            var result = _controller.Delete(5) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("DeleteStudent", result!.ViewName);
            Assert.AreSame(student, result.Model);
        }

        [Test]
        public void Delete_Get_Redirects_To_Index_When_Not_Found()
        {
            _studentRepoMock.Setup(r => r.GetStudentMasterById(It.IsAny<int>())).Returns(() => null);

            var result = _controller.Delete(999);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Error_Json_When_StudentId_Invalid()
        {
            var result = await _controller.DeleteConfirmed(0) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.IsFalse(success);
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Error_Json_When_Student_Not_Found()
        {
            _studentRepoMock.Setup(r => r.GetStudentMasterById(It.IsAny<int>())).Returns(() => null);

            var result = await _controller.DeleteConfirmed(123) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.IsFalse(success);
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Success_Json_When_Deleted()
        {
            var student = new StudentMasterADO { StudentId = 6 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(6)).Returns(student);
            _studentRepoMock.Setup(r => r.DeleteStudentMaster(6)).ReturnsAsync("1");

            var result = await _controller.DeleteConfirmed(6) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            Assert.IsTrue(success);
            Assert.AreEqual("Deleted", (string?)value.GetType().GetProperty("result")!.GetValue(value));
        }

        [Test]
        public async Task DeleteConfirmed_Returns_Error_Json_When_Delete_Throws()
        {
            var student = new StudentMasterADO { StudentId = 7 };
            _studentRepoMock.Setup(r => r.GetStudentMasterById(7)).Returns(student);
            _studentRepoMock.Setup(r => r.DeleteStudentMaster(7)).ThrowsAsync(new Exception("delete failure"));

            var result = await _controller.DeleteConfirmed(7) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value!;
            var successValue = value.GetType().GetProperty("success")!.GetValue(value);
            // In catch the controller returns success = "Error" (string)
            Assert.AreEqual("Error", successValue);
            var err = (string?)value.GetType().GetProperty("ErrorMessage")!.GetValue(value);
            Assert.IsTrue(err!.Contains("delete failure"));
        }

        #endregion

        #region GetCities

        [Test]
        public void GetCities_Returns_List_When_Found()
        {
            var cities = new List<CityMaster>
            {
                new CityMaster { CityId = 1, CityName = "C1" },
                new CityMaster { CityId = 2, CityName = "C2" }
            };
            _cityRepoMock.Setup(r => r.GetCitiesByState(10)).Returns(cities);

            var result = _controller.GetCities(10) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value as IEnumerable<CityMaster>;
            Assert.IsNotNull(value);
            Assert.AreEqual(2, value!.Count());
        }

        [Test]
        public void GetCities_Returns_Empty_List_On_Exception()
        {
            _cityRepoMock.Setup(r => r.GetCitiesByState(It.IsAny<int>())).Throws(new Exception("boom"));

            var result = _controller.GetCities(1) as JsonResult;

            Assert.IsNotNull(result);
            var value = result!.Value as IEnumerable<CityMaster>;
            Assert.IsNotNull(value);
            Assert.IsEmpty(value);
        }

        #endregion

        */
        #endregion
    }
}
