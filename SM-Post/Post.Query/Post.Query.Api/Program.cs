using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    Action<DbContextOptionsBuilder> configureDbContext = (o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
    builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
    builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));
    
    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<IQueryHandler, QueryHandler>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.handlers.EventHandler>();
    builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
    builder.Services.AddScoped<IEventConsumer, EventConsumer>();

    //register query handler methods
    var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
    var dispatcher = new QueryDispatcher();
    dispatcher.RegisterHAndler<FindAllPostQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHAndler<FindPostByIdQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHAndler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHAndler<FindPostsWithLikesQueries>(queryHandler.HandleAsync);
    dispatcher.RegisterHAndler<FindPostWithCommentsQuery>(queryHandler.HandleAsync);

    builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);

    builder.Services.AddControllers();

    builder.Services.AddHostedService<ConsumerHostedService>();
    //create database and tables from code
    var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
    dataContext.Database.EnsureCreated();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}

app.Run();