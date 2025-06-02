# Library Management System

A web-based library management system built with ASP.NET Core MVC, Entity Framework Core, and SQLite.

## Project Overview
This application allows you to manage books, members, and lending operations for a library. Data is stored in JSON, XML, and flat files. The system features:
- **Book Management**: Add, search, and list books
- **Member Management**: Register and list members
- **Lending System**: Lend and return books, view lending history
- **Data Storage**: Books (JSON), Members (XML), Borrow Records (flat file)
- **Logging**: All lending/return actions are logged to `Data/logs.txt`

## Architecture & Best Practices
- **Clean Architecture**: Controllers and services are modular, testable, and follow separation of concerns.
- **Dependency Injection**: All services are injected and configured in `Program.cs` for maintainability and testability.
- **Async/Await**: All I/O operations (file/database access) use async/await for scalability and responsiveness.
- **Robust Null Handling**: All code and views include null checks to prevent runtime errors.
- **English UI & Comments**: All user interface text and code comments are in English.
- **Copilot Custom Instructions**: See [.githubcopilotinstructions.md](.githubcopilotinstructions.md) for project-specific Copilot guidance (C# 10, clean architecture, DI, async/await, English comments).

## Installation Guide
1. **Install .NET 9 SDK** (or the version specified in `libraryManagementSystem.csproj`)
2. **Clone the repository**
3. **Restore dependencies**:
   ```powershell
   dotnet restore
   ```
4. **Build the project**:
   ```powershell
   dotnet build
   ```
5. **Run the application**:
   ```powershell
   dotnet run
   ```
6. **Access the app**: Open your browser at the URL shown in the console (e.g., http://localhost:5161)

## Screenshots
- ![Add Book](Screenshots/AddBook.png)
- ![Book List](Screenshots/BookList.png)
- ![Borrowing History](Screenshots/BorrowingHistory.png)
- ![Lend Book](Screenshots/LendBook.png)
- ![Members List](Screenshots/MembersList.png)
- ![Register Member](Screenshots/RegisterMember.png)

## Testing
- **xUnit Modular Tests**: All services and integration logic are covered by modular xUnit tests in the `Tests/` folder.
- **Test Data Isolation**: Each test uses its own test data files and ensures the test data directory exists.
- **Sequential Execution**: Tests are marked with `[Collection("Sequential")]` to avoid file access conflicts.
- **Run all tests**:
   ```powershell
   dotnet test
   ```

## Example GitHub Copilot Prompts Used
- "Build a web-based library management system using ASP.NET Core MVC and Entity Framework Core with SQLite."
- "Refactor controllers and services to use dependency injection and async/await."
- "Add robust null checks or strongly-typed view models to Borrow/Index.cshtml to prevent runtime errors."
- "Implement logging for all lend and return actions to logs.txt."
- "Write modular xUnit test classes for BookService, MemberService, BorrowService, DataSeeder, and integration tests."
- "Update .csproj to copy Data/ files to output directory for tests."
- "Fix CS8602 warnings by adding Assert.NotNull before dereferencing possibly null objects in tests."
- "Write a README.md in English with project overview, installation, screenshots, Copilot prompts, and development challenges."
- "Add runtime logging for all lending/return actions and ensure logs are written to Data/logs.txt."
- "Ensure all test data files are copied to the output directory for reliable automated testing."
- "Implement in-memory service classes for modular and isolated unit testing."

## Development Challenges
- **Null Reference Handling**: Ensuring all dynamic view models and data loading logic are robust against missing or inconsistent data.
- **File-Based Data Storage**: Coordinating JSON, XML, and flat file access and updates, especially for concurrent operations and testability.
- **Test Data Availability**: Ensuring test data files are always available in the test output directory for reliable automated testing.
- **Integration of Logging**: Implementing runtime logging for all lending/return actions in a way that is robust and does not impact performance.
- **Localization**: Ensuring the UI is consistently in English and navigation is user-friendly.
- **Async/Await Refactoring**: Migrating all I/O and controller logic to async/await for scalability.
- **Dependency Injection**: Refactoring all services and controllers to use DI for maintainability and testability.

---

**Note:** This project is a functional prototype and can be extended with features like editing/deleting books/members, advanced validation, and improved UI/UX.
