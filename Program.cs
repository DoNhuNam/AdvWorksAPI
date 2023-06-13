using AdvWorksAPI.ConstantClasses;
using AdvWorksAPI.EntityLayer;
using AdvWorksAPI.ExtensionClasses;

// **********************************************
// Create a WebApplicationBuilder object
// to configure the how the ASP.NET service runs
// **********************************************
var builder = WebApplication.CreateBuilder(args);

// **********************************************
// Add and Configure Services
// **********************************************
// Add & Configure Global Application Settings
builder.ConfigureGlobalSettings();

// Add & Configure AdventureWorksLT DbContext
builder.Services.ConfigureAdventureWorksDB(
  builder.Configuration.GetConnectionString("DefaultConnection"));

// Add & Configure Repository Classes
builder.Services.AddRepositoryClasses();

// Add & Configure CORS
builder.Services.ConfigureCors();

// Add & Configure Logging using Serilog
builder.Host.ConfigureSeriLog();

// Add & Configure JWT Authentication
builder.Services.ConfigureJwtAuthentication(
  builder.Configuration.GetRequiredSection("AdvWorksAPI").Get<AdvWorksAPIDefaults>());

// Add & Configure JWT Authorization
builder.Services.ConfigureJwtAuthorization();

// Configure ASP.NET to use the Controller model
// Add & Configure JSON Options
builder.Services.AddControllers().ConfigureJsonOptions();

// Add & Configure Open API (Swagger)
builder.Services.ConfigureOpenAPI();

// **********************************************
// After adding and configuring services
// Create an instance of a WebApplication object
// **********************************************
var app = builder.Build();

// **********************************************
// Configure the HTTP Request Pipeline
// **********************************************
if (app.Environment.IsDevelopment()) {
  // When in Development mode
  // Enable the Open API (Swagger) page
  app.UseSwagger();
  app.UseSwaggerUI();
}

// Enable Exception Handling Middleware
if (app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/DevelopmentError");
}
else {
  app.UseExceptionHandler("/ProductionError");
}

// Handle status code errors in the range 400-599
app.UseStatusCodePagesWithReExecute("/StatusCodeHandler/{0}");

// Enable CORS Middleware
app.UseCors(AdvWorksAPIConstants.CORS_POLICY);

// Enable Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable the endpoints of Controller Action Methods
app.MapControllers();

// Run the Application
app.Run();
