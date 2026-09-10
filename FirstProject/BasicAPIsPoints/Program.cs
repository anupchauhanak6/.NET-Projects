using BasicAPIsPoints.Controllers;

var builder = WebApplication.CreateBuilder(args);

// add services to the controller
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


app.Run();
