using System.Net;
using System.Text;

namespace TimeCapsuleServer;

public class ServerListener
{
    public bool keepRunning;

    public void StartServer(RequestHandler requestHandler)
    {
        keepRunning = true;

        //create an HTTP listener and tell it to bind to http/
        
        var listener = new HttpListener();
        Console.WriteLine("made http listener");
        listener.Prefixes.Add("http://*:2736/");
        Console.WriteLine("asked for the port");
        //listener.Prefixes.Add("https://*:2736/");

        //start the listener and wait for a request
        listener.Start();
        Console.WriteLine("started....");
        //run and handle requests until we are told to stop
        while (keepRunning)
            try
            {
                //get the request data from the client
                var context = listener.GetContext();
                Console.WriteLine("someone is here");
                var request = context.Request;

                var query = request.QueryString;
                //they didnt tell us what they wanted
                if (query.Count == 0)
                {
                    Console.WriteLine("Got an empty request");
                    continue;
                }

                //we dont know what they want
                if (!requestHandler.QueryHandlers.ContainsKey(query.Keys[0]))
                {
                    Console.WriteLine("Got an unknown request");
                    Console.WriteLine(request.QueryString.ToString());
                    continue;
                }
                
                Console.WriteLine("we can handle this");
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