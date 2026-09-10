var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// routes
app.MapGet("/", () => "Hello World!"); // get

app.MapPost("/post", () => "Post"); // post

app.MapPut("/put", () => "Edit the file"); // put (edit)

app.MapDelete("/delete", () => "Deleted"); // delete

app.Run();
