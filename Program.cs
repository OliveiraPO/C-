using Program.Models;
using Program.Data;


class Helloweb
{
    static void Main(string[] args)
    {
        var origins = "_origins";

        //Estrutura basica de uma aplicação cliente servidor ***
        //Configurar uma aplicação cliente servidor
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "Pedidos";
            config.Title = "Pedidos.API";
            config.Version = "v1";
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: origins,
            policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
        //cria a aplicação que a gente configurou em cima "new WebAplication"
        	
        builder.Services.AddDbContext<AppDbContext>();
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi(config =>
            {
                config.DocumentTitle = "Pedidos API";
                config.Path = "/swagger";
                config.DocumentPath = "/swagger/{documentName}/swagger.json";
                config.DocExpansion = "list";
            });
        }
        app.UseCors(origins);
        //Retorna a lista
    
        app.MapGet("/Pedidos", (AppDbContext context) =>
        { //
            return Results.Ok(context.Pedidos);
        }).Produces<Pedido>();
        //Retorna pelo Id

        app.MapGet("/Pedidos/{id}", ( AppDbContext context,  Guid id) => 
        {
            foreach (Pedido pedido in context.Pedidos)
            {
                if (pedido.Id == id)
                {
                    return Results.Ok(pedido);
                }
            }
            return Results.NotFound();
        }).Produces<Pedido>();

        //Retorna pedidos dos restaurantes
        app.MapGet("/PedidosRestaurante/{Restaurante}", (AppDbContext context, string Restaurante) =>
        {
            var listapedidos = context.Pedidos.Where(pedido => pedido.Restaurante == Restaurante).ToList();

            if (listapedidos.Count == 0)
            {
                return Results.NotFound("Nenhum pedido encontrado para o restaurante especificado.");
            }
            else
            {
                return Results.Ok(listapedidos);
            }
        }).Produces<Pedido>();
        app.MapPost("/Pedidos", (AppDbContext context, string Restaurante, string Prato, int Quantidade) =>
        {
            var pedido = new Pedido(Guid.NewGuid(), Restaurante, Prato, Quantidade);
            context.Pedidos.Add(pedido);
            context.SaveChanges();
            return Results.Ok(context.Pedidos);
        }).Produces<Pedido>();
        app.MapDelete("/PedidosDelete/{id}", (AppDbContext context, Guid id) =>
        {
            foreach (Pedido pedido in context.Pedidos)
            {
                if (pedido.Id == id)
                {
                    context.Pedidos.Remove(pedido);
                    context.SaveChanges();
                    return Results.Ok(pedido);
                }
            }
            return Results.NotFound();

        }).Produces<Pedido>();
        
        //Atualiza
        app.MapPut("/PedidosAtualiza/{id}", (AppDbContext context, Guid id, string restaurante, string prato, int quantidade) =>
        {
            foreach (Pedido pedido in context.Pedidos)
            {
                if (pedido.Id == id)
                {
                    pedido.Restaurante = restaurante;
                    pedido.Prato = prato;
                    pedido.Quantidade = quantidade;
                    context.SaveChanges();
                    return Results.Ok(pedido);
                }
            }
            return Results.NotFound();
        }).Produces<Pedido>();
        app.MapPatch("/PedidosPatch/{id}", (AppDbContext context, Guid id, RestaurantePatchModel nome) =>
        {
            var pedidoAlterado= new Pedido(Guid.NewGuid(),"","",0);
            foreach (Pedido pedido in context.Pedidos)
            {
                if (pedido.Id == id)
                {
                    if (!string.IsNullOrEmpty(nome.restaurante))
                    {
                        pedido.Restaurante = nome.restaurante;
                        context.SaveChanges();
                        pedidoAlterado=pedido;

                    }else return Results.NoContent();
                }
            }return Results.Ok(pedidoAlterado);
        }).Produces<Pedido>();

        //aqui de fato rodamos a aplicação
        app.Run();
    }
}