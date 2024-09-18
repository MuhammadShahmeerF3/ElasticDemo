using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Configure Elasticsearch client
//var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
//    .DefaultIndex("students");
//var client = new ElasticClient(settings);

//builder.Services.AddSingleton<IElasticClient>(client);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSwaggerUI();
app.UseSwagger();
app.MapControllers();

app.Run();
