using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace AdvWorksAPI.BaseClasses;

public class ControllerBaseAPI : ControllerBase
{
  protected readonly ILogger _Logger;

  public ControllerBaseAPI(ILogger logger)
  {
    _Logger = logger;
    InfoMessage = string.Empty;
    ErrorLogMessage = string.Empty;
    EntityAsJson = string.Empty;
  }

  public string InfoMessage { get; set; }
  public string ErrorLogMessage { get; set; }
  public string EntityAsJson { get; set; }

  /// <summary>
  /// Serialize an object into a JSON string
  /// </summary>
  /// <typeparam name="T">The type to serialize</typeparam>
  /// <param name="entity">An instance of the type</param>
  /// <returns>A JSON string</returns>
  protected string SerializeEntity<T>(T entity)
  {
    EntityAsJson = "Nothing serialized";

    try {
      // Attempt to serialize entity
      EntityAsJson = JsonSerializer.Serialize(entity);
    }
    catch {
      // Ignore the error
    }

    return EntityAsJson;
  }

  /// <summary>
  /// Call this method to return a '500 Internal Server Error' and log an exception.
  /// </summary>
  /// <typeparam name="T">The type to return</typeparam>
  /// <param name="ex">An Exception object</param>
  /// <param name="infoMsg">The info message to display to the user<param>
  /// <param name="errorMsg">The error message to log</param>
  /// <returns>A Status Code of 500</returns>
  protected ActionResult<T> HandleException<T>(Exception ex, string infoMsg, string errorMsg)
  {
    // Set properties from parameters passed in
    InfoMessage = infoMsg;
    ErrorLogMessage = errorMsg;

    return HandleException<T>(ex);
  }

  /// <summary>
  /// Call this method to return a '500 Internal Server Error' and log an exception.
  /// Prior to calling this method...
  ///    Fill in the InfoMessage property with the value to display to the caller.
  ///    Fill in the ErrorLogMessage property with the value to place into the log file.
  /// </summary>
  /// <typeparam name="T">The type to return</typeparam>
  /// <param name="ex">An Exception object</param>
  /// <returns>A Status Code of 500</returns>
  protected ActionResult<T> HandleException<T>(Exception ex)
  {
    ActionResult<T> ret;

    // Create status code with generic message
    ret = StatusCode(StatusCodes.Status500InternalServerError, InfoMessage);

    // Add Message and Source
    ErrorLogMessage += $"{Environment.NewLine}Message: {ex.Message}";
    ErrorLogMessage += $"{Environment.NewLine}Source: {ex.Source}";
    ErrorLogMessage += $"{Environment.NewLine}Stack Trace: {ex.StackTrace}";

    // Log the exception
    _Logger.LogError(ex, "{ErrorLogMessage}", ErrorLogMessage);

    return ret;
  }
}
