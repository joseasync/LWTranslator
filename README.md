# LWTranslator Application

The **LWTranslator** project is a web-based application built with Blazor for the frontend and a .NET 8 Web API for the backend. This application allows users to translate text between languages by leveraging the Google Translate API. It includes features for selecting source and target languages, entering text, and retrieving translations.

### Demo

[![LWTranslator Application - Demo]()](https://youtu.be/9eljBy4wmnA)


<br>

## Features

- **Language Selection**: Users can select both the source and target languages for translation from a dropdown list of available languages.
- **Text Translation**: Users can enter text to be translated, which is then processed and translated via the Google Translate API.
- **Real-Time Translation**: The application sends requests to the Web API, which interacts with Google Translate, providing translated text instantly upon request.

## Project Structure

- **LW.Client**: Contains the Blazor frontend application that provides the user interface and handles interactions.
- **LW.Api**: Contains the .NET 8 Web API backend, which processes translation requests and interacts with the Google Translate API.

## Prerequisites

- **Docker**: Ensure Docker is installed and running.
- **Google Translate API Key**: A valid Google Translate API key is required for translation services.

## Setup Instructions

### Environment Variable

To enable Google Translate API integration, set an environment variable named `GOOGLE_TRANSLATE_API_KEY` with your Google Translate API key. You can set this variable either on your machine or directly in the Dockerfile.

#### Setting the Environment Variable

1. **On Your Computer**:
   - On Windows:
     ```shell
     set GOOGLE_TRANSLATE_API_KEY=your_api_key_here
     ```
   - On macOS/Linux:
     ```shell
     export GOOGLE_TRANSLATE_API_KEY=your_api_key_here
     ```

2. **In the Dockerfile**:
   You can also add the `GOOGLE_TRANSLATE_API_KEY` environment variable in each service's Dockerfile to pass the key directly to the container.

   ```dockerfile
   ENV GOOGLE_TRANSLATE_API_KEY=your_api_key_here
   ```

### Docker Setup

1. **Clone the Repository**:
   ```shell
   git clone https://github.com/joseasync/LWTranslator.git
   cd LWTranslator
   ```

2. **Build and Run the Docker Containers**:
   Use Docker Compose to build and run the containers for both the Blazor frontend and the Web API.

   ```shell
   docker-compose up --build
   ```

   - **Blazor Application**: Runs on [http://localhost:7200](http://localhost:7200)
   - **Web API**: Runs on [http://localhost:8080](http://localhost:8080)

3. **Access the Application**:
   - Open a browser and navigate to [http://localhost:7200](http://localhost:7200) to start using the translation application.

<br>



## Frontend

### Registration and Login

- **Registration**: `http://localhost:7200/register`
- **Login**: `GET http://localhost:7200/login`


### Translation Feature

- **Translator**: `http://localhost:7200/translator`

------

## API Endpoints

### Available Languages

- **URL**: `GET /translator/languages/available`
- **Description**: Retrieves a list of languages supported by the Google Translate API.


### Translation Endpoint

- **Login**: Use the access token for authentication.
- **URL**: `POST /translator/generate`
- **Description**: Takes source and target languages along with the text to be translated and returns the translated text.
- **Request Body**:
  ```json
  {
    "fromLanguage": "en-US",
    "toLanguage": "fr-FR",
    "inputText": "Hello, world!"
  }
  ```

- **Response**:
  ```json
  {
    "language": "fr-FR",
    "text": "Bonjour, le monde!"
  }
  ```

### Notes

- **Error Handling**: The application includes error handling and logs errors in case of unsuccessful API calls.
- **Testing**: When testing locally without Docker, ensure the `GOOGLE_TRANSLATE_API_KEY` is correctly set in your environment.
