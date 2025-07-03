using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BibliotecaDbContext>();

var app = builder.Build();

// 1 - POST: Criar LIVRO
app.MapPost("/api/livros", ([FromBody] Livro livro, [FromServices] BibliotecaDbContext ctx) => {
    var categoria = ctx.Categoria.Find(livro.CategoriaId);
    if (categoria == null) {
        return Results.BadRequest("Status nulo");
    }

    livro.Categoria = categoria;

    if (livro == null || livro.Categoria.Length < 3) {
        return Results.BadRequest("Os requisitos para criar o livro não foram atendidos.");
    }

    ctx.Livros.Add(livro);
    ctx.SaveChanges();
    return Results.Created("", livro);
});

// 2 - GET: Listar todos Livros
app.MapGet("/api/livros", ([FromServices] BibliotecaDbContext ctx) => {
    var livros = ctx.Livros.Include(t => t.Categoria).ToList();
    if(livros.Any()){
        return Results.Ok(livros);
    }
    return Results.NotFound();
});

// 3 - GET: Buscar TAREFA por ID
app.MapGet("/api/livros/{id}", ([FromRoute] int id, [FromServices] BibliotecaDbContext ctx) => {
    Livro? livro = ctx.Livros.Include(t => t.Categoria).FirstOrDefault(t => t.Id == id);

    if (livro != null) {
        return Results.Ok(livro);
    }

    return Results.NotFound();
});

app.Run();