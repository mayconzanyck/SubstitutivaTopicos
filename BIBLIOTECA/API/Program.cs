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
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livro.Categoria = categoria;

    if (livro == null || livro.Titulo.Length < 3) {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }

    livro.Titulo = titulo;

    if (livro == null || livro.Autor.Length < 3) {
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");
    }

    livro.Autor = autor;

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

    return Results.NotFound("Livro com ID {id} não encontrado.");
});

// 4 - PUT: Atualizar Livro
app.MapPut("/api/livros/{id}", ([FromRoute] int id, [FromBody] Livro livro, [FromServices] BibliotecaDbContext ctx) => {
    Livro? entidade = ctx.Livros.Include(t => t.Categoria).FirstOrDefault(t => t.Id == id);

    if (entidade == null) {
        return Results.NotFound("Livro com ID {id} não encontrado para atualização.");
    }

    var categoria = ctx.Categoria.Find(livro.CategoriaId);
    if (categoria == null) {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livro.Categoria = categoria;

    if (livro == null || livro.Titulo.Length < 3) {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }

    livro.Titulo = titulo;

    if (livro == null || livro.Autor.Length < 3) {
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");
    }

    livro.Autor = autor;

    ctx.Livros.Update(livro);
    ctx.SaveChanges();
    return Results.Ok(ctx.Livros.Include(t => t.Categoria).FirstOrDefault(t => t.Id == id));
});

// 5 - DELETE: Remover Livro
app.MapDelete("/api/livros/{id}", ([FromRoute] int id, [FromServices] BibliotecaDbContext ctx) => {
    Livro? livro = ctx.Livros.Find(id);
    if(livro == null){
        return Results.NotFound("Livro com ID {id} não encontrado para remoção.");
    }

    ctx.Livros.Remove(livro);
    ctx.SaveChanges();
    return Results.NoContent();
});

app.Run();