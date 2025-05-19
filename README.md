# OpenAI Proxy Server

A lightweight ASP.NET Core Web API designed to securely handle and forward requests to the OpenAI API. Acts as a proxy between your front-end application and OpenAI, keeping your API key hidden from the client side.

---

## Features

- Secure API key protection  
- Request forwarding for OpenAI endpoints (api/response)  
- Lightweight and high-performance  
- Easy integration with C#, Unity, WPF, or other .NET clients  

---

## Why This Exists

Embedding your OpenAI API key in a client-side application (like a game overlay) poses a security risk. This proxy server lets your client send requests without ever exposing your key.

---

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later  
- An OpenAI API key  
- An OpenAI assistant created at https://platform.openai.com/assistants/

---

## Getting Started

1. **Clone the repository**  
   ```bash
   git clone https://github.com/mmahoney09110/OpenAI-Proxy-Server.git
   cd OpenAI-Proxy-Server
   ```

2. **Configure your API key**  
   - **Option A**: Edit `appsettings.json`  
     ```json
     {
       "OpenAI": {
         "APIKey": "your-openai-api-key"
       }
     }
     ```  
   - **Option B**: Set an environment variable  
     ```bash
     setx OPENAI__APIKEY "your-openai-api-key"
     ```
3. **Configure your assistant key**  
   - **In Services/AiServiceVectorStore.cs**: Edit Line 31 `assistantId`  
     ```C#
     var assistentId = "asst_YOUR-Assistant-ID";
     ```  

3. **Run the server**  
   ```bash
   dotnet run
   ```

---

## API Usage

### POST `/api/response`

**Request Body**
```cmd
curl -X POST https://YourDomain.example/api/response ^
     -H "Content-Type: application/x-www-form-urlencoded" ^
     --data "Body=Hello AI!"
```

**Response**  
Returns the raw OpenAI response object, which your client can parse and display.

---

## Deployment

This application can be deployed to any .NET-compatible hosting environment:

- Azure App Service  
- Railway  
- Docker (self-hosted or any container platform)  
- VPS or dedicated server  

---

## Configuration & Environment

You can adjust settings in `appsettings.json`, including rate limits, logging levels, and CORS policy.  
To override in production, use environment variables with the same key structure (e.g., `OpenAI__APIKey`).

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Acknowledgments

- Built for the Elden Ring AI Companion Project  
- Powered by .NET 9 and OpenAI  
