﻿/*
 * Copyright © 2016 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Fronter Developments plc.
 */
using EDDiscovery;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace EDDiscovery2.HTTP
{
    public class HttpCom
    {
        protected string _serverAddress;

        public String MimeType { get; set; } = "application/json; charset=utf-8";

        protected ResponseData RequestPost(string json, string action, NameValueCollection headers = null, bool handleException = false)
        {
            if (_serverAddress == null || _serverAddress.Length == 0)           // for debugging, set _serveraddress empty
            {
                System.Diagnostics.Trace.WriteLine("POST:" + action);
                return new ResponseData(HttpStatusCode.Unauthorized);
            }

            try
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serverAddress + action);
                    // Set the Method property of the request to POST.
                    request.Method = "POST";
                    if (headers != null)
                    {
                        request.Headers.Add(headers);
                    }
                    // Create POST data and convert it to a byte array.
                    //WRITE JSON DATA TO VARIABLE D
                    //string postData = "{\"requests\":[{\"C\":\"Gpf_Auth_Service\", \"M\":\"authenticate\", \"fields\":[[\"name\",\"value\"],[\"Id\",\"\"],[\"username\",\"user@example.com\"],[\"password\",\"ab9ce908\"],[\"rememberMe\",\"Y\"],[\"language\",\"en-US\"],[\"roleType\",\"M\"]]}],\"C\":\"Gpf_Rpc_Server\", \"M\":\"run\"}";
                    string postData = json;


                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    // Set the ContentType property of the WebRequest.
                    request.ContentType = MimeType;
                    //request.Headers.Add("Accept-Encoding", "gzip,deflate");
                    // Set the ContentLength property of the WebRequest.
                    request.ContentLength = byteArray.Length;
                    // Get the request stream.
                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();
                    // Get the response.
                    //request.Timeout = 740 * 1000;

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        WriteLog("POST " + request.RequestUri, postData);

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    var data = getResponseData(response);
                    response.Close();

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                    {
                        //TODO: Log Status Code too
                        WriteLog(data.Body, "");
                    }

                    return data;
                }
                catch (WebException ex)
                {
                    if (!handleException)
                    {
                        throw;
                    }

                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        var data = getResponseData(httpResponse);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        System.Diagnostics.Trace.WriteLine("WebException : " + ex.Message);
                        System.Diagnostics.Trace.WriteLine("Response code : " + httpResponse.StatusCode);
                        System.Diagnostics.Trace.WriteLine("Response body : " + data.Body);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        {
                            WriteLog("WebException" + ex.Message, "");
                            WriteLog($"HTTP Error code: {httpResponse.StatusCode}", "");
                            WriteLog($"HTTP Error body: {data.Body}", "");
                        }
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!handleException)
                {
                    throw;
                }

                System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                if (EDDiscoveryForm.EDDConfig.EDSMLog)
                {
                    WriteLog("Exception" + ex.Message, "");
                }

                return new ResponseData(HttpStatusCode.BadRequest);
            }
        }

        protected ResponseData RequestPatch(string json, string action, NameValueCollection headers = null, bool handleException = false)
        {
            if (_serverAddress == null || _serverAddress.Length == 0)           // for debugging, set _serveraddress empty
            {
                System.Diagnostics.Trace.WriteLine("PATCH:" + action);
                return new ResponseData(HttpStatusCode.Unauthorized);
            }

            try
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serverAddress + action);
                    request.Method = "PATCH";
                    if (headers != null)
                    {
                        request.Headers.Add(headers);
                    }
                    string postData = json;


                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    // Set the ContentType property of the WebRequest.
                    request.ContentType = MimeType;
                    //request.Headers.Add("Accept-Encoding", "gzip,deflate");
                    // Set the ContentLength property of the WebRequest.
                    request.ContentLength = byteArray.Length;
                    // Get the request stream.
                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();
                    // Get the response.
                    //request.Timeout = 740 * 1000;

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        WriteLog("PATCH " + request.RequestUri, postData);

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    var data = getResponseData(response);
                    response.Close();

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                    {
                        WriteLog(data.Body, "");
                    }

                    return data;
                }
                catch (WebException ex)
                {
                    if (!handleException)
                    {
                        throw;
                    }

                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        var data = getResponseData(httpResponse);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        System.Diagnostics.Trace.WriteLine("WebException : " + ex.Message);
                        System.Diagnostics.Trace.WriteLine("Response code : " + httpResponse.StatusCode);
                        System.Diagnostics.Trace.WriteLine("Response body : " + data.Body);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        {
                            WriteLog("WebException" + ex.Message, "");
                            WriteLog($"HTTP Error code: {httpResponse.StatusCode}", "");
                            WriteLog($"HTTP Error body: {data.Body}", "");
                        }
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!handleException)
                {
                    throw;
                }

                System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                if (EDDiscoveryForm.EDDConfig.EDSMLog)
                {
                    WriteLog("Exception" + ex.Message, "");
                }

                return new ResponseData(HttpStatusCode.BadRequest);
            }
        }



        protected ResponseData RequestGet(string action, NameValueCollection headers = null, bool handleException = false)
        {
            if ( _serverAddress == null || _serverAddress.Length == 0 )           // for debugging, set _serveraddress empty
            {
                System.Diagnostics.Trace.WriteLine("GET:" + action);
                return new ResponseData(HttpStatusCode.Unauthorized);
            }

            try
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serverAddress + action);
                    // Set the Method property of the request to POST.
                    request.Method = "GET";

                    // Set the ContentType property of the WebRequest.
                    request.ContentType = MimeType;
                    request.Headers.Add("Accept-Encoding", "gzip,deflate");
                    if (headers != null)
                    {
                        request.Headers.Add(headers);
                    }
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        WriteLog("GET " + request.RequestUri, "");


                    // Get the response.
                    //request.Timeout = 740 * 1000;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    var data = getResponseData(response);
                    response.Close();

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                    {
                        WriteLog(data.Body, "");
                    }

                    return data;
                }
                catch (WebException ex)
                {
                    if (!handleException)
                    {
                        throw;
                    }

                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        var data = getResponseData(httpResponse);

      
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        System.Diagnostics.Trace.WriteLine("WebException : " + ex.Message);
                        if (httpResponse != null)
                        {
                            System.Diagnostics.Trace.WriteLine("Response code : " + httpResponse.StatusCode);
                            System.Diagnostics.Trace.WriteLine("Response body : " + data.Body);
                        }
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        {
                            WriteLog("WebException" + ex.Message, "");
                            if (httpResponse != null)
                            {

                                WriteLog($"HTTP Error code: {httpResponse.StatusCode}", "");
                                WriteLog($"HTTP Error body: {data.Body}", "");
                            }
                        }
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!handleException)
                {
                    throw;
                }

                System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);

                if (EDDiscoveryForm.EDDConfig.EDSMLog)
                {
                    WriteLog("Exception" + ex.Message, "");
                }


                return new ResponseData(HttpStatusCode.BadRequest);
            }

        }


        protected ResponseData RequestDelete(string action, NameValueCollection headers = null, bool handleException = false)
        {
            if (_serverAddress == null || _serverAddress.Length == 0)           // for debugging, set _serveraddress empty
            {
                System.Diagnostics.Trace.WriteLine("DELETE:" + action);
                return new ResponseData(HttpStatusCode.Unauthorized);
            }

            try
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serverAddress + action);
                    request.Method = "DELETE";

                    // Set the ContentType property of the WebRequest.
                    request.ContentType = MimeType;
                    request.Headers.Add("Accept-Encoding", "gzip,deflate");
                    if (headers != null)
                    {
                        request.Headers.Add(headers);
                    }
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        WriteLog("DELETE " + request.RequestUri, "");


                    // Get the response.
                    //request.Timeout = 740 * 1000;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    var data = getResponseData(response);
                    response.Close();

                    if (EDDiscoveryForm.EDDConfig.EDSMLog)
                    {
                        WriteLog(data.Body, "");
                    }

                    return data;
                }
                catch (WebException ex)
                {
                    if (!handleException)
                    {
                        throw;
                    }

                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        var data = getResponseData(httpResponse);
                        data.Error = true;
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        System.Diagnostics.Trace.WriteLine("WebException : " + ex.Message);
                        System.Diagnostics.Trace.WriteLine("Response code : " + httpResponse.StatusCode);
                        System.Diagnostics.Trace.WriteLine("Response body : " + data.Body);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        if (EDDiscoveryForm.EDDConfig.EDSMLog)
                        {
                            WriteLog("WebException" + ex.Message, "");
                            WriteLog($"HTTP Error code: {httpResponse.StatusCode}", "");
                            WriteLog($"HTTP Error body: {data.Body}", "");
                        }
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!handleException)
                {
                    throw;
                }

                System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);

                if (EDDiscoveryForm.EDDConfig.EDSMLog)
                {
                    WriteLog("Exception" + ex.Message, "");
                }


                return new ResponseData(HttpStatusCode.BadRequest, error: true);
            }

        }


        private static Object LOCK = new Object();
        static public  void WriteLog(string str1, string str2)
        {
            if (str1 != null && str1.ToUpper().Contains("PASSWORD"))
                str1 = "** This string contains a password so not logging it.**";

            if (str2 != null && str2.ToUpper().Contains("PASSWORD"))
                str2 = "** This string contains a password so not logging it.**";

            if (EDDiscoveryForm.EDDConfig.EDSMLog == false)
                return;

            try
            {
                lock(LOCK)
                {
                    string filename = Path.Combine(Tools.GetAppDataDirectory(), "Log", "EDD_" + EDDiscoveryForm.EDDConfig.LogIndex + ".log");

                    using (StreamWriter w = File.AppendText(filename))
                    {
                        w.WriteLine(DateTime.Now.ToLongTimeString() + "; " + str1 + "; " + str2);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
        }
        private ResponseData getResponseData(HttpWebResponse response, bool? error = null)
        {
            if (response == null)
                return new ResponseData(HttpStatusCode.NotFound);

            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var data = new ResponseData(response.StatusCode, reader.ReadToEnd(), response.Headers);
            reader.Close();
            dataStream.Close();
            return data;
        }

    }
}
