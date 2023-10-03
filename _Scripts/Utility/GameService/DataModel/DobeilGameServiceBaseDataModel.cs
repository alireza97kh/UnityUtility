using UnityEngine.Networking;

namespace Dobeil
{
    public class HttpResponse
    {
        public int StatusCode { get; private set; }
        public string Error { get; private set; }
        public string Text { get; private set; }

        public HttpResponse(UnityWebRequest request)
        {
            StatusCode = (int)request.responseCode;
            Error = request.isNetworkError || request.isHttpError ? request.error : null;
            Text = request.downloadHandler.text;
        }
    }

}