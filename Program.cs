using libraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using libraryManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add EF Core with SQLite
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register BookService with DI, passing the books.json path
builder.Services.AddScoped<BookService>(provider =>
    new BookService("Data/books.json"));
// Register MemberService with DI, passing the members.xml path
builder.Services.AddScoped<MemberService>(provider =>
    new MemberService("Data/members.xml"));
// Register BorrowService with DI, passing the borrow.sql and logs.txt paths, and injecting BookService and MemberService
builder.Services.AddScoped<BorrowService>(provider =>
    new BorrowService(
        "Data/borrow.sql",
        "Data/logs.txt",
        provider.GetRequiredService<BookService>(),
        provider.GetRequiredService<MemberService>()
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
