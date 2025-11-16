using Csharp3_A3.Models;
using Csharp3_A3.Models.Enums;
using Csharp3_A3.Services;
using Csharp3_A3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Csharp3_A3.Controllers
{
	[Authorize(Roles = "Staff, Patient")]
	public class AppointmentsController : Controller
	{
		//Services
		private readonly AccountService _accountService;
		private readonly AppointmentService _appointmentService;
		private readonly PatientService _patientService;
		private readonly StaffService _staffService;
		private readonly UserService _userService;

		public AppointmentsController(AccountService accountService, AppointmentService appointmentService, PatientService patientService, StaffService staffService, UserService userService)
		{
			_accountService = accountService;
			_appointmentService = appointmentService;
			_patientService = patientService;
			_staffService = staffService;
			_userService = userService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var username = User.Identity?.Name;

			if (username == null)
				return Forbid();

			var user = await _accountService.GetUserWithRoleByUsernameAsync(username);

			if (user == null)
				return Forbid();

			var model = new AppointmentsViewModel();

			if (User.IsInRole("Staff"))
			{
				model.Staff = await _staffService.GetByIdAsync(user.StaffId!.Value);
				model.Appointments = await _appointmentService.GetAppointmentsByStaffIdAsync(model.Staff!.Id);
			}
			else if (User.IsInRole("Patient"))
			{
				model.Patient = await _userService.GetPatientByUserAsync(user);
				model.Appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(model.Patient!.Id);
			}
			else 
				return Forbid();

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> EditAppointment(int id)
		{
			var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

			if (appointment == null)
				return NotFound();
			else
			{
				var viewModel = new EditAppointmentViewModel()
				{
					Appointment = appointment
				};

				return View(viewModel);
			}
		}

		[HttpGet]
		public async Task<IActionResult> AddAppointment()
		{
			var username = User.Identity?.Name;
			if (username == null)
				return Forbid();

			var currentUser = await _accountService.GetUserWithRoleByUsernameAsync(username);
			if (currentUser == null)
				return Forbid();

			var patientsList = await _patientService.GetAllAsync();
			var staffList = await _staffService.GetAllAsync();

			Patient? patient = null;
			Staff? staff = null;

			if (User.IsInRole("Patient"))
			{
				patient = await _userService.GetPatientByUserAsync(currentUser);
			}
			else if (User.IsInRole("Staff"))
			{
				staff = await _userService.GetStaffByUserAsync(currentUser);
			}

			var viewModel = new AddAppointmentViewModel()
			{
				Patient = patient,
				Staff = staff,
				SelectPatients = new SelectList(patientsList, "Id", "Name"),
				SelectStaff = new SelectList(staffList, "Id", "Name")
			};

			//ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(AppointmentStatus)));

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditAppointment(EditAppointmentViewModel model)
		{
			if (!ModelState.IsValid)
			{
				// Repopulate StatusList before returning view
				if (model.StatusList == null || !model.StatusList.Any())
				{
					model.StatusList = Enum.GetValues(typeof(AppointmentStatus))
						.Cast<AppointmentStatus>()
						.Select(s => new SelectListItem
						{
							Value = ((int)s).ToString(),
							Text = s.ToString()
						});
				}

				return View(model);
			}

			var itemToUpdate = await _appointmentService.GetAppointmentByIdAsync(model.Appointment.Id);
			if (itemToUpdate == null)
				return NotFound();

			itemToUpdate.PatientId = model.Appointment.PatientId;
			itemToUpdate.StaffId = model.Appointment.StaffId;
			itemToUpdate.DateOfAppointment = model.Appointment.DateOfAppointment;
			itemToUpdate.Reason = model.Appointment.Reason;
			
			if (User.IsInRole("Staff"))
			{
				itemToUpdate.Status = model.Appointment.Status;
			}

			await _appointmentService.UpdateAsync(itemToUpdate);
			return RedirectToAction("Index", "Appointments");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
			if (appointment == null)
				return NotFound();

			await _appointmentService.DeleteAppointmentByIdAsync(id);
			return RedirectToAction("Index", "Appointments");
		}

		[HttpPost]
		public async Task<IActionResult> AddAppointment(AddAppointmentViewModel model)
		{
			var username = User.Identity?.Name;
			if (username == null)
				return Forbid();

			var currentUser = await _accountService.GetUserWithRoleByUsernameAsync(username);
			if (currentUser == null)
				return Forbid();

			if (currentUser.PatientId != null)
				model.Appointment.PatientId = (int)currentUser.PatientId;
			else if (currentUser.StaffId != null)
				model.Appointment.StaffId = (int)currentUser.StaffId;
			
			if (!ModelState.IsValid)
			{
				model.Patient = _patientService.GetByIdAsync(model.Appointment.PatientId).Result;
				model.Staff = _staffService.GetByIdAsync(model.Appointment.StaffId).Result;
				model.SelectPatients = new SelectList(await _patientService.GetAllAsync(), "Id", "Name");
				model.SelectStaff = new SelectList(await _staffService.GetAllAsync(), "Id", "Name");
				model.StatusList = new SelectList(Enum.GetValues(typeof(AppointmentStatus)));
				return View(model);
			}
			
			var staffAppointments = await _appointmentService.GetAppointmentsByStaffIdAsync(model.Appointment.StaffId);
			bool dateConflict = staffAppointments.Any(a => a.DateOfAppointment == model.Appointment.DateOfAppointment);

			if (dateConflict)
			{
				ModelState.AddModelError("", "The selected staff member already has an appointment at the chosen date and time. Please select a different time.");
				return View(model);
			}
			
			await _appointmentService.AddAppointmentAsync(model.Appointment);

			return RedirectToAction("Index", "Appointments");
		}
	}
}
