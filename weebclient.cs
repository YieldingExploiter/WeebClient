#region Flags
//#define EXCLUDENONCONFIGALIASES // If pressent, this removes all aliases that add a default WeebData
#define INCLUDEDOWNLOADMETHODS // Include Download* Methods
#define INCLUDEUPLOADMETHODS // Include Upload* Methods
#define USEWEEBCLIENT
#endregion

#region Imports
using System.Security;
using System.IO;
using System.Net.Http;
#endregion

#region WeebClient @ 1.0.0.0
// WeebClient: Single-File WebClient replacement using HttpClient
// Made by weebs, for weebs
// https://github.com/yieldingexploiter/WeebClient
// Copyright © 2022 Yielding#3961. Licensed under the MIT License.

// Available Functions are (usually) API-Compatible with WebClient.
// Some functions are unavailable, whether due to implementation size/difficulty, or otherwise

/// <summary>
/// Namespace for WeebClient & similar stuff
/// </summary>
namespace Weebs
{
  #region OverwriteMode
  public enum OverwriteMode
  {
    /// <summary>
    /// If the file exists already, error
    /// </summary>
    Error = 0,
    /// <summary>
    /// If the file exists already, exit silently
    /// </summary>
    Skip = 1,
    /// <summary>
    /// If the file exists already, replace it
    /// </summary>
    Overwrite = 2,
  }
  #endregion
  #region WeebData
  public class WeebData
  {
    /// <summary>
    /// Ensures an HTTP 200 Success Code
    /// </summary>
    public bool EnsureSuccess = true;
    /// <summary>
    /// Only for file-operations; 
    /// </summary>
    public OverwriteMode OverwriteMode = OverwriteMode.Error;
  }
  #endregion
  #region Custom Exceptions
  /// <summary>
  /// Method is not (and has no plans of being) implemented.
  /// </summary>
  [Serializable]
  public class MethodNotImplementedException : Exception
  {
    public MethodNotImplementedException() { }
    public MethodNotImplementedException(string message) : base(message) { }
    public MethodNotImplementedException(string message, Exception inner) : base(message, inner) { }
    protected MethodNotImplementedException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }
  /// <summary>
  /// Input disallows certain behaviour, which was triggered regardless.
  /// </summary>

  [Serializable]
  public class ConfigDisallowsBehaviourException : Exception
  {
    public ConfigDisallowsBehaviourException() { }
    public ConfigDisallowsBehaviourException(string message) : base(message) { }
    public ConfigDisallowsBehaviourException(string message, Exception inner) : base(message, inner) { }
    protected ConfigDisallowsBehaviourException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }
  /// <summary>
  /// Overwrites disabled in <seealso cref="WeebData">WeebData</seealso> (or it's defaults), and the destiation file alreaddy exists.<br/>
  /// The request has already been made by the time this is thrown.<br/>
  /// <br/>
  /// (Extends <see cref="ConfigDisallowsBehaviourException"/>)
  /// </summary>
  [Serializable]
  public class OverwriteDisabledException : ConfigDisallowsBehaviourException
  {
    public OverwriteDisabledException() { }
    public OverwriteDisabledException(string message) : base(message) { }
    public OverwriteDisabledException(string message, Exception inner) : base(message, inner) { }
    protected OverwriteDisabledException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }
  /// <summary>
  /// Feature was disabled at compile-time
  /// </summary>

  [Serializable]
  public class FeatureDisabledAtCompileTimeException : Exception
  {
    public FeatureDisabledAtCompileTimeException() { }
    public FeatureDisabledAtCompileTimeException(string message) : base(message) { }
    public FeatureDisabledAtCompileTimeException(string message, Exception inner) : base(message, inner) { }
    protected FeatureDisabledAtCompileTimeException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }
  #endregion
  /// <summary>
  /// <seealso cref="WebClient"/> Polyfill for <seealso cref="Weebs"/>, by <seealso cref="Weebs"/>, using <seealso cref="HttpClient"/>.<br/>
  /// <br/>
  /// Copyright © 2022 <a href="https://github.com/yieldingexploiter">Yielding#3961</a>.<br/>
  /// Copyright © 2022 <a href="https://github.com/mokiycodes">MokiyCodes</a>.<br/>
  /// Licensed under the MIT License.<br/>
  /// </summary>
  /// <remarks>
  /// Documentation is partial and incomplete.<br/><br/>
  /// Unavailable (with no plans of being added soon):<br/> <!-- PRs to add these are welcome! -->
  /// - OpenRead(-Async)<br/>
  /// - OpenWrite(-Async)<br/>
  /// <br/>
  /// Incomplete/Unavailable (with plans of being added):<br/>
  /// - UploadData(-Async)<br/>
  /// - UploadFile(-Async)<br/>
  /// - UploadValues(-Async)<br/>
  /// - UploadString(-Async)<br/>
  /// </remarks>
  public class WeebClient
  {
    #region Default Config Data
    /// <summary>
    /// Default Config Data; used when no special data is specified.<br/>
    /// See <see cref="WeebData"/> for more info.
    /// </summary>
    /// <remarks>
    /// Not part of Vanilla WebClient; custom config for WeebClient
    /// </remarks>
    public WeebData DefaultConfig = new WeebData();
    #endregion
    #region Byte[] <-> string
    /// <summary>
    /// Converts a byte array to a string
    /// </summary>
    /// <param name="bytes">Byte array</param>
    /// <returns>String matching the <paramref name="bytes"/></returns>
    public static string ByteArrayToString(byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    /// <summary>
    /// Converts a string to a byte array
    /// </summary>
    /// <param name="str">String</param>
    /// <returns>Byte array matching <paramref name="str"/></returns>
    public static byte[] StringToByteArray(string str) => System.Text.Encoding.UTF8.GetBytes(str);
    #endregion
    #region Unsupported methods
    [Obsolete("Cancelling requests is too much of a pain in the ass to implement, and is therefor unsupported. Please use HttpClient directly if this functionality is needed.", true)]
    public void CancelAsync()
    {
      throw new MethodNotImplementedException("Method is not (and will not) be implemented in WeebClient. See Compiler Warnings for more information.");
    }
    [Obsolete("Disposing of Weebclient is unnecessary and therefor unavailable.")]
    public void Dispose()
    {
      Console.Error.WriteLine(new MethodNotImplementedException("Disposing is not supported in WeebClient. Just feed it to the garbage collector (Weebclient disposes everything post-use)"));
    }
    #endregion
    #region IsBusy
    internal int _RequestCount = 0;
    /// <summary>
    /// Amount of currently active requests
    /// </summary>
    public int RequestCount
    {
      get
      {
        return _RequestCount;
      }
    }
    /// <summary>
    /// Is WeebClient currently handling a request?<br/>
    /// <b>IMPORTANT:</b> WeebClient can handle multiple requests at once. This is only true during the request (not during, ie, file write, etc...)
    /// </summary>
    public bool IsBusy
    {
      get
      {
        return _RequestCount > 0;
      }
    }
    #endregion
    #region Create New HTTPClient
    /// <summary>
    /// Internal method to make an Http Client
    /// </summary>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <returns><see cref="HttpClient"/> with things like the correct headers, etc...</returns>
    public HttpClient CreateNewHttpClient()
    {
      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Clear();
      foreach (object header in Headers)
      {
        client.DefaultRequestHeaders.Add((string)header, Headers.Get((string)header));
      }
      return client;
    }
    #endregion
    #region Raw HTTP Function
    internal System.Net.WebHeaderCollection _Headers = new System.Net.WebHeaderCollection();
    /// <summary>
    /// Headers to send with every request
    /// </summary>
    public System.Net.WebHeaderCollection Headers { get { return _Headers; } }
    /// <summary>
    /// Internal Request Function; returns HttpResponseMessage from request made to URI
    /// </summary>
    /// <param name="address">URI to request</param>
    /// <returns>HttpResponseMessage Response</returns>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="TaskCanceledException"/>
    public async Task<HttpResponseMessage> RawGetAsync(Uri address)
    {
      // Input Check
      if (address == null) throw new ArgumentNullException("address is null");
      // Mark as busy
      _RequestCount++;
      // Make Client
      HttpClient client = CreateNewHttpClient();
      // Add Headers
      // Perform Request
      HttpResponseMessage response = await client.GetAsync(address);
      // Dispose of HTTP Client
      client.Dispose();
      // Mark as not busy
      _RequestCount--;
      // Return value
      return response;
    }
    /// <summary>
    /// Internal Request Function; returns HttpResponseMessage from request made to URI
    /// </summary>
    /// <param name="address">URI to request</param>
    /// <param name="Data">Data for the request</param>
    /// <returns>HttpResponseMessage Response</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="TaskCanceledException"/>
    public async Task<HttpResponseMessage> RawPostAsync(Uri address, byte[] Data)
    {
      // Input Check
      if (address == null) throw new ArgumentNullException("address is null");
      if (Data == null) Data = Array.Empty<byte>();
      // Mark as busy
      _RequestCount++;
      // Make Client
      HttpClient client = CreateNewHttpClient();
      // Perform Request
      HttpResponseMessage response = await client.PostAsync(address, new ByteArrayContent(Data));
      // Dispose of HTTP Client
      client.Dispose();
      // Mark as not busy
      _RequestCount--;
      // Return value
      return response;
    }
    /// <summary>
    /// Internal Request Function; returns HttpResponseMessage from request made to URI
    /// </summary>
    /// <param name="address">URI to request</param>
    /// <param name="Data">Data for the request</param>
    /// <returns>HttpResponseMessage Response</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="TaskCanceledException"/>
    public async Task<HttpResponseMessage> RawPostAsync(Uri address, string Data)
    {
      // Mark as busy
      _RequestCount++;
      // Make Client
      HttpClient client = CreateNewHttpClient();
      // Perform Request
      HttpResponseMessage response = await client.PostAsync(address, new StringContent(Data));
      // Dispose of HTTP Client
      client.Dispose();
      // Mark as not busy
      _RequestCount--;
      // Return value
      return response;
    }
    #endregion
    #region Download Methods
#if (INCLUDEDOWNLOADMETHODS)
    #region Download Data
    /// <summary>
    /// Makes a request to <paramref name="address"/>, using config <paramref name="configData"/>
    /// </summary>
    /// <param name="address">URI</param>
    /// <param name="configData">Configuration</param>
    /// <returns>byte[] containing data from HTTP Request</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="OperationCanceledException"/>
    public async Task<byte[]> DownloadDataAsync(Uri address, WeebData configData)
    {
      // Input Check
      if (address == null) throw new ArgumentNullException("address is null");
      if (configData == null) configData = new WeebData();
      // Perform HTTP request
      HttpResponseMessage response = await RawGetAsync(address);
      if (configData.EnsureSuccess) response.EnsureSuccessStatusCode();
      // Read Data
      byte[] data = await response.Content.ReadAsByteArrayAsync();
      // Dispose Response
      response.Dispose();
      // Return Data
      return data;
    }
    /// <inheritdoc cref="DownloadDataAsync(Uri, WeebData)"/>
    /// <exception cref="UriFormatException"/>
    public async Task<byte[]> DownloadDataAsync(string address, WeebData configData)
    {
      if (address == null) throw new ArgumentNullException("Address is null");
      if (configData == null) configData = new WeebData();
      Uri addressUri = new Uri(address);
      return await DownloadDataAsync(addressUri, configData);
    }
#if (!EXCLUDENONCONFIGALIASES)
    /// <summary>
    /// <see cref="DownloadDataAsync(string, WeebData)"/> with a default config
    /// </summary>
    /// <inheritdoc cref="DownloadDataAsync(string, WeebData)"/>
    public async Task<byte[]> DownloadDataAsync(string address) => await DownloadDataAsync(address, DefaultConfig);
    /// <summary>
    /// <see cref="DownloadDataAsync(Uri, WeebData)"/> with a default config
    /// </summary>
    /// <inheritdoc cref="DownloadDataAsync(string, WeebData)"/>
    public async Task<byte[]> DownloadDataAsync(Uri address) => await DownloadDataAsync(address, DefaultConfig);
#endif
    /// <summary>
    /// Synchronous <see cref="DownloadDataAsync(Uri, WeebData)"/>
    /// </summary>
    /// <inheritdoc cref="DownloadDataAsync(string, WeebData)"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="ObjectDisposedException"/>
    /// <exception cref="AggregateException"/>
    public byte[] DownloadData(Uri address, WeebData configData)
    {
      if (address == null) throw new ArgumentNullException("Address is null");
      if (configData == null) configData = new WeebData();
      Task<byte[]> task = Task.Run(async () =>
      {
        return await DownloadDataAsync(address, configData);
      });
      task.Wait();
      return task.Result;
    }
    /// <summary>
    /// Synchronous <see cref="DownloadDataAsync(string, WeebData)"/>
    /// </summary>
    /// <inheritdoc cref="DownloadData(Uri, WeebData)"/>
    /// <exception cref="UriFormatException"/>
    public byte[] DownloadData(string address, WeebData configData) => DownloadData(new Uri(address), configData);
#if (!EXCLUDENONCONFIGALIASES)
    /// <inheritdoc cref="DownloadDataAsync(Uri)"/>
    /// <exception cref="UriFormatException"/>
    public byte[] DownloadData(string address) => DownloadData(address, DefaultConfig);
    /// <summary>
    /// Synchronous <see cref="DownloadDataAsync(string)"/>
    /// </summary>
    /// <inheritdoc cref="DownloadData(Uri, WeebData)"/>
    public byte[] DownloadData(Uri address) => DownloadData(address, DefaultConfig);
#endif
    #endregion
    #region Download File

    /// <inheritdoc cref="__Docs__DownloadFileAsync_Config"/>
    public async Task DownloadFileAsync(Uri address, string path, WeebData requestConfig)
    {
      // Input Check
      if (address == null) throw new ArgumentNullException("address is null");
      if (path == null) throw new ArgumentNullException("path is null");
      if (requestConfig == null) requestConfig = new WeebData();
      // Perform HTTP Request
      HttpResponseMessage response = await RawGetAsync(address);
      // Get File Path
      string FilePath = Path.GetFullPath(path);
      // Create FileStream
      FileStream fs;
      if (File.Exists(FilePath))
      {
        switch (requestConfig.OverwriteMode)
        {
          case OverwriteMode.Skip:
            return;
          case OverwriteMode.Error:
            throw new OverwriteDisabledException("Overwriting files disabled in WeebData config. Please set to OverwriteMode.Overwrite to overwrite the file, or OverwriteMode.Skip to ignore.");
        }
        fs = new FileStream(FilePath, FileMode.Truncate);
      }
      else fs = new FileStream(FilePath, FileMode.CreateNew);
      // Write to file
      await response.Content.CopyToAsync(fs);
      // Propperly dispose of everything
      fs.Close();
      fs.Dispose();
      response.Dispose();
    }
    /// <inheritdoc cref="__Docs__DownloadFileAsync_Config"/>
    /// <exception cref="UriFormatException"/>
    public async Task DownloadFileAsync(string address, string path, WeebData requestConfig)
    {
      Uri addressUri = new Uri(address);
      await DownloadFileAsync(addressUri, path, requestConfig);
    }
    /// <inheritdoc cref="__Docs__DownloadFile_Config"/>
    public void DownloadFile(Uri address, string path, WeebData requestConfig)
    {
      Task task = Task.Run(async () =>
      {
        await DownloadFileAsync(address, path, requestConfig);
      });
      task.Wait();
    }
    /// <inheritdoc cref="__Docs__DownloadFile_Config"/>
    /// <exception cref="UriFormatException"/>
    public void DownloadFile(string address, string path, WeebData requestConfig)
    {
      Uri addressUri = new Uri(address);
      DownloadFile(addressUri, path, requestConfig);
    }
    #region Non-Config Aliases
#if (!EXCLUDENONCONFIGALIASES)
    /// <inheritdoc cref="__Docs__DownloadFileAsync_NoConfig"/>
    public Task DownloadFileAsync(Uri address, string path) => DownloadFileAsync(address, path, DefaultConfig);
    /// <inheritdoc cref="__Docs__DownloadFileAsync_NoConfig"/>
    /// <exception cref="UriFormatException"/>
    public Task DownloadFileAsync(string address, string path) => DownloadFileAsync(address, path, DefaultConfig);
    /// <inheritdoc cref="__Docs__DownloadFile_NoConfig"/>
    public void DownloadFile(Uri address, string path) => DownloadFile(address, path, DefaultConfig);
    /// <inheritdoc cref="__Docs__DownloadFile_NoConfig"/>
    public void DownloadFile(string address, string path) => DownloadFile(address, path, DefaultConfig);
#endif
    #endregion
    #region Docs
    /// <exception cref="ConfigDisallowsBehaviourException"/>
    /// <exception cref="OverwriteDisabledException">If file at <paramref name="path"/> exists, and Data.OverwriteMode == Error (or Data/Data.OverwriteMode was not specified)</exception>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="TaskCanceledException"/>
    /// <exception cref="SecurityException"/>
    /// <exception cref="NotSupportedException"/>
    /// <exception cref="PathTooLongException"/>
    public void __Docs__DownloadFile_Exceptions()
    { }
    /// <summary>
    /// Downloads the file at <paramref name="address"/> to <paramref name="path"/>
    /// </summary>
    /// <inheritdoc cref="__Docs__DownloadFile_Exceptions"/>
    public void __Docs__DownloadFileAsync_NoConfig()
    { }
    /// <summary>
    /// <inheritdoc cref="__Docs__DownloadFileAsync_NoConfig"/>, using config <paramref name="requestConfig"/>
    /// </summary>
    /// <inheritdoc cref="__Docs__DownloadFile_Exceptions"/>
    public void __Docs__DownloadFileAsync_Config()
    { }
    /// <summary>
    /// Synchronously <inheritdoc cref="__Docs__DownloadFileAsync_NoConfig"/>
    /// </summary>
    /// <inheritdoc cref="__Docs__DownloadFile_Exceptions"/>
    /// <exception cref="ObjectDisposedException"/>
    /// <exception cref="AggregateException"/>
    public void __Docs__DownloadFile_NoConfig()
    { }
    /// <summary>
    /// Synchronously <inheritdoc cref="__Docs__DownloadFileAsync_Config"/>
    /// </summary>
    /// <inheritdoc cref="__Docs__DownloadFile_Exceptions"/>
    /// <exception cref="ObjectDisposedException"/>
    /// <exception cref="AggregateException"/>
    public void __Docs__DownloadFile_Config()
    { }
    #endregion
    #endregion
    #region Download String
    /// <inheritdoc cref="__Docs__DownloadStringAsync_Config"/>
    public async Task<string> DownloadStringAsync(Uri address, WeebData configData) => ByteArrayToString(await DownloadDataAsync(address, configData));
    /// <inheritdoc cref="__Docs__DownloadStringAsync_Config"/>
    /// <exception cref="UriFormatException"/>
    public async Task<string> DownloadStringAsync(string address, WeebData configData) => ByteArrayToString(await DownloadDataAsync(address, configData));
    /// <inheritdoc cref="__Docs__DownloadStringAsync_NoConfig"/>
    public async Task<string> DownloadStringAsync(Uri address) => ByteArrayToString(await DownloadDataAsync(address));
    /// <inheritdoc cref="__Docs__DownloadStringAsync_NoConfig"/>
    /// <exception cref="UriFormatException"/>
    public async Task<string> DownloadStringAsync(string address) => ByteArrayToString(await DownloadDataAsync(address));
    /// <inheritdoc cref="__Docs__DownloadString_Config"/>
    public string DownloadString(Uri address, WeebData configData) => ByteArrayToString(DownloadData(address, configData));
    /// <inheritdoc cref="__Docs__DownloadString_Config"/>
    /// <exception cref="UriFormatException"/>
    public string DownloadString(string address, WeebData configData) => ByteArrayToString(DownloadData(address, configData));
    /// <inheritdoc cref="__Docs__DownloadString_NoConfig"/>
    public string DownloadString(Uri address) => ByteArrayToString(DownloadData(address));
    /// <inheritdoc cref="__Docs__DownloadString_NoConfig"/>
    /// <exception cref="UriFormatException"/>
    public string DownloadString(string address) => ByteArrayToString(DownloadData(address));
    #region Docs
    /// <summary></summary>
    /// <returns></returns>
    /// <inheritdoc cref="DownloadData(Uri, WeebData)"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="FormatException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="OperationCanceledException"/>
    public void __Docs__DownloadStringAsync_Exceptions()
    { }
    /// <inheritdoc cref="__Docs__DownloadStringAsync_Exceptions"/>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="AggregateException"/>
    public void __Docs__DownloadString_Exceptions()
    { }
    /// <summary>
    /// Downloads the file at <paramref name="address"/> using config <paramref name="configData"/>, and returns it represented as a string.<br/>
    /// Internally, this calls <seealso cref="ByteArrayToString(byte[])"/> on the output from <seealso cref="DownloadDataAsync"/>.
    /// </summary>
    /// <returns>Raw, <b>unsanitized</b> string</returns>
    /// <inheritdoc cref="__Docs__DownloadStringAsync_Exceptions"/>
    public void __Docs__DownloadStringAsync_Config()
    { }
    /// <summary>
    /// Downloads the file at <paramref name="address"/> using the default config, and returns it represented as a string.<br/>
    /// Internally, this calls <seealso cref="ByteArrayToString(byte[])"/> on the output from <seealso cref="DownloadDataAsync"/>.
    /// </summary>
    /// <returns>Raw, <b>unsanitized</b> string</returns>
    /// <inheritdoc cref="__Docs__DownloadStringAsync_Exceptions"/>
    public void __Docs__DownloadStringAsync_NoConfig()
    { }
    /// <summary>
    /// Synchronously downloads the file at <paramref name="address"/> using config <paramref name="configData"/>, and returns it represented as a string.<br/>
    /// Internally, this calls <seealso cref="ByteArrayToString(byte[])"/> on the output from <seealso cref="DownloadData"/>.
    /// </summary>
    /// <returns>Raw, <b>unsanitized</b> string</returns>
    /// <inheritdoc cref="__Docs__DownloadString_Exceptions"/>
    public void __Docs__DownloadString_Config()
    { }

    /// <summary>
    /// Synchronously downloads the file at <paramref name="address"/> using the default config, and returns it represented as a string.<br/>
    /// Internally, this calls <seealso cref="ByteArrayToString(byte[])"/> on the output from <seealso cref="DownloadData"/>.
    /// </summary>
    /// <returns>Raw, <b>unsanitized</b> string</returns>
    /// <inheritdoc cref="__Docs__DownloadString_Exceptions"/>
    public void __Docs__DownloadString_NoConfig()
    { }
    #endregion
    #endregion
#endif
    #endregion
    // TODO: Add upload variants of everything
    #region Upload Methods
#if INCLUDEUPLOADMETHODS
    #region Upload Data
    /// <summary>
    /// Sends a <b>POST</b> request to <paramref name="address"/> with data <paramref name="send"/>
    /// </summary>
    /// <param name="address">Address to send the data to</param>
    /// <param name="send">Data to send</param>
    /// <param name="configData">Configuration Options</param>
    /// <returns>Response Body</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc cref="RawPostAsync(Uri, byte[])"/>
    public async Task<byte[]> UploadDataAsync(Uri address, byte[] send, WeebData configData)
    {
      // Input Check
      if (address == null) throw new ArgumentNullException("address is null");
      if (configData == null) configData = new WeebData();
      // Perform HTTP request
      HttpResponseMessage res = await RawPostAsync(address, send);
      if (configData.EnsureSuccess) res.EnsureSuccessStatusCode();
      // Read Data
      byte[] received = await res.Content.ReadAsByteArrayAsync();
      // Dispose Response
      res.Dispose();
      // Return Data
      return received;
    }
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[], WeebData)"/>
    /// <exception cref="UriFormatException"/>
    public Task<byte[]> UploadDataAsync(string address, byte[] send, WeebData configData) => UploadDataAsync(new Uri(address), send, configData);
#if (!EXCLUDENONCONFIGALIASES)
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[], WeebData)"/>
    /// <exception cref="UriFormatException"/>
    /// <remarks>
    /// Uses <see cref="DefaultConfig"/> as the config. If this is undesired, add your own.
    /// </remarks>
    public Task<byte[]> UploadDataAsync(Uri address, byte[] send) => UploadDataAsync(address, send, DefaultConfig);
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[])"/>
    /// <exception cref="UriFormatException"/>
    public Task<byte[]> UploadDataAsync(string address, byte[] send) => UploadDataAsync(address, send, DefaultConfig);
#endif
    #endregion
    #region UploadString
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[], WeebData)"/>
    public async Task<string> UploadStringAsync(Uri address, string send, WeebData config) => ByteArrayToString(await UploadDataAsync(address, StringToByteArray(send), config));
    /// <inheritdoc cref="UploadDataAsync(string, byte[], WeebData)"/>
    public Task<string> UploadStringAsync(string address, string send, WeebData config) => UploadStringAsync(new Uri(address), send, config);
#if (!EXCLUDENONCONFIGALIASES)
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[])"/>
    public Task<string> UploadStringAsync(Uri address, string send) => UploadStringAsync(address, send, DefaultConfig);
    /// <inheritdoc cref="UploadDataAsync(string, byte[])"/>
    public Task<string> UploadStringAsync(string address, string send) => UploadStringAsync(address, send, DefaultConfig);
#endif
    #endregion
    #region Upload
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[], WeebData)"/>
    public Task<byte[]> UploadAsync(Uri address, byte[] send, WeebData config) => UploadDataAsync(address, send, config);
    /// <inheritdoc cref="UploadDataAsync(string, byte[], WeebData)"/>
    public Task<byte[]> UploadAsync(string address, byte[] send, WeebData config) => UploadDataAsync(address, send, config);
#if (!EXCLUDENONCONFIGALIASES)
    /// <inheritdoc cref="UploadDataAsync(Uri, byte[])"/>
    public Task<byte[]> UploadAsync(Uri address, byte[] send) => UploadDataAsync(address, send);
    /// <inheritdoc cref="UploadDataAsync(string, byte[])"/>
    public Task<byte[]> UploadAsync(string address, byte[] send) => UploadDataAsync(address, send);
#endif
    /// <inheritdoc cref="UploadStringAsync(string, string, WeebData)"/>
    public Task<string> UploadAsync(string address, string send, WeebData config) => UploadStringAsync(address, send, config);
    /// <inheritdoc cref="UploadStringAsync(Uri, string, WeebData)"/>
    public Task<string> UploadAsync(Uri address, string send, WeebData config) => UploadStringAsync(address, send, config);
    /// <inheritdoc cref="UploadStringAsync(string, string)"/>
    public Task<string> UploadAsync(string address, string send) => UploadStringAsync(address, send);
    /// <inheritdoc cref="UploadStringAsync(Uri, string)"/>
    public Task<string> UploadAsync(Uri address, string send) => UploadStringAsync(address, send);
    #endregion
#endif
    #endregion
  }
  #region Alias: WebClient
  /// <summary>
  /// Alias to <seealso cref="WeebClient"/>; for ease of use.
  /// <inheritdoc cref="WeebClient"/>
  /// </summary>
  /// <inheritdoc cref="WeebClient"/>
  public class WebClient : WeebClient { }
  #endregion
}
#endregion
