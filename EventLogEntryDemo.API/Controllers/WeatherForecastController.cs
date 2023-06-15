using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;

namespace EventLogEntryDemo.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private const string SectionName = "Application"; // it is reference to the Application event log section, not app name
    private const string SourceName = "EventLogEntryDemo.API.SourceNew";

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var appLog = new EventLog(SectionName);
        var sourceName = CreateEventSource(SectionName);
        appLog.Source = sourceName;

        appLog.WriteEntry("Weather forecast endpoint reached.");
        
        string message = "This is a test message.";
        
        using (var eventLog = new EventLog(SectionName))
        {
            eventLog.Source = sourceName;
            eventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        return result;
    }

    private static string CreateEventSource(string currentAppName)
    {
        var sourceExists = EventLog.SourceExists(SourceName);

        if (!sourceExists)
        {
            EventLog.CreateEventSource(SourceName, currentAppName);
        }
        
        return SourceName;
    }
}