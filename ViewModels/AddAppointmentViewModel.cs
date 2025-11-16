using Csharp3_A3.Models;
using Csharp3_A3.Models.Enums;
using Csharp3_A3.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Csharp3_A3.ViewModels
{
	public class AddAppointmentViewModel
	{
		public Patient? Patient { get; set; }
		public Staff? Staff { get; set; }
		public SelectList? SelectPatients { get; set; }
		public SelectList? SelectStaff { get; set; }
		public SelectList StatusList { get; set; } = new(Enum.GetValues(typeof(AppointmentStatus)));
		public Appointment Appointment { get; set; } = new();

		[Required(ErrorMessage = "A reason is required for every appointment")]
		[StringLength(200, ErrorMessage = "The reason cannot exceed 200 characters.")]
		public string Reason
		{
			get => Appointment.Reason; 
			set => Appointment.Reason = value;
		}

		[DataType(DataType.DateTime)]
		[FutureDateTime]
		public DateTime DateOfAppointment
		{
			get => Appointment.DateOfAppointment;
			set => Appointment.DateOfAppointment = value;
		}
	}
}
