using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.UseCases.Exceptions;
using AuthService.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Web.Controllers
{
	[ApiController]
	[Route("api/register")]
	public class UserContoller : ControllerBase
	{
		private readonly IUserService _userService;

		public UserContoller(IUserService userService)
		{
			_userService = userService;
		}

		//*Metodo para el registro de usuarios
		[Microsoft.AspNetCore.Mvc.HttpPost("/user")]
		public async Task<IActionResult> Register([Microsoft.AspNetCore.Mvc.FromBody] RegisterRequest dto)
		{
			try
			{
				//!Pasar esta validacion a useCases
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return BadRequest(new { error = "Datos invalidos", detail = errors });
				}
				var infoRegister = await _userService.RegisterUser(dto);
				return Ok(infoRegister);
			}
			catch (PhoneAlreadyExistException ex)
			{
				return BadRequest(new { error = ex.Message, detail = "El numero esta en uso por otra persona" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
			}
		}

		//*Metodo par el registro de conductores
		[Microsoft.AspNetCore.Mvc.HttpPost("/driver")]
		public async Task<IActionResult> RegisterDriver([Microsoft.AspNetCore.Mvc.FromBody] RegisterRequest dto)
		{
			try
			{
				//!Pasar esta validacion a useCases
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return BadRequest(new { error = "Datos invalidos", detail = errors });
				}
				var infoRegister = await _userService.RegisterDriver(dto);
				return Ok(infoRegister);
			}
			catch (PhoneAlreadyExistException ex)
			{
				return BadRequest(new { error = ex.Message, detail = "El numero esta en uso por otra persona" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
			}
		}

		[Microsoft.AspNetCore.Mvc.HttpPost("/login")]
		public async Task<IActionResult> Login([Microsoft.AspNetCore.Mvc.FromBody] LoginRequest dto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
					return BadRequest(new { error = "Datos invalidos", detail = errors });
				}
				Console.WriteLine("usuario" + dto.password);
				Console.WriteLine("usuario" + dto.Phone.Number);
				var infoLogin = await _userService.LoginUser(dto);
				return Ok(infoLogin);
			}
			catch (LoginException ex)
			{
				return BadRequest(new { error = "Usuario no encontrado", detail = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
			}

		}

	}
}