using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Route("api/customers")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(String name, String phoneNumber)
    {
        //для маски ввода
        var problemDetailsPhoneNumber = new ProblemDetails
        {
            Status = 400, // Код состояния HTTP (например, 400 для ошибки запроса)
            Title = "Ошибка в запросе",
            Detail = "Укажите номер телефона в следующем формате: +996#########.",
            Instance = HttpContext.Request.Path,
        };

        //для уникальности номера телефона
        var problemDetailsDuplicatePhoneNubmer = new ProblemDetails
        {
            Status = 400, // Код состояния HTTP (например, 400 для ошибки запроса)
            Title = "Ошибка в запросе",
            Detail = "Такой номер уже зарегистрирован!",
            Instance = HttpContext.Request.Path,
        };

        //для проверки имени клиента
        var problemDetailsName = new ProblemDetails
        {
            Status = 400, // Код состояния HTTP (например, 400 для ошибки запроса)
            Title = "Ошибка в запросе",
            Detail = "Имя должна содержать только буквы!",
            Instance = HttpContext.Request.Path,
        };

        // Регулярное выражение для маски
        string patternOfPhoneNumber = @"^\+996\d{9}$";
        // Регулярное выражение для букв
        string patternOfName = @"^[A-Za-z]+$";

        //проверка имени
        if (!Regex.IsMatch(name, patternOfName))
        {
            return BadRequest(problemDetailsName); // Возврат ответа с кодом 400 и информацией об ошибке
        }
        //проверка маски ввода для номера телефона
        else if (Regex.IsMatch(phoneNumber, patternOfPhoneNumber))
        {
            var clients = _context.Customers.ToListAsync();
            foreach (var item in await clients)
            {
                //проверка дублирующих записей номера телефона
                if (phoneNumber == item.PhoneNumber)
                {
                    return BadRequest(problemDetailsDuplicatePhoneNubmer); // Возврат ответа с кодом 400 и информацией об ошибке
                }
            }

            Customer customer = new Customer();
            customer.Name = name;
            customer.PhoneNumber = phoneNumber;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(customer);
        }
        else
        {
            return BadRequest(problemDetailsPhoneNumber); // Возврат ответа с кодом 400 и информацией об ошибке
        }
    }
}
