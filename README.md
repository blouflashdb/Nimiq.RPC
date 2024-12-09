# Nimiq.RPC SDK

## Overview

DemoApp is a sample application that demonstrates how to use the Nimiq.RPC SDK to interact with a Nimiq node. 
The application reads configuration settings from an `appsettings.json` file and makes an RPC call to get the current epoch number.

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or any other C# IDE
- Internet connection to access the Nimiq node

## Configuration

The application uses an `appsettings.json` file to store the RPC settings. Create an `appsettings.json` file in the root of your project directory with the following content:
`` 
{ 
"RPCSettings": { 
"RpcUrl": "http://localhost:8648", 
"Username": "super", 
"Password": "secret" 
} 
}``

Replace the values with your actual RPC URL, username, and password.

## How to Run

1. Clone the repository or download the source code.
2. Open the project in Visual Studio 2022 or your preferred C# IDE.
3. Ensure that the `appsettings.json` file is in the root of your project directory and contains the correct RPC settings.
4. Build the project to restore the necessary NuGet packages.
5. Run the application.

## Code Explanation

The `Program.cs` file contains the main logic of the application. It reads the RPC settings from the `appsettings.json` file, creates an instance of `NimiqHttpClient`, and makes an RPC call to get the current epoch number.



## Dependencies

- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Json`
- `System.Text.Json`

## Roadmap and TODO List

## Phase 1: Core Features

### RPC Client Integration
- [x] **HTTP Client for Nimiq RPC**: Implement the HTTP client for basic RPC calls.
- [x] **WebSocket Client for Nimiq RPC**: Implement WebSocket client for listening to blockchain events (e.g., new blocks, transactions).
- [x] **Authentication Handling**: Implement username/password authentication via Basic Auth for both HTTP and WebSocket clients.

## Phase 2: Enhancements and New Features

### Expand HTTP Client Method Calls
- [ ] **Individual RPC Method Calls**: Add support for additional Nimiq RPC methods like `getBlockByNumber`, `getTransactionCount`, `getBlockCount`, etc.
- [ ] **Custom RPC Methods**: Add flexibility to call custom methods as per the Nimiq node's configuration.

### Expand WebSocket Client
- [ ] **Individual RPC Method Calls**: Implement functionality to listen for new blocks, transactions, or other events via WebSocket connection.
- [ ] **Reconnect Logic**: Implement automatic reconnect functionality for WebSocket in case of disconnections.

## Phase 3: Models, Types, and Entities

### Create Data Models and Types
- [ ] **Transaction Model**: Define models for transactions that include relevant properties (e.g., hash, block, sender, recipient, value).
- [ ] **Block Model**: Create models for blocks that include block hash, number, timestamp, and other block-level data.
- [ ] **Response Models**: Define common response models for RPC responses, including error handling.

### Serialization and Deserialization
- [ ] **JSON Response Handling**: Implement methods to handle JSON serialization and deserialization for HTTP and WebSocket responses.

## Phase 4: Advanced Features

### Error Handling and Logging
- [ ] **Error Models**: Define a model for errors in both HTTP and WebSocket communication.
- [ ] **Centralized Logging**: Add logging capabilities for RPC call errors, WebSocket disconnections, and response parsing.

## Phase 5: Documentation and Testing

### Improve Documentation
- [ ] **Comprehensive README**: Update the README file with detailed instructions, examples, and use cases for both HTTP and WebSocket clients.
- [ ] **Inline Code Documentation**: Add XML comments and explanations to the code for better understanding and usage.

### Add Unit and Integration Tests
- [ ] **Unit Testing**: Write unit tests for HTTP and WebSocket clients to ensure correctness of individual methods.
- [ ] **Integration Testing**: Implement integration tests to simulate real RPC and WebSocket interactions with a Nimiq node.



## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
