using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.VectorStores;
using System.ClientModel;
using System.Text;
using System.Text.RegularExpressions;


namespace OpenAI.Examples
{
    public class AiServiceVectorStore
    {
        private readonly string _apiKey;


        public AiServiceVectorStore(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:APIKey"] ?? string.Empty;
            Console.WriteLine($"[DEBUG] API Key Loaded: {(_apiKey.Length > 0 ? "Yes" : "No")}");
        }

        public async Task<string> GenerateResponseAsync(string stats)
        {
#pragma warning disable OPENAI001
            Console.WriteLine("[DEBUG] Starting GenerateResponseAsync...");
            OpenAIClient openAIClient = new(_apiKey);
            OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
            AssistantClient assistantClient = openAIClient.GetAssistantClient();
            var vectorStoreClient = openAIClient.GetVectorStoreClient();

            var assistantId = "asst_a3nc4C3fUnmfyIDkNNd4enHB";

            // Create a thread with the student's and chat bot's past 5 message and wait for the response.
            Console.WriteLine($"[DEBUG] Creating thread with student's message: {stats}");

            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { stats },
            };

            ThreadRun threadRun = null; // Declare threadRun outside try-catch

            try
            {
                threadRun = await assistantClient.CreateThreadAndRunAsync(assistantId, threadOptions);
                Console.WriteLine($"[DEBUG] Thread created. Thread ID: {threadRun.ThreadId}, Run ID: {threadRun.Id}");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No assistant found with id") || ex.Message.Contains("Value cannot be null. (Parameter 'assistantId')"))
                {
                    Console.WriteLine("[ERROR] Assistant not found.");
                }
                else
                {
                    // Log or rethrow other exceptions
                    Console.WriteLine($"[ERROR] Unexpected error: {ex.Message}");
                    throw;
                }
            }

            // Wait for thread run to complete.
            Console.WriteLine("[DEBUG] Waiting for thread run to complete...");
            do
            {
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                threadRun = await assistantClient.GetRunAsync(threadRun.ThreadId, threadRun.Id);
                Console.WriteLine($"[DEBUG] Thread run status: {threadRun.Status}");
            } while (!threadRun.Status.IsTerminal);
            Console.WriteLine("[DEBUG] Thread run completed.");

            // Retrieve and process the AI's response.
            Console.WriteLine("[DEBUG] Retrieving thread messages...");
            CollectionResult<ThreadMessage> messages = assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });
            Console.WriteLine($"[DEBUG] Retrieved {messages.Count()} messages from thread.");

            string response = "";
            bool isFirstMessage = true;
            foreach (ThreadMessage message in messages)
            {
                Console.WriteLine($"[DEBUG] Processing message from {message.Role}");
                if (isFirstMessage)
                {
                    isFirstMessage = false;
                    Console.WriteLine("[DEBUG] Skipping first message (student's input).");
                    continue;
                }
                foreach (MessageContent contentItem in message.Content)
                {
                    if (!string.IsNullOrEmpty(contentItem.Text))
                    {
                        Console.WriteLine($"[DEBUG] Message content: {contentItem.Text}");
                        response += contentItem.Text + "\n";
                    }
                }
            }

            // Efficiently delete only the thread so that the assistant and files remain for reuse.
            Console.WriteLine("[DEBUG] Cleaning up: Deleting thread only...");
            await assistantClient.DeleteThreadAsync(threadRun.ThreadId);

            Console.WriteLine("[DEBUG] Cleanup complete.");

            // Ensure the response is within 1600 characters and removes citation.
            Console.WriteLine($"[DEBUG] Final response length before truncation: {response.Length} characters.");
            response = response.Length > 1600 ? response.Substring(0, 1599) : response;
            response = Regex.Replace(response, "【.*?】", "");
            Console.WriteLine("[DEBUG] Final response generated.");
            Console.WriteLine($"[DEBUG] Response:\n{response}");

            return response.Trim();
        }

    }
}
