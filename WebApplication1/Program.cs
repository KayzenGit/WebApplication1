using System;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using Azure.Core;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<MyDB>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("DB_CNN_PATH")));

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                                                 .AllowAnyMethod()
                                                 .AllowAnyHeader()
               );
});

var app = builder.Build();

app.MapGet("/", () => "This is WebAPI For Test Project")
    .WithName("Default Get");

app.MapGet("/Salary", async (MyDB _db) => {
    return await _db.Salaries.Select(r => r).ToListAsync();
});
app.MapGet("/Employee", async (MyDB _db) =>
{
    return await _db.Employees.Select(r => r).ToListAsync();
});

app.MapGet("/Salary/{Id}", async (Int64 Id, MyDB _db) => {
    return await _db.Salaries.FindAsync(Id) is Salary salary ?
             Results.Ok(salary) :
             Results.NotFound("There is Not Any Salary !!");
});
app.MapGet("/Employee/{Id}", async (Int64 Id, MyDB _db) => {
    return await _db.Employees.FindAsync(Id) is Employee employee ?
             Results.Ok(employee) :
             Results.NotFound("There is Not Any Employee !!");
});

app.MapPost("/Employee", async (Employee[] employees, MyDB _db) =>
{
    await _db.AddRangeAsync(employees);
    await _db.SaveChangesAsync();
    return Results.Ok(employees);
    //return TypedResults.Created($"{employee.EmployeeID}", employee);
});
app.MapPost("/Salary", async (Salary[] salaries, MyDB _db) =>
{
    await _db.AddRangeAsync(salaries);
    await _db.SaveChangesAsync();
    return Results.Ok(salaries);
    //return TypedResults.Created($"{employee.EmployeeID}", employee);
});

app.MapDelete("/Employee/{id}", async (Int64 Id, MyDB _db) =>
{
    if (await _db.Employees.FindAsync(Id) is Employee employee)
    {
        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
        return Results.Ok($"Employees ID={Id} For Delete Has StatusCodes:" + StatusCodes.Status200OK);
    }
    return Results.NotFound($"Employees ID={Id} For Delete Has StatusCodes:" + StatusCodes.Status404NotFound);
});
app.MapDelete("/Salary/{id}", async (Int64 Id, MyDB _db) =>
{
    if (await _db.Salaries.FindAsync(Id) is Salary salary)
    {
        _db.Salaries.Remove(salary);
        await _db.SaveChangesAsync();
        return Results.Ok($"Salary ID={Id} For Delete Has StatusCodes:" + StatusCodes.Status200OK);
    }
    return Results.NotFound($"Salary ID={Id} For Delete Has StatusCodes:" + StatusCodes.Status404NotFound);
});

app.MapPut("/Salary", async (Salary salary, MyDB _db) =>
{
    var _salary = await _db.Salaries.SingleOrDefaultAsync(x => x.SalaryID == salary.SalaryID);
    if (_salary is not null)
    {
        _salary.SalaryAmount = salary.SalaryAmount;
        _salary.MonthNumber = salary.MonthNumber;
        return await _db.SaveChangesAsync() >= 0 ? 
            Results.Ok($"Update For Salary With ID={_salary.SalaryID} : Done Successfully" ) :
            Results.Content($"Update For Salary With ID={_salary.SalaryID} : Failed To Complete Successfully");
    }
    return Results.NotFound($"Salary ID={_salary.SalaryID} For Update  Status404NotFound ");
});
app.MapPut("/Employee", async (Employee employee, MyDB _db) =>
{
    var _employee = await _db.Employees.SingleOrDefaultAsync(x => x.EmployeeID == employee.EmployeeID);
    if (_employee is not null)
    {
        _employee.EmployeeFirstName = employee.EmployeeFirstName;
        _employee.EmployeeLastName = employee.EmployeeLastName;
        return await _db.SaveChangesAsync() >= 0 ?
            Results.Ok($"Update For Employee With ID={_employee.EmployeeID} : Done Successfully") :
            Results.Content($"Update For Employee With ID={_employee.EmployeeID} : Failed To Complete Successfully");
    }
    return Results.NotFound($"Employee ID={_employee.EmployeeID} For Update  Status404NotFound ");
});

app.Run();



