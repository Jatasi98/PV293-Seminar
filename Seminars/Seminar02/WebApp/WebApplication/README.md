# Seminar 02

This seminar introduces the e-commerce monolith that will be our foundation throughout the course. Your objective is to comprehend its purpose, identify clear design issues, implement enhancements, and begin crafting the initial automated tests.

Everything you accomplish today will establish the baseline for the subsequent transformations into moduliths and microservices.

## TASK 1 – Install an IDE that supports ASP.NET Core

**Do _not_ use Visual Studio Code for this course.** It lacks several features (proper Razor debugging, integrated Docker‑compose tooling, and graphical designers) that will save you time later.

### Option A – Visual Studio 2022 (Windows or WSL)
1. Download the Community Edition from <https://visualstudio.microsoft.com>.
2. In the installer, select **ASP.NET and web development**.
3. (Optional) Add **Data storage and processing** if you want SQL tooling.

### Option B – JetBrains Rider 2024.x (macOS or Linux)
1. Obtain your university licence key: <https://it.muni.cz/sluzby/software/intellij-idea-a-nastroje-jetbrains>.
2. Install Rider and let the first‑run wizard detect your .NET SDK.

> **Why not Visual Studio for Mac?** Microsoft retired it in 2024, so security updates have stopped.

After installation, verify that `dotnet --version` prints at least **9.0.304** (the SDK we will target all semester). If you do not have the required version even after IDE installation, proceed to https://dotnet.microsoft.com/en-us/download/dotnet/9.0 and download the SDK version supported by your system.

## TASK 2 – Install Docker Desktop

1. Download Docker Desktop from <https://www.docker.com/products/docker-desktop/>.
2. Reboot if the installer asks you to do so.
3. Open a terminal and run:
`docker version`

## TASK 3 – Clone and run the project

Clone the seminar repository:

 git clone https://github.com/Jatasi98/PV293-Seminar.git

Now open the **Solution Explorer** and study the layout.

Here are some folders you can expect to be present in the solution and their general purpose.

| Folder/file | Purpose |
|-------------|---------|
| `Dependencies` | NuGet packages and project references |
| `Properties/launchSettings.json` | Debug profiles and port mappings |
| `wwwroot` | Static assets (CSS, JS, images) exposed directly by the web server |
| `Controllers` | Classes that map HTTP routes to C# actions |
| `Models` | View‑specific DTOs used by Razor pages |
| `Views` | Razor templates grouped by controller name |
| `Program.cs` | Host builder, middleware pipeline, DI container configuration |

Build and launch the project using **F5** (Visual Studio) or the **Run** button (Rider). You should see the default home page at `https://localhost:44348/`.

## TASK 4 - *STOP* - What is wrong with this WebApp?

Now take a step back. For now, terminate your running application and focus on the entire solution. Try to identify everything that is wrong with it. Find at least five things that are incorrectly used or implemented.

After some time, we will discuss with the other students, and you will present what you found and what you believe is wrong.

## TASK 5 - Update WebApp and try to correct the solution

Now that you have identified the mistakes, your next task is to update the WebApp and correct the issues you found.

Go through the solution, fix the incorrect implementations, and apply proper practices where needed.

<details>
  <summary>Do not know what steps to take? - show more</summary>

```
1. Try to separate DAL and BL from WebApp (PL)
2. Instead of sending the whole entity from DAL to PL, use DTO and ModelViews instead
3. Is BaseRepository correctly used? If not, try to create a proper repository using the repository pattern
4. Bonus - Do we have admin and non-admin user areas separated correctly (eg, can a non-admin user use an admin action in our WebApp)? If not, try to update it  
```
</details>

<br>
Ensure your changes improve the application's readability, maintainability, and functionality.

## TASK 6 - Try to implement your own Middleware

Create a custom ASP.NET Core middleware that logs request/response metadata and enforces a simple rule.

**Requirements:**
* Define a sealed middleware class with a RequestDelegate constructor and an Invoke(HttpContext) method.
* Log: timestamp (UTC), HTTP method, path, status code, and elapsed milliseconds.
* Enforce rule: If request contains header X-Block: true, end request and return 403 Forbidden
* No third-party logging packages. Use ILogger<T>.

<details>
  <summary>Do you need help? - show more</summary>
  <a>https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-9.0</a>
</details>

## TASK 7 - Create new functionality in WebApp

Add one end-to-end feature: database entity → service (business logic) → controller (HTTP) → minimal views or API endpoints.

**Scope:**
* One new entity with proper keys, validation, *and relationships*.
* One service interface + implementation
* One controller exposing CRUD or a focused subset.
* EF Core migration applied and runnable.
* Unit tests for service (*minimal integration test for one controller route*).

<br>

**Constraints:**
* No business logic in controllers.
* No direct DbContext usage from controllers.
* DTOs/ViewModels for I/O (*no entity leakage*).
* Handle bad input, not-found, and conflicts with correct HTTP codes.

<details>
  <summary>Possible new e-commerce functions - show more</summary>
<li> Wishlist
<li> Product Reviews & Ratings
<li> Coupons/Promotions
</details>