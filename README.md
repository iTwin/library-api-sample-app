# Library API Sample App

Copyright © Bentley Systems, Incorporated. All rights reserved.

An iTwin sample application that demonstrates following workflows using the Library API.
- Upload a component to a catalog
- Query and download components from a catalog
- Create a component and its referenced entites i.e. catalog, manufacturer, application and category. Create and get component related entities i.e. document, variation and webLink.

This application contains sample code that should not be used in a production environment. It contains no retry logic and minimal logging/error handling.


## Prerequisites

* [Git](https://git-scm.com/)
* Visual Studio 2019 or [Visual Studio Code](https://code.visualstudio.com/)
* [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0/)


## Development Setup (Visual Studio 2019)

1. Clone Repository

2. Open iTwinLibrarySampleApp.sln and Build

3. (Optional) Put breakpoint in Program.cs

4. Run to debug

5. It will require a user token. 

   * Go to the Library API [developer portal](https://developer.bentley.com/apis/library/operations/create-catalog/)
   * Click the TryIt Button
   * In the popup window, select authorizationCode in the Bentley OAuth2 Service dropdown
   * This will popup another window that will require you to login.
   * After you login, the Authorization header will be populated. Copy the entire string and paste into the command window for the iTwin Sample Library App.
   * Press Enter

6. You can now step through the code that will create and manage component and related entites.
