using Dobeil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DobeilGameServiceBase : MonoSingleton<DobeilGameServiceBase>
{
    public void SendRequest(string _baseUrl, string method, string bodyData = "", Dictionary<string, string> headers = null, Action<HttpResponse> callback = null)
    {
        string url = _baseUrl;
        StartCoroutine(SendRequestCoroutine(url, method, bodyData, headers, callback));
    }

    private IEnumerator SendRequestCoroutine(string url, string method, string bodyData = "", Dictionary<string, string> headers = null, Action<HttpResponse> callback = null)
    {
        using (UnityWebRequest request = CreateRequest(url, method, bodyData, headers))
        {
            yield return request.SendWebRequest();
            HttpResponse response = new HttpResponse(request);
            callback?.Invoke(response);
        }
    }

    private UnityWebRequest CreateRequest(string url, string method, string bodyData = "", Dictionary<string, string> headers = null)
    {
        UnityWebRequest request;

        switch (method)
        {
            case "GET":
                request = UnityWebRequest.Get(url);
                break;
            case "PUT":
                request = UnityWebRequest.Put(url, bodyData);
                break;
            // Add more methods (PUT, DELETE, etc.) as needed.
            default:
                throw new ArgumentException("Unsupported HTTP method: " + method);
        }

        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        return request;
    }
}
