using System.Net;
using System.Text;

namespace TimeCapsuleServer;

public class ServerListener
{
    public bool keepRunning;

    public void StartServer(RequestHandler requestHandler)
    {
        keepRunning = true;

        //create an HTTP listener and tell it to bind to http/s
        var listener = new HttpListener();
        listener.Prefixes.Add("http://*:2736/");
        //listener.Prefixes.Add("https://*:2736/");

        //start the listener and wait for a request
        listener.Start();

        //run and handle requests until we are told to stop
        while (keepRunning)
            try
            {
                //get the request data from the client
                var context = listener.GetContext();
                var request = context.Request;

                var query = request.QueryString;
                //they didnt tell us what they wanted
                if (query.Count == 0)
                    //Console.WriteLine("Got an empty request");
                    continue;

                //we dont know what they want
                if (!requestHandler.QueryHandlers.ContainsKey(query.Keys[0]))
                {
                    Console.WriteLine("Got an unknown request");
                    Console.WriteLine(request.QueryString.ToString());
                    continue;
                }

                //pull the handler out of the func and do it
                var response = context.Response;
                var responseText = requestHandler.QueryHandlers[query.Keys[0]](query);


                var buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Flush();
                output.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("We broke something");
                Console.WriteLine(e);
            }
    }
}