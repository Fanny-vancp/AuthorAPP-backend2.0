using Neo4j.Driver;
using UniverseCreation.API.Adapter.Out.DataAccess;
using UniverseCreation.API.Adapter.Out.Repository;
using UniverseCreation.API;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers(Options =>
{
    Options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails(Options =>
{
    Options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
    };
});

// Database Graph
// Register application setting using IOption provider mechanism
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

// Fetch settings object from configuration
var settings = new ApplicationSettings();
builder.Configuration.GetSection("ApplicationSettings").Bind(settings);

// Register the neo4j Driver Object as a singleton
builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));

// Data Access Wrapper over Neoj4 session, that is a helper class for executing parameterized Neo4j cypher queries in Transactions
builder.Services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();

// Registration for the domain repository class
builder.Services.AddTransient<ICharacterRepository, CharacterRepository>();

// For the front
// -> TO DO add security
// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();
//app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();