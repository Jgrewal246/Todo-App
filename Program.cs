using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoService>(new InMemoryTodoService());

var app = builder.Build();


// <Middleware>
// Middleware runs before and after requests and can be used on all requests
// matches regex to $1
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// Creating our own middleware for logging HTTP Requests
// app.Use expects a HTTPContext and RequestDelegate
app.Use(async (context, next) => {
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started");
    // Call the next middleware
    await next(context);
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished");
});
// <Middleware>




// Get all todos
app.MapGet("/todos", (ITodoService service) => service.GetTodos());


// Get todo by id
app.MapGet("/todos/{id}", Results<Ok<ToDo>, NotFound> (int id, ITodoService service) => {
    var targetTodo = service.GetTodoById(id);
    return targetTodo is not null
    ? TypedResults.Ok(targetTodo)
    : TypedResults.NotFound();
});


// Add todo 
app.MapPost("/todos", (ToDo todo, ITodoService service) => {
    service.AddTodo(todo);
    return TypedResults.Created("/todos{id}", todo);
}).AddEndpointFilter( async (context, next) => {

    var todo = context.GetArgument<ToDo>(0);
    var errors = new Dictionary<string, string[]>();

    if (todo.IsCompleted){
        errors.Add(nameof(ToDo.IsCompleted), ["Cannot add completed todo"]);
    }

    if (todo.DueDate < DateTime.UtcNow){
        errors.Add(nameof(ToDo.DueDate), ["Cannot add a task that is past it's due date"]);
    }

    if (errors.Count > 0){
        return Results.ValidationProblem(errors);
    }
    return await next(context);

});


// Delete todo by id
app.MapDelete("/todos/{id}", (int id, ITodoService service) => {
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});



app.Run();

public record ToDo(int Id, string Name, DateTime DueDate, bool IsCompleted);

interface ITodoService{
    ToDo AddTodo(ToDo todo);

    void DeleteTodoById(int id);

    ToDo? GetTodoById(int id);

    List<ToDo> GetTodos();
}

class InMemoryTodoService : ITodoService{

    private readonly List<ToDo> _todos = [];
    public ToDo AddTodo(ToDo todo){
        _todos.Add(todo);
        return todo;
    }

    public void DeleteTodoById(int id){
        _todos.RemoveAll(t => t.Id == id);
    }

    public ToDo? GetTodoById(int id){
        //SingleOrDefault is a LINQ query method 
        var todo = _todos.SingleOrDefault(t => t.Id == id);
        return todo;
    }

    public List<ToDo> GetTodos(){
        return _todos;
    }

}
