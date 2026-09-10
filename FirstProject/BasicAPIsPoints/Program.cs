var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/post", () => "Post");

app.MapPut("/put", () => "Edit the file");

app.MapDelete("/delete", () => "Deleted");

app.Run();
