using Microsoft.EntityFrameworkCore;
using OpenLS.Base.UOM;
using OpenWLS.Server;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.DBase.Repositories;
using OpenWLS.Server.DBase.Repositories.InstrumentM;
using OpenWLS.Server.WebSocket;
using System.Net.WebSockets;

ServerGlobals.Init();
MeasurementUnit.Init(ServerGlobals.uom_db_fn);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Base
builder.Services.AddDbContext<GlobalDbContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("GlobalDb")));
builder.Services.AddDbContext<LocalDbContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("LocalDb")));
builder.Services.AddDbContext<CalibrationDbContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("CalibrationDb")));
builder.Services.AddDbContext<SyslogDbContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("SyslogDb")));

//builder.Services.AddScoped<IDeviceContext>(provider => provider.GetService<DeviceContext>());
builder.Services.AddScoped<ISyslogRepository, SyslogRepository>();

builder.Services.AddScoped<IGlobalVDFRepository, GlobalVDFRepository>();
builder.Services.AddScoped<ILocalVDFRepository, LocalVDFRepository>();

builder.Services.AddScoped<IGlobalOCFRepository, GlobalOCFRepository>();
builder.Services.AddScoped<ILocalOCFRepository, LocalOCFRepository>();

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IPassRepository, PassRepository>();
//builder.Services.AddScoped<IRunRepository, RunRepository>();

//builder.Services.AddScoped<ISyslogRepository, SyslogRepository>();

builder.Services.AddScoped<IInstrumentRepository, InstrumentRepository>();
builder.Services.AddScoped<IInstSubRepository, InstSubRepository>();
builder.Services.AddScoped<IMGroupRepository, MGroupRepository>();
builder.Services.AddScoped<IMeasurementRepository, MeasurementRepository>();
builder.Services.AddScoped<IEdgeRepository, EdgeRepository>();



builder.Services.AddScoped<ICalibrationRepository, CalibrationRepository>();

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddGrpc();
builder.Services.AddSingleton<IWsService, WsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseWebSockets();

app.MapControllers();


IWsService wsService = (IWsService)app.Services.GetService<IWsService>();
wsService.Init(app);

//XtfFile f = new XtfFile();
//f.ImportXtf(@"C:\tmp\bb\data\server\jobs\test2\c836KR01.xtf", null);

app.Run();
